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

    public LanguagePage()
	{
		InitializeComponent();

        Init();

        BindingContext = this;
    }

    private void Init()
    {
        var currentLangCode = AppService.Get<LanguageService>().GetCurrentLanguage();

        Languages = new ObservableCollection<LanguageModel>
        {
            new LanguageModel { Name = "O'zbekcha", Flag = "flag_uz.png", IsSelected = true,  Code = Constants.UZ },
            new LanguageModel { Name = "English",   Flag = "flag_en.png", IsSelected = false, Code = Constants.EN },
            new LanguageModel { Name = "Русский",   Flag = "flag_ru.png", IsSelected = false, Code = Constants.RU }
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

            selectedLang.IsSelected = true;
            SelectedLanguage = selectedLang.Name;
            SelectedFlag = selectedLang.Flag;

            AppService.Get<LanguageService>().SetCulture(selectedLang.Code);

            await DropdownList.FadeTo(0, 250);
            DropdownList.IsVisible = false;
        }
    }
} 