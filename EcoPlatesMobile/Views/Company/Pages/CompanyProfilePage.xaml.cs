using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using EcoPlatesMobile.Models;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Views.Components;
using Newtonsoft.Json;

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
    private LanguageService languageService;
    private AppControl appControl;

    public CompanyProfilePage(CompanyApiService companyApiService, LanguageService languageService, AppControl appControl)
    {
        InitializeComponent();

        this.companyApiService = companyApiService;
        this.languageService = languageService;
        this.appControl = appControl;

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

        if (appControl.RefreshCompanyProfilePage)
        {
            await LoadData();
            appControl.RefreshCompanyProfilePage = false;
        }
    }

    public IRelayCommand RefreshCommand => new RelayCommand(async () =>
    {
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
            imCompany.Source = response.resultData.logo_url;
            lbCompanyName.Text = response.resultData.company_name;
            lbPhoneNumber.Text = response.resultData.phone_number;
            tileActive.TileText1 = response.resultData.active_products.ToString();
            tileNoActive.TileText1 = response.resultData.non_active_products.ToString();

            appControl.CompanyInfo = response.resultData;
        }

        loading.IsRunning = false;
    }

    private async void UserInfo_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(grdUserInfo);
      
        await AppNavigatorService.NavigateTo(nameof(CompanyProfileInfoPage));
    }

    private void OnLanguageTapped(object sender, TappedEventArgs e)
    {
        dropdownListBack.IsVisible = true;
        dropdownList.IsVisible = true;
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
        dropdownListBack.IsVisible = false;
        dropdownList.IsVisible = false;
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
                break;
            case ListTileView.ListTileType.Location:
                loading.IsRunning = true;
                var locationService = new LocationService();
                var location = await locationService.GetCurrentLocationAsync();
                
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
                break;
            case ListTileView.ListTileType.AboutApp:
                await AppNavigatorService.NavigateTo(nameof(AboutPage));
                break;
        }
    }
}