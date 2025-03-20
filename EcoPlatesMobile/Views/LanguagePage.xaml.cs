using EcoPlatesMobile.Models;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using Svg.Skia;
using System.Collections.ObjectModel;

namespace EcoPlatesMobile.Views;

public partial class LanguagePage : ContentPage
{
    public ObservableCollection<LanguageModel> Languages { get; set; }
    public string SelectedLanguage { get; set; }
    public string SelectedFlag { get; set; }

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
            await DropdownList.FadeTo(1, 250); // Smooth fade-in animation
        }
        else
        {
            await DropdownList.FadeTo(0, 250);
            DropdownList.IsVisible = false;
        }
    }

    // Handle language selection
    private async void OnLanguageSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is LanguageModel selectedLang)
        {
            // Deselect previous selection
            foreach (var lang in Languages)
                lang.IsSelected = false;

            // Set new selection
            selectedLang.IsSelected = true;
            SelectedLanguage = selectedLang.Name;
            SelectedFlag = selectedLang.Flag;

            // Update UI
            OnPropertyChanged(nameof(SelectedLanguage));
            OnPropertyChanged(nameof(SelectedFlag));

            // Hide dropdown smoothly
            await DropdownList.FadeTo(0, 250);
            DropdownList.IsVisible = false;
        }
    }
}