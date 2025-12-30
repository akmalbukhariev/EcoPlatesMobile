
using System.Collections.ObjectModel;
using System.Diagnostics;
using EcoPlatesMobile.Models.Requests.User;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.ViewModels.User;
using EcoPlatesMobile.Views.User.Pages;

namespace EcoPlatesMobile.Views.User.Components;

public partial class SimilarProductsView : ContentView
{
    public ObservableRangeCollection<ProductModel> Products { get; } = new();
     
    private int offset = 0;
    private const int PageSize = 3;
    private bool hasMoreItems = true;
    private bool _pendingLoadMore;
    private bool _isLoadingMore;

    public string Category = PosterType.FOOD_GENERAL.GetValue();
    
    private UserApiService userApiService;
    private AppControl appControl;
    public SimilarProductsView()
    {
        InitializeComponent();

        BindingContext = this;
    }

    public void SetService(UserApiService userApiService, AppControl appControl)
    { 
        this.userApiService = userApiService;
        this.appControl = appControl;
    }

    private async void OnRemainingItemsThresholdReached(object? sender, EventArgs e)
    {
        if (!hasMoreItems) return;

        if (loadingView.IsRunning)
        {
            _pendingLoadMore = true;
            return;
        }
        
        await LoadMoreAsync();
    }
    
    public async Task LoadInitialAsync()
    {
        offset = 0;
        hasMoreItems = true;
        await MainThread.InvokeOnMainThreadAsync(() => Products.Clear());

        try
        {
            loadingView.IsRunning = true;

            PosterLocationRequest request = new PosterLocationRequest
            {
                poster_type = Category,
                offset = offset,
                pageSize = PageSize,
                radius_km = appControl.IsLoggedIn ? appControl.UserInfo.radius_km : Constants.MaxRadius,
                user_lat = appControl.UserInfo.location_latitude,
                user_lon = appControl.UserInfo.location_longitude,
            };

            PosterListResponse response = appControl.IsLoggedIn ? await userApiService.GetPostersByCurrentLocationAndPosterType(request) :
                                                                  await userApiService.GetPostersByCurrentLocationAndPosterTypeWithoutLogin(request);
            bool isOk = await appControl.CheckUserState(response);
            if (!isOk)
            {
                await appControl.LogoutUser();
                return;
            }

            if (response.resultCode == ApiResult.POSTER_EXIST.GetCodeToString())
            {
                var items = response.resultData;

                if (items == null || items.Count == 0)
                {
                    hasMoreItems = false;
                    return;
                }

                var productModels = items.Select(item => new ProductModel
                {
                    PromotionId = item.poster_id ?? 0,
                    ProductImage = appControl.GetImageUrlOrFallback(item.image_url),
                    Category = item.category,
                    Count = item.total_reviews.ToString(),
                    ProductName = item.title,
                    ProductMakerName = item.company_name,
                    NewPrice = appControl.GetUzbCurrency(item.new_price),
                    OldPrice = appControl.GetUzbCurrency(item.old_price),
                    Stars = item.avg_rating.ToString(),
                    Liked = item.liked,
                    BookmarkId = item.bookmark_id ?? 0,
                    Distance = $"{item.distance_km:0.0} km"
                }).ToList();

                if (productModels.Count <= 0)
                {
                    boxLineView.IsVisible = false;
                    grdAll.IsVisible = false;
                }

                Products.AddRange(productModels);

                offset += PageSize;
                if (productModels.Count < PageSize)
                {
                    hasMoreItems = false;
                }
            }
            else
            {
                hasMoreItems = false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ERROR] LoadInitialAsync: {ex.Message}");
        }
        finally
        {
            loadingView.IsRunning = false;
            
            if (_pendingLoadMore)
            {
                _pendingLoadMore = false;
                await LoadMoreAsync();
            }
        }
    }

    public async Task LoadPromotionAsync()
    {
        if (!hasMoreItems)
            return;

        try
        {
            loadingView.IsRunning = true; 

            PosterLocationRequest request = new PosterLocationRequest
            {
                poster_type = Category,
                offset = offset,
                pageSize = PageSize,
                radius_km = appControl.IsLoggedIn ? appControl.UserInfo.radius_km : Constants.MaxRadius,
                user_lat = appControl.UserInfo.location_latitude,
                user_lon = appControl.UserInfo.location_longitude
            };

            PosterListResponse response = appControl.IsLoggedIn ? await userApiService.GetPostersByCurrentLocationAndPosterType(request) :
                                                                  await userApiService.GetPostersByCurrentLocationAndPosterTypeWithoutLogin(request);
            bool isOk = await appControl.CheckUserState(response);
            if (!isOk)
            {
                await appControl.LogoutUser();
                return;
            }
            
            if (response.resultCode == ApiResult.POSTER_EXIST.GetCodeToString())
            {
                var items = response.resultData;

                if (items == null || items.Count == 0)
                {
                    hasMoreItems = false;
                    return;
                }

                var productModels = items.Select(item => new ProductModel
                {
                    PromotionId = item.poster_id ?? 0,
                    ProductImage = appControl.GetImageUrlOrFallback(item.image_url),
                    Count = item.total_reviews.ToString(),
                    Category = item.category,
                    ProductName = item.title,
                    ProductMakerName = item.company_name,
                    NewPrice = appControl.GetUzbCurrency(item.new_price),
                    OldPrice = appControl.GetUzbCurrency(item.old_price),
                    Stars = item.avg_rating.ToString(),
                    Liked = item.liked,
                    BookmarkId = item.bookmark_id ?? 0,
                    Distance = $"{item.distance_km:0.0} km"
                }).ToList();

                Products.AddRange(productModels);

                offset += PageSize;
                if (productModels.Count < PageSize)
                {
                    hasMoreItems = false;
                }
            }
            else
            {
                hasMoreItems = false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ERROR] LoadPromotionAsync: {ex.Message}");
        }
        finally
        {
            loadingView.IsRunning = false;
        }
    }

    private async Task LoadMoreAsync()
    {
        if (_isLoadingMore || !hasMoreItems)
            return;

        _isLoadingMore = true;

        try
        {
            bool isWifiOn = await appControl.CheckWifiOrNetwork();
            if (!isWifiOn)
                return;

            await LoadPromotionAsync();
        }
        finally
        {
            _isLoadingMore = false;
        }
    }
    
    private async void All_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            await brdAll.ScaleTo(0.90, 100, Easing.CubicOut);
            await brdAll.ScaleTo(1.0, 100, Easing.CubicIn);
            
            await AppNavigatorService.NavigateTo($"{nameof(SimilarProductsPage)}?Category={Category}");
        });
    }
    
    private async void Product_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            if (sender is Frame frame && frame.BindingContext is ProductModel product)
            {
                await frame.ScaleTo(0.95, 100, Easing.CubicOut);
                await frame.ScaleTo(1.0, 100, Easing.CubicIn);
                
                await Shell.Current.GoToAsync(nameof(DetailProductPage), new Dictionary<string, object>
                {
                    ["ProductModel"] = product
                });
            }
        });
    }
}