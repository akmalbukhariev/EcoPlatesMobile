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

    public ChatedUserPage(ChatedUserPageViewModel viewModel, UserSessionService userSessionService, AppControl appControl)
    {
        InitializeComponent();

        this.viewModel = viewModel;
        this.userSessionService = userSessionService;
        this.appControl = appControl;
        BindingContext = viewModel;

        //Loaded += Page_Loaded;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Color color = Constants.COLOR_USER;
        if (userSessionService.Role == UserRole.User)
        {
            header.HeaderBackground = color;
            listProduct.RefreshColor = color;

            bool isWifiOn = await appControl.CheckWifi();
            if (!isWifiOn) return;

            await viewModel.LoadCompaniesData();
        }
        else
        {
            color = Constants.COLOR_COMPANY;
            listProduct.RefreshColor = color;

            bool isWifiOn = await appControl.CheckWifi();
            if (!isWifiOn) return;

            await viewModel.LoadUsersData();
        }

        AppService.Get<IStatusBarService>().SetStatusBarColor(color.ToArgbHex(), false);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        //IsOpen = false;
        //MessagingCenter.Unsubscribe<ChatedUserPage>(this, Constants.SEARCH_NOTIFICATION_FOR_USER);
    }
     
    private async void User_Tapped(object sender, TappedEventArgs e)
    {
        bool isWifiOn = await appControl.CheckWifi();
        if (!isWifiOn) return;

        if (sender is Grid grid && grid.BindingContext is SenderIdInfo tappedItem)
        {
            await AnimateElementScaleDown(grid);

            await Shell.Current.GoToAsync(nameof(ChattingPage), new Dictionary<string, object>
            {
                ["ChatPageModel"] = tappedItem.chatPageModel
            });
        }
    }
}