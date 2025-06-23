using CommunityToolkit.Mvvm.ComponentModel;
using EcoPlatesMobile.Models.Requests.User;
using EcoPlatesMobile.Models.Responses.Company;
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
    public partial class HistoryDataInfo : ObservableObject
    {
        [ObservableProperty] private string searchedText;
    }

    public partial class UserMainSearchPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableRangeCollection<ProductModel> products;
        [ObservableProperty] private ObservableRangeCollection<HistoryDataInfo> historyList;

        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private bool isRefreshingProduct;
        [ObservableProperty] private bool isRefreshingCompany;
        [ObservableProperty] private bool showProductResult;
        [ObservableProperty] private bool showFilterSearchList;
        [ObservableProperty] private bool showRecentSearchList;
        [ObservableProperty] private string searchText;

        private const string HistoryKey = "SearchHistoryProduct";

        private int offsetProduct = 0;
        private int offsetCompany = 0;
        private const int PageSize = 4;
        private bool hasMoreProductItems = true;
        private List<HistoryDataInfo> AllHistoryItems = new();

        public ICommand ClickProductCommand { get; }
        public ICommand ClickHistoryCommand { get; }
        public ICommand RemoveHistoryCommand { get; }

        public UserMainSearchPageViewModel()
        {
            products = new ObservableRangeCollection<ProductModel>();
            historyList = new ObservableRangeCollection<HistoryDataInfo>();

            ClickProductCommand = new Command<ProductModel>(ProductClicked);
            ClickHistoryCommand = new Command<HistoryDataInfo>(ClickHistoryItem);
            RemoveHistoryCommand = new Command<HistoryDataInfo>(RemoveHistoryItem);
             
            ShowProductResult = false;
            ShowFilterSearchList = false;
            ShowRecentSearchList = true;

            LoadSearchHistory();
        }

        private void LoadSearchHistory()
        {
            var stored = AppService.Get<AppStoreService>().Get<List<string>>(HistoryKey, new());
            AllHistoryItems = stored.Select(t => new HistoryDataInfo { SearchedText = t }).ToList();
            FilterHistory(SearchText);
        }

        private async void ProductClicked(ProductModel product)
        {
            await Shell.Current.GoToAsync(nameof(DetailProductPage), new Dictionary<string, object>
            {
                ["ProductModel"] = product
            });
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

                PosterLocationAndNameRequest request = new PosterLocationAndNameRequest()
                {
                    offset = offsetProduct,
                    pageSize = PageSize,
                    user_lat = userInfo.location_latitude,
                    user_lon = userInfo.location_longitude,
                    radius_km = userInfo.radius_km,
                    title = SearchText
                };

                var apiService = AppService.Get<UserApiService>();
                PosterListResponse response = await apiService.GetPostersByCurrentLocationAndName(request);

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

        partial void OnSearchTextChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                ShowRecentSearchList = true;
                ShowFilterSearchList = false;
                ShowProductResult = false;

                HistoryList.ReplaceRange(AllHistoryItems); // Show full history again
            }
            else
            {
                ShowRecentSearchList = false;
                ShowFilterSearchList = true;
                ShowProductResult = false;

                FilterHistory(value); // Filter history by typed input
            }
        }

        private async void ClickHistoryItem(HistoryDataInfo item)
        {
             SearchText = item.SearchedText;

            ShowProductResult = true;
            ShowFilterSearchList = false;
            ShowRecentSearchList = false;

            ExecuteSearch(item.SearchedText);
            await LoadInitialProductAsync();
        }

        private void RemoveHistoryItem(HistoryDataInfo item)
        {
            if (AllHistoryItems.Contains(item))
            {
                AllHistoryItems.Remove(item);
                SaveHistory();
                FilterHistory(SearchText);
            }
        }

        public void ExecuteSearch(string? keyword = null)
        {
            var term = keyword ?? SearchText?.Trim();
            if (string.IsNullOrEmpty(term))
                return;

            SaveSearchTerm(term);
            ShowFilterSearchList = false;
            ShowProductResult = true;
            ShowRecentSearchList = false;
        }

        private void SaveSearchTerm(string term)
        {
            if (!AllHistoryItems.Any(h => h.SearchedText.Equals(term, StringComparison.OrdinalIgnoreCase)))
            {
                var newItem = new HistoryDataInfo { SearchedText = term };
                AllHistoryItems.Insert(0, newItem);
                SaveHistory();
                FilterHistory(SearchText);
            }
        }

        private void SaveHistory()
        {
            var saved = AllHistoryItems.Select(h => h.SearchedText).ToList();
            AppService.Get<AppStoreService>().Set(HistoryKey, saved);
        }

        private void FilterHistory(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                HistoryList.ReplaceRange(AllHistoryItems);
                 
                ShowFilterSearchList = false;
                ShowRecentSearchList = true;
            }
            else
            {
                var filtered = AllHistoryItems
                    .Where(h => h.SearchedText.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                HistoryList.ReplaceRange(filtered);
 
                ShowFilterSearchList = filtered.Any();
                ShowRecentSearchList = false;
            }
        }
    }
}
