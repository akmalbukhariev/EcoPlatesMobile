using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.ViewModels.User;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserBrowserSearchPage : BasePage
{
    private UserBrowserSearchPageViewModel viewModel;
	public UserBrowserSearchPage(UserBrowserSearchPageViewModel vm)
	{
		InitializeComponent();

        this.viewModel = vm;
        
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
        viewModel.ShowCompanyResult = false;
        viewModel.ShowRecentSearchList = true;
    }

    private async void Search_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(sender as Image);
 
        viewModel.ShowCompanyResult = true;
        viewModel.ShowFilterSearchList = false;
        viewModel.ShowRecentSearchList = false;

        if (string.IsNullOrEmpty(viewModel.SearchText) || string.IsNullOrWhiteSpace(viewModel.SearchText))
        {
            await AlertService.ShowAlertAsync(AppResource.Failed, AppResource.MessageFieldCannotBeEmty);
            return;
        }

        viewModel.ExecuteSearch();
        await viewModel.LoadInitialCompanyAsync();
    }
}