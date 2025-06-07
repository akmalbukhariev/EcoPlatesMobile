using EcoPlatesMobile.Models.Requests.Company;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.ViewModels.Company;

namespace EcoPlatesMobile.Views.Company.Pages;

[QueryProperty(nameof(ShowBackQuery), nameof(ShowBackQuery))]
[QueryProperty(nameof(ShowTabBarQuery), nameof(ShowTabBarQuery))]
public partial class NonActiveProductPage : BasePage
{
    private bool ShowBack { get; set; } = false;
    private bool ShowTabBar { get; set; } = true;
    public string ShowBackQuery
    {
        set
        {
            if (bool.TryParse(value, out var result))
                ShowBack = result;
        }
    }

    public string ShowTabBarQuery
    {
        set
        {
            if (bool.TryParse(value, out var result))
                ShowTabBar = result;
        }
    }

    private NonActiveProductPageViewModel viewModel;
	public NonActiveProductPage()
	{
		InitializeComponent();
		
		viewModel = new NonActiveProductPageViewModel();
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetTabBarIsVisible(this, ShowTabBar);
        
        header.ShowBack = ShowBack;
        await viewModel.LoadInitialAsync();
    }

    private async void DeleteProduct_Invoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem &&
            swipeItem.Parent is SwipeItems swipeItems &&
            swipeItems.Parent is SwipeView swipeView &&
            swipeView.BindingContext is ProductModel product)
        {
            bool answer = await Application.Current.MainPage.DisplayAlert(
                                "Confirm",
                                "Do you want to proceed?",
                                "Yes",
                                "No"
                            );
            if (!answer) return;

            product.CompanyId = (long)AppService.Get<AppControl>().CompanyInfo.company_id;
            try
            {
                viewModel.IsLoading = true;
                var apiService = AppService.Get<CompanyApiService>();
                Response response = await apiService.DeletePoster(product.PromotionId);

                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    await AlertService.ShowAlertAsync("Delete poster", "Success.");
                    viewModel.Products.Remove(product);
                    viewModel.UpdateTitle();
                }
                else
                {
                    await AlertService.ShowAlertAsync("Error", response.resultMsg);
                }
            }
            catch (Exception ex)
            {
                await AlertService.ShowAlertAsync("Error", ex.Message);
            }
            finally
            {
                viewModel.IsLoading = false;
            }
        }
    }

    private async void ActiveProduct_Invoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem &&
            swipeItem.Parent is SwipeItems swipeItems &&
            swipeItems.Parent is SwipeView swipeView &&
            swipeView.BindingContext is ProductModel product)
        {
            bool answer = await Application.Current.MainPage.DisplayAlert(
                                "Confirm",
                                "Do you want to proceed?",
                                "Yes",
                                "No"
                            );

            if (!answer) return;

            try
            {
                viewModel.IsLoading = true;
                ChangePosterDeletionRequest request = new ChangePosterDeletionRequest()
                {
                    poster_id = product.PromotionId,
                    deleted = false
                };

                var apiService = AppService.Get<CompanyApiService>();
                Response response = await apiService.ChangePosterDeletionStatus(request);
                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    viewModel.Products.Remove(product);
                    viewModel.UpdateTitle();
                }
                else
                {
                    await AlertService.ShowAlertAsync("Error", response.resultMsg);
                }
            }
            catch (Exception ex)
            {
                await AlertService.ShowAlertAsync("Error", ex.Message);
            }
            finally
            {
                viewModel.IsLoading = false;
            }
        }
    }
}