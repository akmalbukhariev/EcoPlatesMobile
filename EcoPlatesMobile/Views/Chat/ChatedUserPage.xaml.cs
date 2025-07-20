using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.ViewModels.Chat;

namespace EcoPlatesMobile.Views.Chat;

public partial class ChatedUserPage : BasePage
{
	private ChatedUserPageViewModel viewModel;
    private UserSessionService userSessionService;
	public ChatedUserPage(ChatedUserPageViewModel viewModel, UserSessionService userSessionService)
    {
        InitializeComponent();

        this.viewModel = viewModel;
        this.userSessionService = userSessionService;

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
            await viewModel.LoadCompaniesData();

        }
        else
        {
            color = Constants.COLOR_COMPANY;
            listProduct.RefreshColor = color;
            await viewModel.LoadUsersData();
        }
        
        AppService.Get<IStatusBarService>().SetStatusBarColor(color.ToArgbHex(), false);
    }

    private async void User_Tapped(object sender, TappedEventArgs e)
    {
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