using EcoPlatesMobile.Models;
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

        Languages = new ObservableCollection<LanguageModel>
        {
            new LanguageModel { Name = "O'zbekcha", Flag = "flag_uz.png", IsSelected = true },
            new LanguageModel { Name = "English", Flag = "flag_en.png", IsSelected = false },
            new LanguageModel { Name = "Русский", Flag = "flag_ru.png", IsSelected = false }
        };

        SelectedLanguage = Languages[0].Name;
        SelectedFlag = Languages[0].Flag;
        BindingContext = this;
    }

    // Handle tap on Frame to show Picker
    private async void OnFrameTapped(object sender, EventArgs e)
    {
        if (!DropdownList.IsVisible)
        {
            DropdownList.Opacity = 0;
            DropdownList.IsVisible = true;
            await DropdownList.FadeTo(1, 250); // ✅ Smooth fade-in
        }
        else
        {
            await DropdownList.FadeTo(0, 250);
            DropdownList.IsVisible = false; // ✅ Hide after fade-out
        }
    }

    // Handle language selection
    private async void OnLanguageSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is LanguageModel selectedLang)
        {
            // ✅ Deselect previous selection
            foreach (var lang in Languages)
                lang.IsSelected = false;

            // ✅ Set new selection
            selectedLang.IsSelected = true;
            SelectedLanguage = selectedLang.Name;
            SelectedFlag = selectedLang.Flag;

            // ✅ Hide dropdown smoothly
            await DropdownList.FadeTo(0, 250);
            DropdownList.IsVisible = false;
        }
    }
} 