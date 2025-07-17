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

        entryMessage.EventClickSend += EventClickSend;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        this.BackgroundColor = userSessionService.Role == UserRole.User
        ? Constants.COLOR_USER
        : Constants.COLOR_COMPANY;

        await viewModel.Init();
    }

    private async void EventClickSend()
    {
        string message = entryMessage.GetEntryText();
        if (string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message)) return;

        if (await viewModel.SendMessage(message))
        {
            entryMessage.SetEntryText("");
        }
    }

    private async void Number_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(sender as Label);

        if (PhoneDialer.Default.IsSupported)
            PhoneDialer.Default.Open(viewModel.CompanyNumber);
    }

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(sender as Image);
        await viewModel.Disconnect();

        await Back();
    } 
}