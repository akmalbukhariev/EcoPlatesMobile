using CommunityToolkit.Mvvm.Input;
using EcoPlatesMobile.Models;
using EcoPlatesMobile.Models.Responses.User;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.ViewModels.User;
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

    public UserProfilePage()
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
         
        lbVersion.Text = $"v. {Constants.Version} ({Constants.OsName} - {Constants.OsVersion})";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetTabBarIsVisible(this, true);

        AppControl control = AppService.Get<AppControl>();

        if (control.RefreshProfilePage)
        {
            await LoadData(control);
            control.RefreshProfilePage = false;
        }
    }

    public IRelayCommand RefreshCommand => new RelayCommand(async () =>
    {
        IsRefreshing = true;
        var control = AppService.Get<AppControl>();
        await LoadData(control);
        IsRefreshing = false;
    });

    private async Task LoadData(AppControl control)
    {
        loading.ShowLoading = true;

        var apiService = AppService.Get<UserApiService>();

        response = await apiService.GetUserInfo();
        if (response.resultCode == ApiResult.USER_EXIST.GetCodeToString())
        {
            imUser.Source = response.resultData.profile_picture_url;
            lbUserName.Text = response.resultData.first_name;
            lbPhoneNumber.Text = response.resultData.phone_number;

            control.UserInfo = response.resultData;
        }

        loading.ShowLoading = false;
    }
     

    private async void UserInfo_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(grdUserInfo);

        await AppNavigatorService.NavigateTo(nameof(UserProfileInfoPage));
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
            case ListTileView.ListTileType.Share:
                break;
            case ListTileView.ListTileType.Suggestions:
                await AppNavigatorService.NavigateTo($"{nameof(SuggestionsPage)}?IsUser={true}");
                break;
            case ListTileView.ListTileType.Message:
                break;
            case ListTileView.ListTileType.AboutApp:
                await AppNavigatorService.NavigateTo($"{nameof(AboutPage)}?IsUser={true}");
                break;
        }
    }
}