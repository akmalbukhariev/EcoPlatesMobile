using System.Collections.ObjectModel;
using EcoPlatesMobile.Models;

namespace EcoPlatesMobile.Views.Company.Pages;

public partial class CompanyProfilePage : ContentPage
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
}