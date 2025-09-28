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
using EcoPlatesMobile.Views.Company.Components;

namespace EcoPlatesMobile.ViewModels.Company
{
    public partial class InActiveProductPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableRangeCollection<ProductModel> products;
        [ObservableProperty] private ProductModel selectedProduct;
        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private bool isRefreshing;
        [ObservableProperty] private bool showAddButton;
        [ObservableProperty] private string inActiveProductCount;
        [ObservableProperty] private bool stackBottomEnabled = false;
        [ObservableProperty] private bool isCheckedProduct = false;
        [ObservableProperty] private bool isCheckedAllProduct = false;
        [ObservableProperty] private bool isShowChekProduct;
        [ObservableProperty] private bool isShowChekAllProducts;
        [ObservableProperty] private bool allowSwipe = true;
        [ObservableProperty] private string activeImage = "active_gray.png";
        [ObservableProperty] private string deleteImage = "delete_gray.png";

        private int offset = 0;
        private const int PageSize = 4;
        private bool hasMoreItems = true;
        public bool checkAllCheckedAlready = false;

        private CompanyApiService companyApiService;
        private AppControl appControl;

        public InActiveProductPageViewModel(CompanyApiService companyApiService, AppControl appControl)
        {
            this.companyApiService = companyApiService;
            this.appControl = appControl;

            products = new ObservableRangeCollection<ProductModel>();

            ClickProductCommand = new Command<ProductModel>(ProductClicked);
            LoadMoreCommand = new AsyncRelayCommand(LoadMoreAsync);
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        }

        private async void ProductClicked(ProductModel product)
        {
            bool isWifiOn = await appControl.CheckWifi();
            if (!isWifiOn) return;

            if (IsCheckedProduct)
            {
                product.IsCheckedProduct = !product.IsCheckedProduct;
                product.IsNonActiveProduct = !product.IsNonActiveProduct;

                StackBottomEnabled = Products.Any(item => item.IsCheckedProduct);
                if (StackBottomEnabled)
                {
                    ActiveImage = "active.png";
                    DeleteImage = "delete.png";
                }
                else
                {
                    ActiveImage = "active_gray.png";
                    DeleteImage = "delete_gray.png";
                }

                if (IsShowChekAllProducts)
                {
                    checkAllCheckedAlready = true;
                    if (Products.Any(item => !item.IsCheckedProduct))
                    {
                        IsCheckedAllProduct = false;
                    }
                    else
                    {
                        IsCheckedAllProduct = true;
                    }
                    checkAllCheckedAlready = false;
                }
                return;
            }

            product.IsThisActivePage = false;
            product.CompanyId = appControl.CompanyInfo.company_id;
            await Shell.Current.GoToAsync(nameof(CompanyEditProductPage), new Dictionary<string, object>
            {
                ["ProductModel"] = product,
                ["IsActivePage"] = false
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
                IsShowChekProduct = false;
                IsCheckedProduct = false;

                PaginationWithDeletedParam request = new PaginationWithDeletedParam
                {
                    deleted = true,
                    offset = offset,
                    pageSize = PageSize
                };

                PosterListResponse response = await companyApiService.GetCompanyPoster(request);
                bool isOk = await appControl.CheckUserState(response);
                if (!isOk)
                {
                    await appControl.LogoutCompany();
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
                        ProductImage = string.IsNullOrWhiteSpace(item.image_url) ? "no_image.png" : item.image_url,
                        //Count = "2 qoldi",
                        ProductName = item.title,
                        ProductMakerName = item.company_name,
                        NewPrice = appControl.GetUzbCurrency(item.new_price),
                        OldPrice = appControl.GetUzbCurrency(item.old_price),
                        NewPriceDigit = item.new_price ?? 0,
                        OldPriceDigit = item.old_price ?? 0,
                        description = item.description,
                        Stars = item.avg_rating.ToString(),
                        IsNonActiveProduct = true,
                        Liked = item.liked,
                        BookmarkId = item.bookmark_id ?? 0,
                        Distance = $"{item.distance_km:0.0} km"
                    }).ToList();

                    ProductView.BeginNewAnimationCycle();
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
                IsShowChekProduct = Products.Count > 0;
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
                IsShowChekProduct = false;
                IsCheckedProduct = false;

                PaginationWithDeletedParam request = new PaginationWithDeletedParam
                {
                    deleted = true,
                    offset = offset,
                    pageSize = PageSize
                };

                PosterListResponse response = await companyApiService.GetCompanyPoster(request);
                bool isOk = await appControl.CheckUserState(response);
                if (!isOk)
                {
                    await appControl.LogoutCompany();
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
                        ProductImage = string.IsNullOrWhiteSpace(item.image_url) ? "no_image.png" : item.image_url,
                        //Count = "2 qoldi",
                        ProductName = item.title,
                        ProductMakerName = item.company_name,
                        NewPrice = appControl.GetUzbCurrency(item.new_price),
                        OldPrice = appControl.GetUzbCurrency(item.old_price),
                        NewPriceDigit = item.new_price ?? 0,
                        OldPriceDigit = item.old_price ?? 0,
                        description = item.description,
                        Stars = item.avg_rating.ToString(),
                        IsNonActiveProduct = true,
                        Liked = item.liked,
                        BookmarkId = item.bookmark_id ?? 0,
                        Distance = $"{item.distance_km:0.0} km"
                    }).ToList();

                    ProductView.BeginNewAnimationCycle();
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
                IsShowChekProduct = Products.Count > 0;
            }
        }

        public void UpdateTitle()
        {
            if (Products.Count == 0)
            {
                IsCheckedProduct = false;
            }
            IsShowChekProduct = Products.Count > 0;
            InActiveProductCount = AppResource.InActiveProductCount + " " + Products.Count;
        }

        public void ShowCheckProduct(bool show)
        {
            foreach (ProductModel product in Products)
            {
                product.ShowCheckProduct = show;
                if (!show)
                {
                    product.IsCheckedProduct = false;
                    product.IsNonActiveProduct = true;
                }
            }

            AllowSwipe = !show;
            if (!show)
            {
                ActiveImage = "active_gray.png";
                DeleteImage = "delete_gray.png";
            }
        }

        public ICommand ClickProductCommand { get; }

        public IAsyncRelayCommand LoadMoreCommand { get; }
        public IAsyncRelayCommand RefreshCommand { get; }

        private async Task LoadMoreAsync()
        {
            bool isWifiOn = await appControl.CheckWifi();
            if (!isWifiOn) return;

            if (IsLoading || IsRefreshing || !hasMoreItems)
                return;

            await LoadPromotionAsync();
        }

        private async Task RefreshAsync()
        {
            bool isWifiOn = await appControl.CheckWifi();
            if (!isWifiOn) return;

            if (IsCheckedProduct)
            {
                IsRefreshing = false;
                return;
            }

            await LoadPromotionAsync(isRefresh: true);
        }
        
        /*
        public IRelayCommand LoadMoreCommand => new RelayCommand(async () =>
        {
            bool isWifiOn = await appControl.CheckWifi();
            if (!isWifiOn) return;

            if (IsLoading || IsRefreshing || !hasMoreItems)
                return;

            await LoadPromotionAsync();
        });

        public IRelayCommand RefreshCommand => new RelayCommand(async () =>
        {
            bool isWifiOn = await appControl.CheckWifi();
            if (!isWifiOn) return;

            if (IsCheckedProduct)
            {
                IsRefreshing = false;
                return;
            }

            await LoadPromotionAsync(isRefresh: true);
        });
        */
    }
}