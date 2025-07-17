using EcoPlatesMobile.Models.Requests.Company;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.ViewModels.Company;

namespace EcoPlatesMobile.Views.Company.Pages;

[QueryProperty(nameof(ShowBackQuery), nameof(ShowBackQuery))]
[QueryProperty(nameof(ShowTabBarQuery), nameof(ShowTabBarQuery))]
public partial class InActiveProductPage : BasePage
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

    private InActiveProductPageViewModel viewModel;
    private AppControl appControl;
    private CompanyApiService companyApiService;

    public InActiveProductPage(InActiveProductPageViewModel vm, AppControl appControl, CompanyApiService companyApiService)
	{
		InitializeComponent();

        this.viewModel = vm;
        this.appControl = appControl;
        this.companyApiService = companyApiService;

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
            bool answer = await AlertService.ShowConfirmationAsync(
                                AppResource.Confirm,
                                AppResource.MessageConfirm,
                                AppResource.Yes,AppResource.No);
            if (!answer) return;

            product.CompanyId = (long)appControl.CompanyInfo.company_id;
            try
            {
                viewModel.IsLoading = true; 
                Response response = await companyApiService.DeletePoster(product.PromotionId);

                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    await AlertService.ShowAlertAsync(AppResource.DeleteProduct, AppResource.Success);
                    viewModel.Products.Remove(product);
                    viewModel.UpdateTitle();

                    appControl.RefreshCompanyProfilePage = true;
                }
                else
                {
                    await AlertService.ShowAlertAsync(AppResource.Error, response.resultMsg);
                }
            }
            catch (Exception ex)
            {
                await AlertService.ShowAlertAsync(AppResource.Error, ex.Message);
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
            bool answer = await AlertService.ShowConfirmationAsync(
                                AppResource.Confirm,
                                AppResource.MessageConfirm,
                                AppResource.Yes,AppResource.No);

            if (!answer) return;

            try
            {
                viewModel.IsLoading = true;
                ChangePosterDeletionRequest request = new ChangePosterDeletionRequest()
                {
                    poster_id = product.PromotionId,
                    deleted = false
                };
 
                Response response = await companyApiService.ChangePosterDeletionStatus(request);
                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    viewModel.Products.Remove(product);
                    viewModel.UpdateTitle();
                    appControl.RefreshCompanyProfilePage = true;
                }
                else
                {
                    await AlertService.ShowAlertAsync(AppResource.Error, response.resultMsg);
                }
            }
            catch (Exception ex)
            {
                await AlertService.ShowAlertAsync(AppResource.Error, ex.Message);
            }
            finally
            {
                viewModel.IsLoading = false;
            }
        }
    }
}