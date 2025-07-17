using EcoPlatesMobile.ViewModels.Chat;

namespace EcoPlatesMobile.Views.Chat;

public partial class ChatedUserPage : BasePage
{
	private ChatedUserPageViewModel viewModel;
	public ChatedUserPage(ChatedUserPageViewModel viewModel)
	{
		InitializeComponent();
		
		this.viewModel = viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}