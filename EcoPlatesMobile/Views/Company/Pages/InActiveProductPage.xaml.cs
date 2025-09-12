using EcoPlatesMobile.Models.Requests.Company;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.ViewModels.Company;
using ViewExt = Microsoft.Maui.Controls.ViewExtensions;

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

        bool isWifiOn = await appControl.CheckWifi();
        if (!isWifiOn) return;

        await viewModel.LoadInitialAsync();
    }

    private async void DeleteProduct_Invoked(object sender, EventArgs e)
    {
        bool isWifiOn = await appControl.CheckWifi();
        if (!isWifiOn) return;

        if (sender is SwipeItem swipeItem &&
            swipeItem.Parent is SwipeItems swipeItems &&
            swipeItems.Parent is SwipeView swipeView &&
            swipeView.BindingContext is ProductModel product)
        {
            bool answer = await AlertService.ShowConfirmationAsync(
                                AppResource.Confirm,
                                AppResource.MessageConfirm,
                                AppResource.Yes, AppResource.No);
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
        bool isWifiOn = await appControl.CheckWifi();
        if (!isWifiOn) return;

        if (sender is SwipeItem swipeItem &&
            swipeItem.Parent is SwipeItems swipeItems &&
            swipeItems.Parent is SwipeView swipeView &&
            swipeView.BindingContext is ProductModel product)
        {
            bool answer = await AlertService.ShowConfirmationAsync(
                                AppResource.Confirm,
                                AppResource.MessageConfirm,
                                AppResource.Yes, AppResource.No);

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

    private async void ActiveAll_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            await AnimateElementScaleDown(sender as Image);

            bool answer = await AlertService.ShowConfirmationAsync(
                                    AppResource.Confirm,
                                    AppResource.MessageActiveAllProducts,
                                    AppResource.Yes, AppResource.No);

            if (!answer) return;

            try
            {
                viewModel.IsLoading = true;

                var selected = viewModel.Products.Where(p => p.IsCheckedProduct).ToList();

                var request = new ChangePosterDeletionListRequest
                {
                    dataList = selected.Select(p => new ChangePosterDeletionRequest
                    {
                        poster_id = p.PromotionId,
                        deleted = false
                    }).ToList()
                };

                Response response = await companyApiService.ChangePosterDeletionStatusList(request);
                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        foreach (var item in selected)
                            viewModel.Products.Remove(item);
                    });

                    appControl.RefreshCompanyProfilePage = true;
                    StCheckProductTapped(null, null);
                    if (viewModel.Products.Count == 0)
                        viewModel.IsShowChekProduct = false;

                    //await AlertService.ShowAlertAsync(AppResource.InactiveProducts, AppResource.Success);
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
        });
    }

    private async void DeleteAll_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            await AnimateElementScaleDown(sender as Image);

            bool answer = await AlertService.ShowConfirmationAsync(
                                    AppResource.Confirm,
                                    AppResource.MessageDeleteAllProducts,
                                    AppResource.Yes, AppResource.No);

            if (!answer) return;

            try
            {
                viewModel.IsLoading = true;

                var selected = viewModel.Products.Where(p => p.IsCheckedProduct).ToList();

                var request = new ChangePosterDeletionListRequest
                {
                    dataList = selected.Select(p => new ChangePosterDeletionRequest
                    {
                        poster_id = p.PromotionId,
                        deleted = false
                    }).ToList()
                };

                Response response = await companyApiService.DeletePosterList(request);
                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        foreach (var item in selected)
                            viewModel.Products.Remove(item);
                    });

                    appControl.RefreshCompanyProfilePage = true;
                    StCheckProductTapped(null, null);
                    if (viewModel.Products.Count == 0)
                        viewModel.IsShowChekProduct = false;

                    //await AlertService.ShowAlertAsync(AppResource.InactiveProducts, AppResource.Success);
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
        });
    }

    private async void StCheckProductTapped(object sender, TappedEventArgs e)
    {
        checkProduct.IsChecked = !checkProduct.IsChecked;
        viewModel.ShowCheckProduct(checkProduct.IsChecked);

        viewModel.checkAllCheckedAlready = true;
        viewModel.IsCheckedAllProduct = false;
        viewModel.checkAllCheckedAlready = false;

        await AnimateSelectAllBarAsync(selectAllBar, checkProduct.IsChecked);
    }
    
    private void StSelectAllProductTapped(object sender, TappedEventArgs e)
    {
        viewModel.checkAllCheckedAlready = true;
        checkAllProducts.IsChecked = !checkAllProducts.IsChecked;
        CheckAllProducts(checkAllProducts.IsChecked);
        viewModel.checkAllCheckedAlready = false;
    }

    private async void CheckProduct_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        viewModel.StackBottomEnabled = false;
        viewModel.ShowCheckProduct(checkProduct.IsChecked);
        if (checkProduct.IsChecked && currentlyOpenSwipeView != null)
        {
            currentlyOpenSwipeView.Close();
        }

        await AnimateStackBottomAsync(show: e.Value);
        viewModel.IsShowChekAllProducts = e.Value;

        viewModel.checkAllCheckedAlready = true;
        viewModel.IsCheckedAllProduct = false;
        viewModel.checkAllCheckedAlready = false; 
        
        await AnimateSelectAllBarAsync(selectAllBar, checkProduct.IsChecked);
    }

    private void CheckAllProduct_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (viewModel.checkAllCheckedAlready) return;

        CheckAllProducts(checkAllProducts.IsChecked);
    }

    private void CheckAllProducts(bool check)
    {
        if (viewModel?.Products == null) return;

        bool anyChecked = false;

        foreach (var product in viewModel.Products)
        {
            product.IsCheckedProduct = check;
            product.IsNonActiveProduct = !check;

            if (product.IsCheckedProduct) anyChecked = true;
        }

        viewModel.StackBottomEnabled = anyChecked;
        if (anyChecked)
        {
            viewModel.ActiveImage = "active.png";
            viewModel.DeleteImage = "delete.png";
        }
        else
        { 
            viewModel.ActiveImage = "active_gray.png";
            viewModel.DeleteImage = "delete_gray.png";
        }
    } 

    bool isAnimating;
    async Task AnimateStackBottomAsync(bool show)
    {
        if (isAnimating) return;
        isAnimating = true;

        const uint duration = 220;
        var easing = Easing.SinOut;
 
        var off = stackBottom.Width > 0 ? Math.Max(60, stackBottom.Width + 16) : 60;

        if (show)
        {
            stackBottom.IsVisible = true;
            stackBottom.InputTransparent = true; 
 
            stackBottom.TranslationX = off;
            stackBottom.Opacity = 0;

            await Task.WhenAll(
                stackBottom.TranslateTo(0, 0, duration, easing),
                stackBottom.FadeTo(1, duration, easing)
            );

            stackBottom.InputTransparent = false;
        }
        else
        {
            stackBottom.InputTransparent = true;

            await Task.WhenAll(
                stackBottom.TranslateTo(off, 0, duration, easing),
                stackBottom.FadeTo(0, duration, easing)
            );

            stackBottom.IsVisible = false;
        }

        isAnimating = false;
    }

    private SwipeView currentlyOpenSwipeView;
    void OnRowSwipe(object sender, EventArgs e)
    {
        var swipeView = (SwipeView)sender;

        if (currentlyOpenSwipeView != null && currentlyOpenSwipeView != swipeView)
        {
            currentlyOpenSwipeView.Close();
        }

        if (!viewModel.AllowSwipe)
            ((SwipeView)sender).Close();

        currentlyOpenSwipeView = swipeView;
    }
}