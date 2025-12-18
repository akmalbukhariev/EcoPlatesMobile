using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using EcoPlatesMobile.Models;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Views.Components;
using Newtonsoft.Json;
using EcoPlatesMobile.Views.Chat;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Views.User.Pages;

namespace EcoPlatesMobile.Views.Company.Pages;

public partial class CompanyProfilePage : BasePage
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

    private GetCompanyInfoResponse response;

    private CompanyApiService companyApiService;
    private UserApiService userApiService;
    private LanguageService languageService;
    private AppControl appControl;
    private LocationService locationService;
    private IStatusBarService statusBarService;
    private UserSessionService userSessionService;

    public CompanyProfilePage(CompanyApiService companyApiService,
                                UserApiService userApiService,
                                LanguageService languageService,
                                AppControl appControl,
                                LocationService locationService,
                                IStatusBarService statusBarService,
                                UserSessionService userSessionService)
    {
        InitializeComponent();

        this.companyApiService = companyApiService;
        this.userApiService = userApiService;
        this.languageService = languageService;
        this.appControl = appControl;
        this.locationService = locationService;
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
        statusBarService.SetStatusBarColor(Constants.COLOR_COMPANY.ToArgbHex(), false);

        bool isWifiOn = await appControl.CheckWifiOrNetwork();
		if (!isWifiOn) return;
        
        if (appControl.RefreshCompanyProfilePage)
        {
            await LoadData();
            appControl.RefreshCompanyProfilePage = false;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    
        cts?.Cancel();
        cts = null;
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
        loading.IsRunning = true;

        response = await companyApiService.GetCompanyInfo();
        if (response.resultCode == ApiResult.COMPANY_EXIST.GetCodeToString())
        {
            imCompany.Source = appControl.GetImageUrlOrFallback(response.resultData.logo_url);
            lbCompanyName.Text = response.resultData.company_name;
            lbPhoneNumber.Text = response.resultData.phone_number;
            tileActive.TileText1 = response.resultData.active_products.ToString();
            tileInactive.TileText1 = response.resultData.non_active_products.ToString();

            appControl.CompanyInfo = response.resultData;
        }

        loading.IsRunning = false;
    }

    private async void UserInfo_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            await AnimateElementScaleDown(grdUserInfo);

            await AppNavigatorService.NavigateTo(nameof(CompanyProfileInfoPage));
        });
    }

    private void OnLanguageTapped(object sender, TappedEventArgs e)
    {
        dropdownList.Opacity = 1;
        dropdownList.IsVisible = true;

        dropdownListBack.Opacity = 0.5;      // show dark overlay
        dropdownListBack.InputTransparent = false; // start catching taps
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

            ((App)Application.Current).ReloadAppShell();
        }
    }
    
    private void OnLanguageBackTapped(object sender, TappedEventArgs e)
    {
        dropdownList.IsVisible = false;

        dropdownListBack.Opacity = 0;        // hide overlay
        dropdownListBack.InputTransparent = true;  // let taps pass through again
    }

    private async void Tile_EventClick(object obj)
    {
        ListTileView view = (ListTileView)obj;
        switch (view.TileType)
        {
            case ListTileView.ListTileType.ActiveAds:
                await AppNavigatorService.NavigateTo($"{nameof(ActiveProductPage)}?ShowBackQuery={true}&ShowTabBarQuery={false}");
                break;
            case ListTileView.ListTileType.PreviousAds:
                await AppNavigatorService.NavigateTo($"{nameof(InActiveProductPage)}?ShowBackQuery={true}&ShowTabBarQuery={false}");
                break;
            case ListTileView.ListTileType.Share:
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Uri = appControl.CompanyInfo.share_link,
                    Title = "Check out my app"
                });
                break;
            case ListTileView.ListTileType.Location:
                cts = new CancellationTokenSource();
                loading.IsRunning = true;
                var location = await locationService.GetCurrentLocationAsync(cts.Token);
                loading.IsRunning = false;

                if (location == null)
                {
                    return;
                }
                await AppNavigatorService.NavigateTo(nameof(LocationPage));
                break;
            case ListTileView.ListTileType.Suggestions:
                await AppNavigatorService.NavigateTo(nameof(SuggestionsPage));
                break;
            case ListTileView.ListTileType.Message:
                await AppNavigatorService.NavigateTo(nameof(ChatedUserPage));
                break;
            case ListTileView.ListTileType.AboutApp:
                await AppNavigatorService.NavigateTo(nameof(AboutPage));
                break;
            case ListTileView.ListTileType.SwitchRole:
                {
                    loading.IsRunning = true;
                    Response response = await userApiService.CheckUser(appControl.CompanyInfo.phone_number);
                    loading.IsRunning = false;

                    if (response.resultCode == ApiResult.USER_EXIST.GetCodeToString())
                    {
                        userSessionService.SetUser(UserRole.User);
                        
                        loading.IsRunning = true;
                        await appControl.LoginUser(appControl.CompanyInfo.phone_number);
                        loading.IsRunning = false;

                        statusBarService.SetStatusBarColor(Constants.COLOR_USER.ToArgbHex(), false);
                    }
                    else if (response.resultCode == ApiResult.USER_NOT_EXIST.GetCodeToString())
                    {
                        bool answer = await AlertService.ShowConfirmationAsync(
                                    AppResource.Confirm,
                                    AppResource.MessageEnterPhoneNumberNotRegisteredUser,
                                    AppResource.Yes, AppResource.No);

                        if (!answer) return;

                        statusBarService.SetStatusBarColor(Constants.COLOR_USER.ToArgbHex(), false);
                        await AppNavigatorService.NavigateTo($"{nameof(UserRegistrationPage)}?PhoneNumber={appControl.CompanyInfo.phone_number}");
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