namespace EcoPlatesMobile.Views;

[QueryProperty(nameof(IsUser), nameof(IsUser))]
public partial class SuggestionsPage : BasePage
{
    private bool _isUser = false;
    public bool IsUser
    {
        get => _isUser;
        set
        {
            _isUser = value;
        }
    }

    public List<string> FeedbackTypes { get; set; } = new() { "Suggestions", "Complaints" };
    public string SelectedType { get; set; }

    public SuggestionsPage()
	{
		InitializeComponent();
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_isUser)
        {
            header.HeaderBackground = btnSubmit.BackgroundColor = Colors.Green;
            imFeedBack.Source = "user_feedback_icon.png";
        }
        else
        {
            header.HeaderBackground = btnSubmit.BackgroundColor = Color.FromArgb("#8338EC");
            imFeedBack.Source = "company_feedback_icon.png";
        }
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SelectedType) || string.IsNullOrWhiteSpace(messageEditor.Text))
        {
            await DisplayAlert("Missing Info", "Please select a type and enter your message.", "OK");
            return;
        }

        
        await DisplayAlert("Thank you!", "Your feedback has been submitted.", "Close");
        await AppNavigatorService.NavigateTo("..");

        typePicker.SelectedIndex = -1;
        messageEditor.Text = string.Empty;
    }
}