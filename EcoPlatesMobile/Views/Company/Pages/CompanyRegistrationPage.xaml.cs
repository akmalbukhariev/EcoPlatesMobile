namespace EcoPlatesMobile.Views.Company.Pages;

public partial class CompanyRegistrationPage : ContentPage
{
	public CompanyRegistrationPage()
	{
		InitializeComponent();
	}
    
	private void OnStartTimeTapped(object sender, EventArgs e)
	{
		datePickerView.Show();
	}

	private void OnEndTimeTapped(object sender, EventArgs e)
	{
		datePickerView.Show();
	}

	private void OnDateSelected(object sender, DateTime selectedDate)
	{
		 
	}
}