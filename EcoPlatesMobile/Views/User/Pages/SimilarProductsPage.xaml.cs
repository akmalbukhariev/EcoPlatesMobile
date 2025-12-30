using System.ComponentModel;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.ViewModels.User;


namespace EcoPlatesMobile.Views.User.Pages;

[QueryProperty(nameof(Category), nameof(Category))]
public partial class SimilarProductsPage : BasePage
{
    public string Category
    {
        set
        {
            viewModel.Category = value;
        }
    }

    private SimilarProductsPageViewModel viewModel;
    private AppControl appControl;

    public SimilarProductsPage()
    {
        InitializeComponent();

        viewModel = AppService.Get<SimilarProductsPageViewModel>();
        appControl = AppService.Get<AppControl>();

        viewModel.PropertyChanged += ViewModel_PropertyChanged;

        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await viewModel.LoadInitialAsync();
    }

    private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(viewModel.ShowLikedView) && viewModel.ShowLikedView)
        {
            await likeView.DisplayAsAnimation();
            viewModel.ShowLikedView = false;
        }
    }
}