namespace EcoPlatesMobile.Views.Company.Pages;

public partial class CompanyProfileInfoPage : BasePage
{
	public CompanyProfileInfoPage()
    {
		InitializeComponent();
    }
     
    private void BorderImage_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void Cancel_Clicked(object sender, EventArgs e)
    {

    }

    private void Done_Clicked(object sender, EventArgs e)
    {

    }

    private async void PhoneNumber_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(grdPhoneNumber);
        await Shell.Current.GoToAsync(nameof(PhoneNumberChangePage));
    }
}