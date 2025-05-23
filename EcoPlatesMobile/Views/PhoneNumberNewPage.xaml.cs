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
            ContinueButton.IsEnabled = !string.IsNullOrWhiteSpace(phoneEntry.Text);
            ContinueButton.BackgroundColor = ContinueButton.IsEnabled ? Color.FromArgb("#0088cc") : Color.FromArgb("#e0e0e0");
            ContinueButton.TextColor = ContinueButton.IsEnabled ? Colors.White : Colors.Gray;
        };
    }

    private void PhoneEntry_Completed(object sender, EventArgs e)
    {
        // Logic when user presses "done" or enter
    }

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(imBack);

        await Shell.Current.GoToAsync("..", true);
    }
}