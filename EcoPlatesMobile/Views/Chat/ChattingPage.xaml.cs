using System.Net.WebSockets;
using EcoPlatesMobile.Models.Chat;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.ViewModels.Chat;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Views.User.Pages;
using Microsoft.Maui.Platform; 


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

#if IOS
        // Stop iOS from moving the whole page up when the keyboard appears
        KeyboardAutoManagerScroll.Disconnect();
#endif

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

        bool isWifiOn = await appControl.CheckWifiOrNetwork();
        if (!isWifiOn) return;

        await viewModel.Init();
    }

    protected override void OnDisappearing()
    {
#if IOS
        // Re-enable default behavior for other pages
        KeyboardAutoManagerScroll.Connect();
#endif
        base.OnDisappearing();
        _ = viewModel.Disconnect();
    }

    private async void Number_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            await AnimateElementScaleDown(sender as Label);

            if (PhoneDialer.Default.IsSupported)
                PhoneDialer.Default.Open(viewModel.ReceiverNumber);
        });
    }

    private void ScrollToBottomRequested(object? sender, EventArgs e)
    {
        if (viewModel.Messages == null || viewModel.Messages.Count == 0)
            return;

        var lastItem = viewModel.Messages[^1];
        messageList.ScrollTo(item: lastItem, position: ScrollToPosition.End, animate: true);
    }

    private async void Send_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            await sendImage.ScaleTo(0.8, 100, Easing.CubicOut);
            await sendImage.ScaleTo(1.0, 100, Easing.CubicIn);

            bool isWifiOn = await appControl.CheckWifiOrNetwork();
            if (!isWifiOn) return;

            string message = editorMessage.Text;
            if (string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message)) return;

            if (await viewModel.SendMessage(message))
            {
                editorMessage.Text = "";
            }
        });
    }

    private async void Receiver_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            await AnimateElementScaleDown(borderReceiver);

            if (userSessionService.Role == UserRole.Company) return;

            await AppNavigatorService.NavigateTo($"{nameof(UserCompanyPage)}?CompanyId={viewModel.ChatPageModel.ReceiverId}");
        });
    }

    private const int MaxMessageLength = 300;
    private void EditorMessage_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue != null && e.NewTextValue.Length > MaxMessageLength)
        {
            editorMessage.Text = e.NewTextValue.Substring(0, MaxMessageLength);
        }
    }

    private void EditorMessage_Focused(object sender, FocusEventArgs e)
    {
    #if IOS
        // Push content up so the editor sits above the keyboard
        // You can tweak 330 depending on device / design
        rootGrid.Padding = new Thickness(0, 0, 0, 330);

        // Optional: scroll messages to bottom so last message is visible
        if (viewModel?.Messages != null && viewModel.Messages.Count > 0)
        {
            var lastItem = viewModel.Messages[^1];
            messageList.ScrollTo(
                item: lastItem,
                position: ScrollToPosition.End,
                animate: true
            );
        }
    #endif
    }

    private void EditorMessage_Unfocused(object sender, FocusEventArgs e)
    {
    #if IOS
        // Restore original layout when keyboard hides
        rootGrid.Padding = new Thickness(0);
    #endif
    }

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            await AnimateElementScaleDown(imBack);

            await Back();
        });
    }
}