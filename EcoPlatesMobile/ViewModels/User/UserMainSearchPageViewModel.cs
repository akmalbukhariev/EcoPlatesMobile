using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private const int PageSize = 4;
        private bool hasMoreItems = true;
        private List<HistoryDataInfo> AllHistoryItems = new();

        public ICommand ClickProductCommand { get; }
        public ICommand ClickHistoryCommand { get; }
        public ICommand RemoveHistoryCommand { get; }

        private UserApiService userApiService;
        private AppControl appControl;
        private AppStoreService appStoreService;

        public UserMainSearchPageViewModel(UserApiService userApiService, AppControl appControl, AppStoreService appStoreService)
        {
            this.userApiService = userApiService;
            this.appControl = appControl;
            this.appStoreService = appStoreService;

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
            var stored = appStoreService.Get<List<string>>(HistoryKey, new());
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

        public async Task LoadInitialProductAsync(bool loadMore = false)
        {
            if (loadMore)
            {
                if (IsLoading || !hasMoreItems)
                    return;
            }
            else
            {
                offsetProduct = 0;
                Products.Clear();
                hasMoreItems = true;
            }

            try
            {
                IsLoading = true;

                PosterLocationAndNameRequest request = new PosterLocationAndNameRequest()
                {
                    offset = offsetProduct,
                    pageSize = PageSize,
                    user_lat = appControl.UserInfo.location_latitude,
                    user_lon = appControl.UserInfo.location_longitude,
                    radius_km = appControl.UserInfo.radius_km,
                    title = SearchText
                };
 
                PosterListResponse response = await userApiService.GetPostersByCurrentLocationAndName(request);

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
                Debug.WriteLine($"[ERROR] LoadInitialProductAsync: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        public IRelayCommand LoadProductMoreCommand => new RelayCommand( async () =>
        {
            if (IsLoading || !hasMoreItems)
                return;

            await LoadInitialProductAsync(true);
        });

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
            appStoreService.Set(HistoryKey, saved);
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
