using System.Collections.ObjectModel;
using EcoPlatesMobile.Models;
using EcoPlatesMobile.Models.Responses.Company;
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

    CompanyProfileInfoResponse response;
    public CompanyProfilePage()
    {
        InitializeComponent();

        Languages = new ObservableCollection<LanguageModel>
        {
            new LanguageModel { Name = "O'zbekcha", Flag = "flag_uz.png", IsSelected = true },
            new LanguageModel { Name = "English", Flag = "flag_en.png", IsSelected = false },
            new LanguageModel { Name = "Русский", Flag = "flag_ru.png", IsSelected = false }
        };

        SelectedFlag = Languages[0].Flag;
        SelectedLanguage = Languages[0].Name;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetTabBarIsVisible(this, true);

        loading.IsRunning = true;

        var apiService = AppService.Get<CompanyApiService>();

        int compayId = (int)AppService.Get<AppControl>().CompanyInfo.company_id;
        response = await apiService.GetCompanyProfileInfo(compayId);
        if (response.resultCode == ApiResult.COMPANY_EXIST.GetCodeToString())
        {
            imCompany.Source = response.resultData.logo_url;
            lbCompanyName.Text = response.resultData.company_name;
            lbPhoneNumber.Text = response.resultData.phone_number;
            tileActive.TileText1 = response.resultData.active_products.ToString();
            tileNoActive.TileText1 = response.resultData.non_active_products.ToString();
        }

        loading.IsRunning = false;
    }

    private async void UserInfo_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(grdUserInfo);
        //string uri = $"?CompanyImage={response?.resultData.logo_url}&CompanyName={response?.resultData.company_name}&CompanyPhone={response?.resultData.phone_number}";

        //await Shell.Current.GoToAsync($"{nameof(CompanyProfileInfoPage)}{uri}");
 
        //string json = JsonConvert.SerializeObject(response.resultData);
        //await Shell.Current.GoToAsync($"{nameof(CompanyProfileInfoPage)}?CompanyProfileInfoJson={Uri.EscapeDataString(json)}");
 
        await AppNavigatorService.NavigateTo(nameof(CompanyProfileInfoPage), new Dictionary<string, object>
        {
            ["CompanyProfileInfo"] = response.resultData
        });
    }

    private void OnLanguageTapped(object sender, TappedEventArgs e)
    {
        dropdownListBack.IsVisible = true;
        dropdownList.IsVisible = true;
    }

    private void OnLanguageSelected(object sender, SelectionChangedEventArgs e)
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
                await Shell.Current.GoToAsync($"{nameof(ActiveProductPage)}?ShowBackQuery={true}&ShowTabBarQuery={false}");
                break;
            case ListTileView.ListTileType.PreviousAds:
                await Shell.Current.GoToAsync($"{nameof(NonActiveProductPage)}?ShowBackQuery={true}&ShowTabBarQuery={false}");
                break;
            case ListTileView.ListTileType.Share:
                break;
            case ListTileView.ListTileType.AboutApp:
                break;
            case ListTileView.ListTileType.Suggestions:
                break;
            case ListTileView.ListTileType.Message:
                break;
        }
    }
}