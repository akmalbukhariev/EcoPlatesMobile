using System.Net.WebSockets;
using EcoPlatesMobile.Models.Chat;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.ViewModels.Chat;

namespace EcoPlatesMobile.Views.Chat;

public partial class ChattingPage : BasePage
{
    private ChattingPageViewModel viewModel;
    private ChatWebSocketService webSocketService;
    public ChattingPage(ChattingPageViewModel viewModel, ChatWebSocketService webSocketService)
    {
        InitializeComponent();

        this.viewModel = viewModel;
        this.webSocketService = webSocketService;

        BindingContext = viewModel;

        entryMessage.EventClickSend += EventClickSend;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        this.BackgroundColor = Color.FromArgb(Constants.COLOR_USER);

        if (webSocketService.State != WebSocketState.Open)
        {
            await webSocketService.ConnectAsync();
        }
    }

    private async void EventClickSend()
    {
        string message = entryMessage.GetEntryText();
        if (string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message)) return;

        try
        {
            if (webSocketService.State != WebSocketState.Open)
            {
                await webSocketService.ConnectAsync();
            }

            await webSocketService.SendMessageAsync(message);
            entryMessage.SetEntryText("");

            viewModel.Messages.Add(new Message
            {
                Text = message,
                Time = DateTime.Now.ToString("HH:mm"),
                MsgType = MessageType.Sender,
                BackColor = Color.FromArgb(Constants.COLOR_USER)
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Send failed: {ex.Message}");
        }
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