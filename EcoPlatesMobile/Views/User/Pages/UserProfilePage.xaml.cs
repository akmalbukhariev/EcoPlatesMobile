using CommunityToolkit.Mvvm.Input;
using EcoPlatesMobile.Models;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.User;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.ViewModels.User;
using EcoPlatesMobile.Views.Chat;
using EcoPlatesMobile.Views.Company.Pages;
using EcoPlatesMobile.Views.Components;
using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserProfilePage : BasePage 
{
    public ObservableCollection<LanguageModel> Languages { get; set; }

    private string _selectedLanguage;
    public string SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            _selectedLanguage = value;
            OnPropertyChanged(nameof(SelectedLanguage));
        }
    }

    private string _selectedFlag;
    public string SelectedFlag
    {
        get => _selectedFlag;
        set
        {
            _selectedFlag = value;
            OnPropertyChanged(nameof(SelectedFlag));
        }
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set
        {
            _isRefreshing = value;
            OnPropertyChanged(nameof(IsRefreshing));
        }
    }
    private bool _isRefreshing;
    
    GetUserInfoResponse response;

    private LanguageService languageService;
    private AppControl appControl;
    private CompanyApiService companyApiService;
    private UserApiService userApiService;
    private IStatusBarService statusBarService;
    private UserSessionService userSessionService;

    public UserProfilePage(LanguageService languageService,
                            AppControl appControl,
                            UserApiService userApiService,
                            CompanyApiService companyApiService,
                            IStatusBarService statusBarService,
                            UserSessionService userSessionService)
    {
        InitializeComponent();

        this.languageService = languageService;
        this.appControl = appControl;
        this.userApiService = userApiService;
        this.companyApiService = companyApiService;
        this.statusBarService = statusBarService;
        this.userSessionService = userSessionService;

        Init();

        BindingContext = this;
    }

    private void Init()
    {
        var currentLangCode = languageService.GetCurrentLanguage();

        Languages = new ObservableCollection<LanguageModel>
        {
            new LanguageModel { Name = Constants.LAN_UZBEK,   Flag = Constants.LAN_ICON_UZBEK, IsSelected = true,  Code = Constants.UZ },
            new LanguageModel { Name = Constants.LAN_ENGLISH, Flag = Constants.LAN_ICON_ENGLISH, IsSelected = false, Code = Constants.EN },
            new LanguageModel { Name = Constants.LAN_RUSSIAN, Flag = Constants.LAN_ICON_RUSSIAN, IsSelected = false, Code = Constants.RU }
        };

        foreach (var lang in Languages)
        {
            lang.IsSelected = lang.Code == currentLangCode;
            if (lang.IsSelected)
            {
                SelectedFlag = lang.Flag;
                SelectedLanguage = lang.Name;
            }
        }

        lbVersion.Text = $"v. {Constants.Version} ({Constants.OsName} - {Constants.OsVersion})";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetTabBarIsVisible(this, true);

        bool isWifiOn = await appControl.CheckWifiOrNetwork();
		if (!isWifiOn) return;

        if (!appControl.IsLoggedIn)
        {
            grdInnerUserInfo.IsVisible = false;
            lbLogInOrSignUp.IsVisible = true;
            imUserInfo.Source = "right.png";
            listTileUser.IsVisible = false;
            return;
        }

        listTileUser.IsVisible = true;
        statusBarService.SetStatusBarColor(Constants.COLOR_USER.ToArgbHex(), false);
        if (appControl.RefreshUserProfilePage)
        {
            await LoadData();
            appControl.RefreshUserProfilePage = false;
        }
    }

    public IRelayCommand RefreshCommand => new RelayCommand(async () =>
    {
        bool isWifiOn = await appControl.CheckWifiOrNetwork();
		if (!isWifiOn)
        {
            IsRefreshing = false;
            return;
        }
        
        IsRefreshing = true;
        await LoadData();
        IsRefreshing = false;
    });

    private async Task LoadData()
    {
        loading.ShowLoading = true;
        
        response = await userApiService.GetUserInfo();
        bool isOk = await appControl.CheckUserState(response);
        if (!isOk)
        {
            await appControl.LogoutUser();
            return;
        }
        
        if (response.resultCode == ApiResult.USER_EXIST.GetCodeToString())
        {
            imUser.Source = appControl.GetImageUrlOrFallback(response.resultData.profile_picture_url);
            lbUserName.Text = response.resultData.first_name;
            lbPhoneNumber.Text = response.resultData.phone_number;

            appControl.UserInfo = response.resultData;
        }

        loading.ShowLoading = false;
    }

    private async void UserInfo_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(grdUserInfo);

        if (!appControl.IsLoggedIn)
        {
            //await AppNavigatorService.NavigateTo(nameof(PhoneNumberRegisterPage));
            appControl.RefreshMainPage = true;
            appControl.RefreshBrowserPage = true;
            appControl.IsLoggedIn = false;
            Application.Current.MainPage = new AppEntryShell();
            return;
        }

        await AppNavigatorService.NavigateTo(nameof(UserProfileInfoPage));
    }

    private async void OnLanguageTapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            dropdownListBack.IsVisible = true;
            dropdownList.IsVisible = true;

            dropdownListBack.Opacity = 0.5;      // show dark overlay
            dropdownListBack.InputTransparent = false; // start catching taps
        });
    }

    private async void OnLanguageSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is LanguageModel selectedLang)
        {
            foreach (var lang in Languages)
                lang.IsSelected = false;

            selectedLang.IsSelected = true;
            SelectedLanguage = selectedLang.Name;
            SelectedFlag = selectedLang.Flag;

            dropdownListBack.IsVisible = false;
            dropdownList.IsVisible = false;

            await AlertService.ShowAlertAsync(AppResource.LanguageChanged,
                                             AppResource.MessageLanguageChanged,
                                             AppResource.Ok);

            languageService.SetCulture(selectedLang.Code);

            appControl.RefreshMainPage = true;
            appControl.RefreshBrowserPage = true;
            ((App)Application.Current).ReloadAppShell();
        }
    }

    private void OnLanguageBackTapped(object sender, TappedEventArgs e)
    {
        dropdownListBack.IsVisible = false;
        dropdownList.IsVisible = false;

        dropdownListBack.Opacity = 0;        // hide overlay
        dropdownListBack.InputTransparent = true;  // let taps pass through again
    }
    
    private async void Tile_EventClick(object obj)
    {
        ListTileView view = (ListTileView)obj;
        switch (view.TileType)
        {
            case ListTileView.ListTileType.Message:
                if (!appControl.IsLoggedIn)
                {
                    await AppNavigatorService.NavigateTo(nameof(PhoneNumberRegisterPage));
                    return;
                }
                await AppNavigatorService.NavigateTo(nameof(ChatedUserPage));
                break;
            case ListTileView.ListTileType.Share:
                if (!appControl.IsLoggedIn)
                {
                    await AppNavigatorService.NavigateTo(nameof(PhoneNumberRegisterPage));
                    return;
                }
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Uri = appControl.UserInfo.share_link,
                    Title = "SaleTop Application"
                });
                break;
            case ListTileView.ListTileType.Suggestions:
                if (!appControl.IsLoggedIn)
                {
                    await AppNavigatorService.NavigateTo(nameof(PhoneNumberRegisterPage));
                    return;
                }
                await AppNavigatorService.NavigateTo(nameof(SuggestionsPage));
                break;
            case ListTileView.ListTileType.AboutApp:
                await AppNavigatorService.NavigateTo(nameof(AboutPage));
                break;
            case ListTileView.ListTileType.SwitchRole:
                 {
                    loading.ShowLoading = true;
                    Response response = await companyApiService.CheckUser(appControl.UserInfo.phone_number);
                    loading.ShowLoading = false;

                    if (response.resultCode == ApiResult.COMPANY_EXIST.GetCodeToString())
                    {
                        userSessionService.SetUser(UserRole.Company);

                        loading.ShowLoading = true;
                        await appControl.LoginCompany(appControl.UserInfo.phone_number);
                        loading.ShowLoading = false;

                        statusBarService.SetStatusBarColor(Constants.COLOR_COMPANY.ToArgbHex(), false);
                    }
                    else if (response.resultCode == ApiResult.COMPANY_NOT_EXIST.GetCodeToString())
                    {
                        bool answer = await AlertService.ShowConfirmationAsync(
                                    AppResource.Confirm,
                                    AppResource.MessageEnterPhoneNumberNotRegisteredCompany,
                                    AppResource.Yes, AppResource.No);

                        if (!answer) return;

                        statusBarService.SetStatusBarColor(Constants.COLOR_COMPANY.ToArgbHex(), false);
                        await AppNavigatorService.NavigateTo($"{nameof(CompanyRegistrationPage)}?PhoneNumber={appControl.UserInfo.phone_number}");
                    }
                    else if (response.resultCode == ApiResult.BLOCK_USER.GetCodeToString())
                    {
                        appControl.StrBlockUntill = response.resultMsg;
                        appControl.ResultCode = ApiResult.BLOCK_USER;
                        await AlertService.ShowAlertAsync(AppResource.Info, AppResource.MessageBlocked);
                        await AppNavigatorService.NavigateTo(nameof(BlockedPage));
                        return;
                    }
                    else if (response.resultCode == ApiResult.DELETE_USER.GetCodeToString())
                    {
                        appControl.StrBlockUntill = response.resultMsg;
                        appControl.ResultCode = ApiResult.DELETE_USER;
                        await AlertService.ShowAlertAsync(AppResource.Info, AppResource.MessageSoftDelete);
                        await AppNavigatorService.NavigateTo(nameof(BlockedPage));
                        return;
                    }
                }
                break;
        }
    }
}