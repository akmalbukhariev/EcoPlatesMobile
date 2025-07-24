using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.ViewModels.User;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserMainSearchPage : BasePage
{
    private UserMainSearchPageViewModel viewModel;
    private AppControl appControl;
	public UserMainSearchPage(UserMainSearchPageViewModel vm, AppControl appControl)
    {
        InitializeComponent();

        this.viewModel = vm;
        this.appControl = appControl;

        BindingContext = viewModel;
    }

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(imBack);
        await AppNavigatorService.NavigateTo("..");
    }

    private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        clearButton.IsVisible = !string.IsNullOrWhiteSpace(entrySearch.Text);
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        entrySearch.Text = string.Empty;
        viewModel.ShowFilterSearchList = false;
        viewModel.ShowProductResult = false;
        viewModel.ShowRecentSearchList = true;
    }

    private async void Search_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(sender as Image);
  
        entrySearch.Unfocus();

        bool isWifiOn = await appControl.CheckWifi();
		if (!isWifiOn) return;

        viewModel.ShowProductResult = true;
        viewModel.ShowFilterSearchList = false;
        viewModel.ShowRecentSearchList = false;

        if (string.IsNullOrEmpty(viewModel.SearchText) || string.IsNullOrWhiteSpace(viewModel.SearchText))
        {
            await AlertService.ShowAlertAsync(AppResource.Failed, AppResource.MessageFieldCannotBeEmty);
            return;
        }

        viewModel.ExecuteSearch();
        await viewModel.LoadInitialProductAsync();
    }
}