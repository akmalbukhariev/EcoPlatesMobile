using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.ViewModels.User;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserMainSearchPage : BasePage
{
    private UserMainSearchPageViewModel viewModel;
    private AppControl appControl;
    private IKeyboardHelper keyboardHelper;
    public UserMainSearchPage(UserMainSearchPageViewModel vm, AppControl appControl, IKeyboardHelper keyboardHelper)
    {
        InitializeComponent();

        this.viewModel = vm;
        this.appControl = appControl;
        this.keyboardHelper = keyboardHelper;

        BindingContext = viewModel;

        entrySearch.Completed += Entry_Completed;

        this.Loaded += (s, e) =>
        {
            entrySearch.Focus();
        };
    }
    
    private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        clearButton.IsVisible = !string.IsNullOrWhiteSpace(entrySearch.Text);
    }

    private async void OnClearClicked(object sender, EventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            entrySearch.Text = string.Empty;
            viewModel.ShowFilterSearchList = false;
            viewModel.ShowProductResult = false;
            viewModel.ShowRecentSearchList = true;
        });
    }

    private void Entry_Completed(object sender, EventArgs e)
    {
        Search_Tapped(imSearch, null);
    }

    private async void Search_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            keyboardHelper.HideKeyboard();
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
        });
    }

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            await AnimateElementScaleDown(imBack);
            await AppNavigatorService.NavigateTo("..");
        });
    }
}