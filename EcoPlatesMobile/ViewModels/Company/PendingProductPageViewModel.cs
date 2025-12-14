using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EcoPlatesMobile.Models.Requests;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Views.Company.Pages;
using EcoPlatesMobile.Views.Company.Components;

namespace EcoPlatesMobile.ViewModels.Company
{
    public partial class PendingProductPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableRangeCollection<ProductModel> products;
        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private bool isRefreshing;
        [ObservableProperty] private bool isShowTitle;

        private int offset = 0;
        private const int PageSize = 4;
        private bool hasMoreItems = true;

        private CompanyApiService companyApiService;
        private AppControl appControl;

        public PendingProductPageViewModel(CompanyApiService companyApiService, AppControl appControl)
        {
            this.companyApiService = companyApiService;
            this.appControl = appControl;

            products = new ObservableRangeCollection<ProductModel>();
            LoadMoreCommand = new AsyncRelayCommand(LoadMoreAsync);
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        }

        public async Task LoadInitialAsync()
        {
            offset = 0;
            hasMoreItems = true;
            Products.Clear();

            try
            {
                IsLoading = true;

                PaginationWithCompanyIdRequest request = new PaginationWithCompanyIdRequest
                {
                    company_id = appControl.CompanyInfo.company_id,
                    offset = offset,
                    pageSize = PageSize
                };

                PosterListResponse response = await companyApiService.GetPendingPosters(request);

                bool isOk = await appControl.CheckUserState(response);
                if (!isOk)
                {
                    await appControl.LogoutCompany();
                    return;
                }

                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    var items = response.resultData;

                    if (items == null || items.Count == 0)
                    {
                        hasMoreItems = false;
                        IsShowTitle = Products.Count > 0;
                        return;
                    }

                    var productModels = items.Select(item => new ProductModel
                    {
                        PromotionId = item.poster_id ?? 0,
                        ProductImage = appControl.GetImageUrlOrFallback(item.image_url),
                        ProductName = item.title,
                        ProductMakerName = item.company_name,
                        NewPrice = appControl.GetUzbCurrency(item.new_price),
                        OldPrice = appControl.GetUzbCurrency(item.old_price),
                        NewPriceDigit = item.new_price ?? 0,
                        OldPriceDigit = item.old_price ?? 0,
                        description = item.description,
                        Stars = item.avg_rating.ToString(),
                        Liked = item.liked,
                        BookmarkId = item.bookmark_id ?? 0,
                        Distance = $"{item.distance_km:0.0} km"
                    }).ToList();

                    ProductView.BeginNewAnimationCycle();
                    Products.AddRange(productModels);

                    offset += PageSize;
                    if (productModels.Count < PageSize)
                    {
                        hasMoreItems = false;
                    }
                
                    IsShowTitle = Products.Count > 0;
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
                IsLoading = false;
            }
        }

        public async Task LoadPromotionAsync(bool isRefresh = false)
        {
            if (IsLoading || (!hasMoreItems && !isRefresh))
                return;
 
            try
            {
                if (isRefresh)
                {
                    IsRefreshing = true;
                    offset = 0;
                    hasMoreItems = true;
                    Products.Clear();
                }
                else
                {
                    IsLoading = true;
                }
                 
                PaginationWithCompanyIdRequest request = new PaginationWithCompanyIdRequest
                {
                    company_id = appControl.CompanyInfo.company_id,
                    offset = offset,
                    pageSize = PageSize
                };

                PosterListResponse response = await companyApiService.GetPendingPosters(request);

                bool isOk = await appControl.CheckUserState(response);
                if (!isOk)
                {
                    await appControl.LogoutCompany();
                    return;
                }

                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    var items = response.resultData;

                    if (items == null || items.Count == 0)
                    {
                        hasMoreItems = false;
                        IsShowTitle = Products.Count > 0;
                        return;
                    }

                    var productModels = items.Select(item => new ProductModel
                    {
                        PromotionId = item.poster_id ?? 0,
                        ProductImage = appControl.GetImageUrlOrFallback(item.image_url),
                        //Count = "2 qoldi",
                        ProductName = item.title,
                        ProductMakerName = item.company_name,
                        NewPrice = appControl.GetUzbCurrency(item.new_price),
                        OldPrice = appControl.GetUzbCurrency(item.old_price),
                        NewPriceDigit = item.new_price ?? 0,
                        OldPriceDigit = item.old_price ?? 0,
                        description = item.description,
                        Stars = item.avg_rating.ToString(),
                        Liked = item.liked,
                        BookmarkId = item.bookmark_id ?? 0,
                        Distance = $"{item.distance_km:0.0} km"
                    }).ToList();

                    ProductView.BeginNewAnimationCycle();
                    Products.AddRange(productModels);

                    offset += PageSize;
                    if (productModels.Count < PageSize)
                    {
                        hasMoreItems = false;
                    }

                    IsShowTitle = Products.Count > 0;
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
                IsRefreshing = false;
                IsLoading = false;
            }
        }
        
        public ICommand ClickProductCommand { get; }

        public IAsyncRelayCommand LoadMoreCommand { get; }
        public IAsyncRelayCommand RefreshCommand { get; }

        private async Task LoadMoreAsync()
        {
            bool isWifiOn = await appControl.CheckWifiOrNetwork();
            if (!isWifiOn) return;

            if (IsLoading || IsRefreshing || !hasMoreItems)
                return;

            await LoadPromotionAsync();
        }
        
        private async Task RefreshAsync()
        {
            bool isWifiOn = await appControl.CheckWifiOrNetwork();
            if (!isWifiOn)
            {
                IsRefreshing = false;
                IsLoading = false;
                return;
            }

            await LoadPromotionAsync(isRefresh: true);
        }
    }
}