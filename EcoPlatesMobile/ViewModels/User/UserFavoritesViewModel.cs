using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcoPlatesMobile.Models.Requests.User;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.User;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Views.User.Pages; 
using System.Diagnostics; 
using System.Windows.Input;

namespace EcoPlatesMobile.ViewModels.User
{
    public partial class UserFavoritesViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableRangeCollection<ProductModel> products;
        [ObservableProperty] private ObservableRangeCollection<CompanyModel> companies;

        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private bool isRefreshingProduct;
        [ObservableProperty] private bool isRefreshingCompany;

        private int offsetProduct = 0;
        private int offsetCompany = 0;
        private const int PageSize = 4;
        private bool hasMoreProductItems = true;
        private bool hasMoreCompanyItems = true;

        public UserFavoritesViewModel()
        {
            products = new ObservableRangeCollection<ProductModel>();
            companies = new ObservableRangeCollection<CompanyModel>();

            ClickProductCommand = new Command<ProductModel>(ProductClicked);
            ClickCompanyCommand = new Command<CompanyModel>(ComapnyClicked);
        }

        private async void ProductClicked(ProductModel product)
        {
            await Shell.Current.GoToAsync(nameof(DetailProductPage), new Dictionary<string, object>
            {
                ["ProductModel"] = product
            });
        }

        private async void ComapnyClicked(CompanyModel company)
        {
            await Shell.Current.GoToAsync($"{nameof(UserCompanyPage)}?CompanyId={company.CompanyId}");
        }

