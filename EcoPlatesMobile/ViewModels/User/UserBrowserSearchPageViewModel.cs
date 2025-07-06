using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcoPlatesMobile.Models.Requests.Company;
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
    public partial class UserBrowserSearchPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableRangeCollection<CompanyModel> companies;
        [ObservableProperty] private ObservableRangeCollection<HistoryDataInfo> historyList;

        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private bool isRefreshingProduct;
        [ObservableProperty] private bool isRefreshingCompany;
        [ObservableProperty] private bool showCompanyResult;
        [ObservableProperty] private bool showFilterSearchList;
        [ObservableProperty] private bool showRecentSearchList;
        [ObservableProperty] private string searchText;

        private const string HistoryKey = "SearchHistoryCompany";

        private int offsetCompany = 0;
        private const int PageSize = 4;
        private bool hasMoreItems = true;
        private List<HistoryDataInfo> AllHistoryItems = new();

        public ICommand ClickCompanyCommand { get; }
        public ICommand ClickHistoryCommand { get; }
        public ICommand RemoveHistoryCommand { get; }

        private UserApiService userApiService;
        private AppControl appControl;
        private AppStoreService appStoreService;

        public UserBrowserSearchPageViewModel(UserApiService userApiService, AppControl appControl, AppStoreService appStoreService)
        {
            this.userApiService = userApiService;
            this.appControl = appControl;
            this.appStoreService = appStoreService;

            companies = new ObservableRangeCollection<CompanyModel>();
            historyList = new ObservableRangeCollection<HistoryDataInfo>();

            ClickCompanyCommand = new Command<CompanyModel>(ComapnyClicked);
            ClickHistoryCommand = new Command<HistoryDataInfo>(ClickHistoryItem);
            RemoveHistoryCommand = new Command<HistoryDataInfo>(RemoveHistoryItem);
             
            ShowCompanyResult = false;
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

        private async void ComapnyClicked(CompanyModel company)
        {
            await AppNavigatorService.NavigateTo($"{nameof(UserCompanyPage)}?CompanyId={company.CompanyId}");
        }

        public async Task LoadInitialCompanyAsync(bool loadMore = false)
        {
            if (loadMore)
            {
                if (IsLoading || !hasMoreItems)
                    return;
            }
            else
            {
                offsetCompany = 0;
                Companies.Clear();
                hasMoreItems = true;
            }

            try
            {
                IsLoading = true;
 
                CompanyLocationAndNameRequest request = new CompanyLocationAndNameRequest()
                {
                    radius_km = appControl.UserInfo.radius_km,
                    user_lat = appControl.UserInfo.location_latitude,
                    user_lon = appControl.UserInfo.location_longitude,
                    offset = offsetCompany,
                    pageSize = PageSize,
                    company_name = SearchText
                };
 
                CompanyListResponse response = await userApiService.GetCompaniesByCurrentLocationAndName(request);

                if (response.resultCode == ApiResult.COMPANY_EXIST.GetCodeToString())
                {
                    var items = response.resultData;

                    if (items == null || items.Count == 0)
                    {
                        hasMoreItems = false;
                        return;
                    }

                    var companyModels = items.Select(item => new CompanyModel
                    {
                        CompanyId = item.company_id,
                        CompanyImage = string.IsNullOrWhiteSpace(item.logo_url) ? "no_image.png" : item.logo_url,
                        CompanyName = item.company_name,
                        WorkingTime = item.working_hours,
                        Liked = item.liked,
                        BookmarkId = item.bookmark_id ?? 0,
                        Distance = $"{item.distance_km:0.0} km"
                    }).ToList();

                    Companies.AddRange(companyModels);

                    offsetCompany += PageSize;
                    if (companyModels.Count < PageSize)
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
                Debug.WriteLine($"[ERROR] LoadInitialCompanyAsync: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public IRelayCommand LoadCompanyMoreCommand => new RelayCommand(async () =>
        {
            if (IsLoading || !hasMoreItems)
                return;

            await LoadInitialCompanyAsync(true);
        });

        partial void OnSearchTextChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                ShowRecentSearchList = true;
                ShowFilterSearchList = false;
                ShowCompanyResult = false;

                HistoryList.ReplaceRange(AllHistoryItems); // Show full history again
            }
            else
            {
                ShowRecentSearchList = false;
                ShowFilterSearchList = true;
                ShowCompanyResult = false;

                FilterHistory(value); // Filter history by typed input
            }
        }

        private async void ClickHistoryItem(HistoryDataInfo item)
        {
             SearchText = item.SearchedText;

            ShowCompanyResult = true;
            ShowFilterSearchList = false;
            ShowRecentSearchList = false;

            ExecuteSearch(item.SearchedText);
            await LoadInitialCompanyAsync();
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
            ShowCompanyResult = true;
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
