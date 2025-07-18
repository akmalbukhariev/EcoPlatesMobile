using EcoPlatesMobile.ViewModels.Chat;

namespace EcoPlatesMobile.Views.Chat;

public partial class ChatedUserPage : BasePage
{
	private ChatedUserPageViewModel viewModel;
	public ChatedUserPage(ChatedUserPageViewModel viewModel)
	{
		InitializeComponent();
		
		this.viewModel = viewModel;

		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
		await viewModel.LoadData();
    }

    private async void User_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Grid grid && grid.BindingContext is SenderIdInfo tappedItem)
        {
            await AnimateElementScaleDown(grid);

            //string name = tappedItem.UserName;
            //string image = tappedItem.UserImage;

            await Shell.Current.GoToAsync(nameof(ChattingPage), new Dictionary<string, object>
            {
                ["ChatPageModel"] = tappedItem.chatPageModel
            });
        }
    }
}