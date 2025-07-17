using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcoPlatesMobile.Models.Requests;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Views.Company.Pages;

namespace EcoPlatesMobile.ViewModels.Company
{ 
    public partial class ActiveProductPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableRangeCollection<ProductModel> products;
        [ObservableProperty] private ProductModel selectedProduct;
        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private bool isRefreshing;
        [ObservableProperty] private bool showAddButton;
        [ObservableProperty] private string activeProductCount;

        private int offset = 0;
        private const int PageSize = 4;
        private bool hasMoreItems = true;

        private CompanyApiService companyApiService;
        private AppControl appControl;

        public ActiveProductPageViewModel(CompanyApiService companyApiService, AppControl appControl)
        {
            this.companyApiService = companyApiService;
            this.appControl = appControl;

            products = new ObservableRangeCollection<ProductModel>();

            ClickProductCommand = new Command<ProductModel>(ProductClicked);
        }

        private async void ProductClicked(ProductModel product)
        {
            product.CompanyId = appControl.CompanyInfo.company_id;
            await Shell.Current.GoToAsync(nameof(CompanyEditProductPage), new Dictionary<string, object>
            {
                ["ProductModel"] = product
            });
        }

        public async Task LoadInitialAsync()
        {
            ShowAddButton = false;

            offset = 0;
            hasMoreItems = true;
            Products.Clear();

            try
            {
                IsLoading = true;
 
                PaginationWithDeletedParam request = new PaginationWithDeletedParam
                {
                    deleted = false,
                    offset = offset,
                    pageSize = PageSize  
                };

                PosterListResponse response = await companyApiService.GetCompanyPoster(request);

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
                        ProductImage = string.IsNullOrWhiteSpace(item.image_url) ? "no_image.png" : item.image_url,
                        //Count = "2 qoldi",
                        ProductName = item.title,
                        ProductMakerName = item.company_name,
                        NewPrice = $"{item.new_price:N0} so'm",
                        OldPrice = $"{item.old_price:N0} so'm",
                        NewPriceDigit = item.new_price ?? 0,
                        OldPriceDigit = item.old_price ?? 0,
                        description = item.description,
                        Stars = item.avg_rating.ToString(),
                        Liked = item.liked,
                        BookmarkId = item.bookmark_id ?? 0,
                        Distance = $"{item.distance_km:0.0} km"
                    }).ToList();
                    Products.AddRange(productModels);
 
                    UpdateTitle();

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
                IsLoading = false;
                ShowAddButton = true;
            }
        }

        public async Task LoadPromotionAsync(bool isRefresh = false)
        {
            if (IsLoading || (!hasMoreItems && !isRefresh))
                return;

            ShowAddButton = false;

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
 
                PaginationWithDeletedParam request = new PaginationWithDeletedParam
                {
                    deleted = false,
                    offset = offset,
                    pageSize = PageSize  
                };

                PosterListResponse response = await companyApiService.GetCompanyPoster(request);

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
                        ProductImage = string.IsNullOrWhiteSpace(item.image_url) ? "no_image.png" : item.image_url,
                        //Count = "2 qoldi",
                        ProductName = item.title,
                        ProductMakerName = item.company_name,
                        NewPrice = $"{item.new_price:N0} so'm",
                        OldPrice = $"{item.old_price:N0} so'm",
                        NewPriceDigit = item.new_price ?? 0,
                        OldPriceDigit = item.old_price ?? 0,
                        description = item.description,
                        Stars = item.avg_rating.ToString(),
                        Liked = item.liked,
                        BookmarkId = item.bookmark_id ?? 0,
                        Distance = $"{item.distance_km:0.0} km"
                    }).ToList();

                    Products.AddRange(productModels);

                    UpdateTitle();

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
                IsRefreshing = false;
                IsLoading = false;
                ShowAddButton = true;
            }
        }

        public void UpdateTitle()
        {
            ActiveProductCount = AppResource.ActiveProductCount + " " + Products.Count;
        }

        public ICommand ClickProductCommand { get; }

        public IRelayCommand LoadMoreCommand => new RelayCommand(async () =>
        {
            if (IsLoading || IsRefreshing || !hasMoreItems)
                return;

            await LoadPromotionAsync();
        });

        public IRelayCommand RefreshCommand => new RelayCommand(async () =>
        {
            await LoadPromotionAsync(isRefresh: true);
        });
    }
}