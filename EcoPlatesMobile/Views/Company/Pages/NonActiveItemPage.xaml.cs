using EcoPlatesMobile.Models.Requests.Company;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.ViewModels.Company;

namespace EcoPlatesMobile.Views.Company.Pages;

public partial class NonActiveItemPage : ContentPage
{
	private NonActiveProductPageViewModel viewModel;
	public NonActiveItemPage()
	{
		InitializeComponent();
		
		viewModel = new NonActiveProductPageViewModel();
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await viewModel.LoadInitialAsync();
    }

    private async void EditProduct_Invoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem &&
            swipeItem.Parent is SwipeItems swipeItems &&
            swipeItems.Parent is SwipeView swipeView &&
            swipeView.BindingContext is ProductModel product)
        {
            product.CompanyId = 11;
            await Shell.Current.GoToAsync(nameof(CompanyAddProductPage), new Dictionary<string, object>
            {
                ["ProductModel"] = product
            });
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