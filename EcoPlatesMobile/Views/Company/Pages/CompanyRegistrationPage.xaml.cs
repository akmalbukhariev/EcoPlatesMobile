using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using EcoPlatesMobile.Helper;

namespace EcoPlatesMobile.Views.Company.Pages;

public partial class CompanyRegistrationPage : ContentPage
{
	public ObservableCollection<CompanyTypeModel> CompanyTypeList { get; set; }

    private string _selectedCompanyType;
    public string SelectedCompanyType
    {
        get => _selectedCompanyType;
        set
        {
            _selectedCompanyType = value;
            OnPropertyChanged(nameof(SelectedCompanyType)); // ✅ Notify UI
        }
    }

	public CompanyRegistrationPage()
	{
		InitializeComponent();

		CompanyTypeList = new ObservableCollection<CompanyTypeModel>
        {
            new CompanyTypeModel { Type = "Cafe"},
            new CompanyTypeModel { Type = "Restaurant"},
            new CompanyTypeModel { Type = "Bakery"},
			new CompanyTypeModel { Type = "Fastfood"},
			new CompanyTypeModel { Type = "Super Market"}
        };

		BindingContext = this;
	}
     
    private async void CompanyTypeTapped(object sender, EventArgs e)
	{
		var popup = new CompanyTypePickerPopup(CompanyTypeList);
        var result = await this.ShowPopupAsync(popup);

        /*if (result is CompanyTypeModel selected)
        {
            labelCompanyType.Text = selected.Type;
        }*/
	}

	private async void CompanyTypeSelected(object sender, SelectionChangedEventArgs e)
    {
        
    }

	private void OnStartDateSelected(object sender, DateChangedEventArgs e)
	{
		//startTimeLabel.Text = e.NewDate.ToString("MMM d, HH:mm");
		//startTimeLabel.TextColor = Colors.Black;
	}

	private void OnEndDateSelected(object sender, DateChangedEventArgs e)
	{
		//endTimeLabel.Text = e.NewDate.ToString("MMM d, HH:mm");
		//endTimeLabel.TextColor = Colors.Black;
	}

	private void OnStartDateOrTimeChanged(object sender, EventArgs e)
	{
		/*var date = startDatePicker.Date;
		var time = startTimePicker.Time;

		var combined = date + time;
		startDateTimeLabel.Text = combined.ToString("dd MMM yyyy HH:mm");
		startDateTimeLabel.TextColor = Colors.Black;*/
	}
}