using System.Net.WebSockets;
using EcoPlatesMobile.Models.Chat;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.ViewModels.Chat;
using EcoPlatesMobile.Services.Api;

namespace EcoPlatesMobile.Views.Chat;
 
public partial class ChattingPage : BasePage
{
    private ChattingPageViewModel viewModel;
    
    private AppControl appControl;
    private UserSessionService userSessionService;
    
    public ChattingPage(ChattingPageViewModel viewModel, AppControl appControl, UserSessionService userSessionService)
    {
        InitializeComponent();

        this.viewModel = viewModel; 
        this.appControl = appControl;
        this.userSessionService = userSessionService;

        this.viewModel.ScrollToBottomRequested += ScrollToBottomRequested;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        this.BackgroundColor = userSessionService.Role == UserRole.User
        ? Constants.COLOR_USER
        : Constants.COLOR_COMPANY;
 
        if (userSessionService.Role == UserRole.User)
        {
            frameMessage.BorderColor = Constants.COLOR_USER;
            sendImage.Source = "send_user.png";
            loading.Color = Constants.COLOR_USER;
        }
        else if (userSessionService.Role == UserRole.Company)
        {
            frameMessage.BorderColor = Constants.COLOR_COMPANY;
            sendImage.Source = "send_company.png";
            loading.Color = Constants.COLOR_COMPANY;
        }
        
        bool isWifiOn = await appControl.CheckWifi();
		if (!isWifiOn) return;

        await viewModel.Init();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _ = viewModel.Disconnect();
    }

    private async void Number_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(sender as Label);

        if (PhoneDialer.Default.IsSupported)
            PhoneDialer.Default.Open(viewModel.ReceiverNumber);
    }

    private void ScrollToBottomRequested(object? sender, EventArgs e)
    {
        var lastItem = viewModel.Messages[^1];
        messageList.ScrollTo(item: lastItem, position: ScrollToPosition.End, animate: true);
    }

    private async void Send_Tapped(object sender, TappedEventArgs e)
    {
        await sendImage.ScaleTo(0.8, 100, Easing.CubicOut);
        await sendImage.ScaleTo(1.0, 100, Easing.CubicIn);

        bool isWifiOn = await appControl.CheckWifi();
		if (!isWifiOn) return;
        
        string message = editorMessage.Text;
        if (string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message)) return;

        if (await viewModel.SendMessage(message))
        {
            editorMessage.Text = "";
        }
    }

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(imBack); 
 
        await Back();
    } 
}