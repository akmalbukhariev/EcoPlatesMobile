using EcoPlatesMobile.Models;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using Svg.Skia;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EcoPlatesMobile.Views;

public partial class LanguagePage : BasePage
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

    LanguageModel selectedLang = null;

    private LanguageService languageService;

    public LanguagePage(LanguageService languageService)
	{
		InitializeComponent();
        this.languageService = languageService;

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
                selectedLang = lang;
            }
        }
    }

    private async void OnFrameTapped(object sender, EventArgs e)
    {
        if (!DropdownList.IsVisible)
        {
            DropdownList.Opacity = 0;
            DropdownList.IsVisible = true;
            await DropdownList.FadeTo(1, 250);
        }
        else
        {
            await DropdownList.FadeTo(0, 250);
            DropdownList.IsVisible = false;
        }
    }

    private async void OnLanguageSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is LanguageModel selectedLang)
        {
            foreach (var lang in Languages)
                lang.IsSelected = false;

            this.selectedLang = selectedLang;

            selectedLang.IsSelected = true;
            SelectedLanguage = selectedLang.Name;
            SelectedFlag = selectedLang.Flag;
             
            await DropdownList.FadeTo(0, 250);
            DropdownList.IsVisible = false;
        }
    }

    private async void Next_Clicked(object sender, EventArgs e)
    {
        var store = AppService.Get<AppStoreService>();
        store.Set(AppKeys.IsLanguageSet, true);

        languageService.SetCulture(selectedLang.Code);
        await AppNavigatorService.NavigateTo(nameof(LoginPage));
    }
}