using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.ViewModels.User;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserBrowserSearchPage : BasePage
{
    private UserBrowserSearchPageViewModel viewModel;
    private AppControl appControl;
    private IKeyboardHelper keyboardHelper;
    private bool backHasClicked = false;
    public UserBrowserSearchPage(UserBrowserSearchPageViewModel vm, AppControl appControl, IKeyboardHelper keyboardHelper)
    {
        InitializeComponent();

        this.viewModel = vm;
        this.appControl = appControl;
        this.keyboardHelper = keyboardHelper;

        BindingContext = viewModel;

        //entrySearch.Completed += Entry_Completed;

        Loaded += (s, e) =>
        {
#if IOS
            // On iOS, use Unfocused as "Completed"
            entrySearch.Unfocused += Entry_Completed;
#else
            // On Android (and others), normal Completed works
            entrySearch.Completed += Entry_Completed;
#endif

            entrySearch.Focus();
        };
    }

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            backHasClicked = true;
            
            await AnimateElementScaleDown(imBack);
            await AppNavigatorService.NavigateTo("..");
        });
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
            viewModel.ShowCompanyResult = false;
            viewModel.ShowRecentSearchList = true;
        });
    }

    private void Entry_Completed(object sender, EventArgs e)
    {
        if (backHasClicked) return;

        Search_Tapped(imSearch, null); 
    }

    private async void Search_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            keyboardHelper.HideKeyboard();

            await AnimateElementScaleDown(sender as Image);

            bool isWifiOn = await appControl.CheckWifiOrNetwork();
            if (!isWifiOn) return;
 
            if (string.IsNullOrEmpty(viewModel.SearchText) || string.IsNullOrWhiteSpace(viewModel.SearchText))
            {
                await AlertService.ShowAlertAsync(AppResource.Failed, AppResource.MessageFieldCannotBeEmty);
                return;
            }

            viewModel.ShowCompanyResult = true;
            viewModel.ShowFilterSearchList = false;
            viewModel.ShowRecentSearchList = false;

            viewModel.ExecuteSearch();
            await viewModel.LoadInitialCompanyAsync();
        });
    }
}