using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.ViewModels.Chat;

namespace EcoPlatesMobile.Views.Chat;

public partial class ChattingPage : BasePage
{
    private ChattingPageViewModel viewModel;
    public ChattingPage(ChattingPageViewModel viewModel)
	{
		InitializeComponent();

        this.viewModel = viewModel;

        BindingContext = viewModel;

        entryMessage.EventClickSend += EventClickSend;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        this.BackgroundColor = Color.FromArgb(Constants.COLOR_USER);
    }

    private void EventClickSend()
    {
        string message = entryMessage.GetEntryText();
        if (string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message)) return;



    }

    private async void Number_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(sender as Label);
    }

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(sender as Image);
        //await Back();
    }

}