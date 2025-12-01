using EcoPlatesMobile.Models.Responses.Notification;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.ViewModels.Chat;
using Newtonsoft.Json.Linq;

namespace EcoPlatesMobile.Views.Chat;

public partial class ChatedUserPage : BasePage
{
	private ChatedUserPageViewModel viewModel;
    private UserSessionService userSessionService;
    private AppControl appControl;
    private IStatusBarService statusBarService;
    public ChatedUserPage(ChatedUserPageViewModel viewModel, UserSessionService userSessionService, AppControl appControl, IStatusBarService statusBarService)
    {
        InitializeComponent();

        this.viewModel = viewModel;
        this.userSessionService = userSessionService;
        this.appControl = appControl;
        this.statusBarService = statusBarService;

        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Color color = Constants.COLOR_USER;
        if (userSessionService.Role == UserRole.User)
        {
            header.HeaderBackground = color;
            listProduct.RefreshColor = color;

            bool isWifiOn = await appControl.CheckWifiOrNetwork();
            if (!isWifiOn) return;

            isWifiOn = await appControl.CheckWifiOrNetworkFor(true);
            if (!isWifiOn) return;

            await viewModel.LoadCompaniesData();
        }
        else
        {
            color = Constants.COLOR_COMPANY;
            listProduct.RefreshColor = color;

            bool isWifiOn = await appControl.CheckWifiOrNetwork();
            if (!isWifiOn) return;

            isWifiOn = await appControl.CheckWifiOrNetworkFor(true);
            if (!isWifiOn) return;

            await viewModel.LoadUsersData();
        }

        statusBarService.SetStatusBarColor(color.ToArgbHex(), false);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
     
    private async void User_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            bool isWifiOn = await appControl.CheckWifiOrNetwork();
            if (!isWifiOn) return;

            if (sender is Grid grid && grid.BindingContext is SenderIdInfo tappedItem)
            {
                await AnimateElementScaleDown(grid);

                await Shell.Current.GoToAsync(nameof(ChattingPage), new Dictionary<string, object>
                {
                    ["ChatPageModel"] = tappedItem.chatPageModel
                });
            }
        });
    }
}