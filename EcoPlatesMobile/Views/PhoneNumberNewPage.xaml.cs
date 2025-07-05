namespace EcoPlatesMobile.Views;

public partial class PhoneNumberNewPage : BasePage
{
	public PhoneNumberNewPage()
	{
		InitializeComponent();

        this.Loaded += (s, e) =>
        {
            phoneEntry.Focus();
        };
 
        phoneEntry.TextChanged += (s, e) =>
        {
            btnContinue.IsEnabled = !string.IsNullOrWhiteSpace(phoneEntry.Text);
            btnContinue.BackgroundColor = btnContinue.IsEnabled ? Color.FromArgb("#0088cc") : Color.FromArgb("#e0e0e0");
            btnContinue.TextColor = btnContinue.IsEnabled ? Colors.White : Colors.Gray;
        };
    }

    private void PhoneEntry_Completed(object sender, EventArgs e)
    {
        // Logic when user presses "done" or enter
    }

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(imBack);

        await AppNavigatorService.NavigateTo("..");
    }
}