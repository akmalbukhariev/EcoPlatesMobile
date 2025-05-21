namespace EcoPlatesMobile.Views;

public partial class PhoneNumberChangePage : BasePage
{
	public PhoneNumberChangePage()
	{
		InitializeComponent();

        lbTitle.Text = "You can change your Saletop number here. Your account and all your cloud data — messages, media, contacts, etc. will be moved to the new number.";
    }

    private async void Next_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(PhoneNumberNewPage));
    }
}