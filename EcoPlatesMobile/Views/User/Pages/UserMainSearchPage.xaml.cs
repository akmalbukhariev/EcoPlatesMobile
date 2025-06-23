 using EcoPlatesMobile.ViewModels.User;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserMainSearchPage : BasePage
{
    private UserMainSearchPageViewModel viewModel;
	public UserMainSearchPage()
	{
		InitializeComponent();

        viewModel = new UserMainSearchPageViewModel();
        BindingContext = viewModel;
    }

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(sender as Image);
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

        viewModel.ShowProductResult = true;
        viewModel.ShowFilterSearchList = false;
        viewModel.ShowRecentSearchList = false;

        viewModel.ExecuteSearch();
        await viewModel.LoadInitialProductAsync();
    }
}