        public async Task LoadInitialProductAsync()
        {
            offsetProduct = 0;
            Products.Clear();
            hasMoreProductItems = true;

            try
            {
                IsLoading = true;

                var userInfo = AppService.Get<AppControl>().UserInfo;

                PaginationWithLocationRequest request = new PaginationWithLocationRequest()
                {
                    user_lat = userInfo.location_latitude,//37.518313,
                    user_lon = userInfo.location_longitude,//126.724187,
                    offset = offsetProduct,
                    pageSize = PageSize
                };

                var apiService = AppService.Get<UserApiService>();
                BookmarkPromotionListResponse response = await apiService.GetUserBookmarkPromotion(request);

                if (response.resultCode == ApiResult.POSTER_EXIST.GetCodeToString())
                {
                    var items = response.resultData;

                    if (items == null || items.Count == 0)
                    {
                        hasMoreProductItems = false;
                        return;
                    }

                    var productModels = items.Select(item => new ProductModel
                    {
                        PromotionId = item.poster_id ?? 0,
                        ProductImage = string.IsNullOrWhiteSpace(item.image_url) ? "no_image.png" : item.image_url,
                        Count = "2 qoldi",
                        ProductName = item.title,
                        ProductMakerName = item.company_name,
                        NewPrice = $"{item.new_price:N0} so'm",
                        OldPrice = $"{item.old_price:N0} so'm",
                        Stars = $"{item.avg_rating}({item.total_reviews})",
                        Liked = item.liked,
                        BookmarkId = item.bookmark_id ?? 0,
                        Distance = $"{item.distance_km:0.0} km"
                    }).ToList();

                    Products.AddRange(productModels);

                    offsetProduct += PageSize;
                    if (productModels.Count < PageSize)
                    {
                        hasMoreProductItems = false;
                    }
                }
                else
                {
                    hasMoreProductItems = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] LoadInitialProductAsync: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task LoadInitialCompanyAsync()
        {
            offsetCompany = 0;
            Companies.Clear();
            hasMoreCompanyItems = true;

            try
            {
                IsLoading = true;

                var userInfo = AppService.Get<AppControl>().UserInfo;

                PaginationWithLocationRequest request = new PaginationWithLocationRequest()
                {
                    user_lat = userInfo.location_latitude,//37.518313,
                    user_lon = userInfo.location_longitude,//126.724187,
                    offset = offsetCompany,
                    pageSize = PageSize
                };

                var apiService = AppService.Get<UserApiService>();
                BookmarkCompanyListResponse response = await apiService.GetUserBookmarkCompany(request);

                if (response.resultCode == ApiResult.COMPANY_EXIST.GetCodeToString())
                {
                    var items = response.resultData;

                    if (items == null || items.Count == 0)
                    {
                        hasMoreCompanyItems = false;
                        return;
                    }

                    var companyModels = items.Select(item => new CompanyModel
                    {
                        CompanyId = item.company_id,
                        CompanyImage = string.IsNullOrWhiteSpace(item.logo_url) ? "no_image.png" : item.logo_url,
                        CompanyName = item.company_name,
                        WorkingTime = item.working_hours,
                        Stars = "3.1",
                        Liked = item.liked,
                        Distance = $"{item.distance_km:0.0} km"
                    }).ToList();

                    Companies.AddRange(companyModels);

                    offsetCompany += PageSize;
                    if (companyModels.Count < PageSize)
                    {
                        hasMoreCompanyItems = false;
                    }
                }
                else
                {
                    hasMoreCompanyItems = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] LoadInitialCompanyAsync: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task DeleteProduct(ProductModel product)
        {
            try
            {
                IsLoading = true;

                SaveOrUpdateBookmarksPromotionRequest request = new SaveOrUpdateBookmarksPromotionRequest()
                {
                    user_id = AppService.Get<AppControl>().UserInfo.user_id,
                    promotion_id = product.PromotionId,
                    deleted = true,
                };

                var apiService = AppService.Get<UserApiService>();
                Response response = await apiService.UpdateUserBookmarkPromotionStatus(request);
                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    Products.Remove(product);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] DeleteProduct: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task DeleteCompany(CompanyModel product)
        {
            try
            {
                IsLoading = true;

                SaveOrUpdateBookmarksCompanyRequest request = new SaveOrUpdateBookmarksCompanyRequest()
                {
                    user_id = AppService.Get<AppControl>().UserInfo.user_id,
                    company_id = product.CompanyId,
                    deleted = true,
                };

                var apiService = AppService.Get<UserApiService>();
                Response response = await apiService.UpdateUserBookmarkCompanyStatus(request);
                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    Companies.Remove(product);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] DeleteCompany: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task LoadProductFavoritesAsync(bool isRefresh = false)
        {
            if (IsLoading || (!hasMoreProductItems && !isRefresh))
                return;

            try
            {
                if (isRefresh)
                {
                    IsRefreshingProduct = true;
                    offsetProduct = 0;
                    hasMoreProductItems = true;
                    Products.Clear();
                }
                else
                {
                    IsLoading = true;
                }

                var userInfo = AppService.Get<AppControl>().UserInfo;

                PaginationWithLocationRequest request = new PaginationWithLocationRequest()
                {
                     user_lat = userInfo.location_latitude,//37.518313,
                     user_lon = userInfo.location_longitude,//126.724187,
                     offset = offsetProduct,
                     pageSize = PageSize
                };

                var apiService = AppService.Get<UserApiService>();
                BookmarkPromotionListResponse response = await apiService.GetUserBookmarkPromotion(request);

                if (response.resultCode == ApiResult.POSTER_EXIST.GetCodeToString())
                {
                    var items = response.resultData;

                    if (items == null || items.Count == 0)
                    {
                        hasMoreProductItems = false;
                        return;
                    }

                    var productModels = items.Select(item => new ProductModel
                    {
                        PromotionId = item.poster_id ?? 0,
                        ProductImage = string.IsNullOrWhiteSpace(item.image_url) ? "no_image.png" : item.image_url,
                        Count = "2 qoldi",
                        ProductName = item.title,
                        ProductMakerName = item.company_name,
                        NewPrice = $"{item.new_price:N0} so'm",
                        OldPrice = $"{item.old_price:N0} so'm",
                        Stars = $"{item.avg_rating}({item.total_reviews})",
                        Liked = item.liked,
                        BookmarkId = item.bookmark_id ?? 0,
                        Distance = $"{item.distance_km:0.0} km"
                    }).ToList();

                    Products.AddRange(productModels);

                    offsetProduct += PageSize;
                    if (productModels.Count < PageSize)
                    {
                        hasMoreProductItems = false;
                    }
                }
                else
                {
                    hasMoreProductItems = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] LoadProductFavoritesAsync: {ex.Message}");
            }
            finally
            {
                IsRefreshingProduct = false;
                IsLoading = false;
            }
        }

        public async Task LoadCompanyFavoritesAsync(bool isRefresh = false)
        {
            if (IsLoading || (!hasMoreCompanyItems && !isRefresh))
                return;

            try
            {
                if (isRefresh)
                {
                    IsRefreshingCompany = true;
                    offsetCompany = 0;
                    hasMoreCompanyItems = true;
                    Companies.Clear();
                }
                else
                {
                    IsLoading = true;
                }
   
                var userInfo = AppService.Get<AppControl>().UserInfo;

                PaginationWithLocationRequest request = new PaginationWithLocationRequest()
                {
                    user_lat = userInfo.location_latitude,//37.518313,
                    user_lon = userInfo.location_longitude,//126.724187,
                    offset = offsetCompany,
                    pageSize = PageSize
                };

                var apiService = AppService.Get<UserApiService>();
                BookmarkCompanyListResponse response = await apiService.GetUserBookmarkCompany(request);

                if (response.resultCode == ApiResult.COMPANY_EXIST.GetCodeToString())
                {
                    var items = response.resultData;

                    if (items == null || items.Count == 0)
                    {
                        hasMoreCompanyItems = false;
                        return;
                    }

                    var companyModels = items.Select(item => new CompanyModel
                    {
                        CompanyId = item.company_id,
                        CompanyImage = string.IsNullOrWhiteSpace(item.logo_url) ? "no_image.png" : item.logo_url,
                        CompanyName = item.company_name,
                        WorkingTime = item.working_hours,
                        Stars = "3.1",
                        Liked = item.liked,
                        Distance = $"{item.distance_km:0.0} km"
                    }).ToList();

                    Companies.AddRange(companyModels);

                    offsetCompany += PageSize;
                    if (companyModels.Count < PageSize)
                    {
                        hasMoreCompanyItems = false;
                    }
                }
                else
                {
                    hasMoreCompanyItems = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] LoadCompanyFavoritesAsync: {ex.Message}");
            }
            finally
            {
                IsRefreshingCompany = false;
                IsLoading = false;
            }
        }

        public ICommand ClickProductCommand { get; }
        public ICommand ClickCompanyCommand { get; }

        public IRelayCommand RefreshProductCommand => new RelayCommand(async () =>
        {
            await LoadProductFavoritesAsync(isRefresh: true);
        });

        public IRelayCommand RefreshCompanyCommand => new RelayCommand(async () =>
        {
            await LoadCompanyFavoritesAsync(isRefresh: true);
        });

        public IRelayCommand LoadProductMoreCommand => new RelayCommand(async () =>
        {
            if (IsLoading || IsRefreshingProduct || !hasMoreProductItems)
                return;

            await LoadProductFavoritesAsync();
        });

        public IRelayCommand LoadCompanyMoreCommand => new RelayCommand(async () =>
        {
            if (IsLoading || IsRefreshingCompany || !hasMoreCompanyItems)
                return;

            await LoadCompanyFavoritesAsync();
        });
    }
}
