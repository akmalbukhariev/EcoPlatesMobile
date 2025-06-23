using CommunityToolkit.Mvvm.ComponentModel;
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

        private int offsetProduct = 0;
        private int offsetCompany = 0;
        private const int PageSize = 4;
        private bool hasMoreCompanyItems = true;
        private List<HistoryDataInfo> AllHistoryItems = new();

        public ICommand ClickCompanyCommand { get; }
        public ICommand ClickHistoryCommand { get; }
        public ICommand RemoveHistoryCommand { get; }

        public UserBrowserSearchPageViewModel()
        {
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
            var stored = AppService.Get<AppStoreService>().Get<List<string>>(HistoryKey, new());
            AllHistoryItems = stored.Select(t => new HistoryDataInfo { SearchedText = t }).ToList();
            FilterHistory(SearchText);
        }

        private async void ComapnyClicked(CompanyModel company)
        {
            await AppNavigatorService.NavigateTo($"{nameof(UserCompanyPage)}?CompanyId={company.CompanyId}");
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
                CompanyLocationAndNameRequest request = new CompanyLocationAndNameRequest()
                {
                    radius_km = userInfo.radius_km,
                    user_lat = userInfo.location_latitude,
                    user_lon = userInfo.location_longitude,
                    offset = offsetCompany,
                    pageSize = PageSize,
                    company_name = SearchText
                };

                var apiService = AppService.Get<UserApiService>();
                CompanyListResponse response = await apiService.GetCompaniesByCurrentLocationAndName(request);

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
                        Liked = item.liked,
                        BookmarkId = item.bookmark_id ?? 0,
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

        partial void OnSearchTextChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                ShowRecentSearchList = true;
                ShowFilterSearchList = false;
                showCompanyResult = false;

                HistoryList.ReplaceRange(AllHistoryItems); // Show full history again
            }
            else
            {
                ShowRecentSearchList = false;
                ShowFilterSearchList = true;
                showCompanyResult = false;

                FilterHistory(value); // Filter history by typed input
            }
        }

        private async void ClickHistoryItem(HistoryDataInfo item)
        {
             SearchText = item.SearchedText;

            showCompanyResult = true;
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
