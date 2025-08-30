
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcoPlatesMobile.Models.Requests.Company;
using EcoPlatesMobile.Models.Requests.User;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Views.User.Pages;

namespace EcoPlatesMobile.ViewModels.User
{
    public partial class UserBrowserPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableRangeCollection<CompanyModel> companies;
        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private bool isRefreshing;
        [ObservableProperty] private bool showLikedView;
        [ObservableProperty] private bool isLikedViewLiked;

        private int offset = 0;
        private const int PageSize = 4;
        private bool hasMoreItems = true;

        private UserApiService userApiService;
        private AppControl appControl;

        public UserBrowserPageViewModel(UserApiService userApiService, AppControl appControl)
        {
            this.userApiService = userApiService;
            this.appControl = appControl;

            Companies = new ObservableRangeCollection<CompanyModel>();
            LikeCompanyCommand = new Command<CompanyModel>(CompanyLiked);
            ClickCompanyCommand = new Command<CompanyModel>(ComapnyClicked);
        }

        private async void CompanyLiked(CompanyModel product)
        {
            bool isWifiOn = await appControl.CheckWifi();
		    if (!isWifiOn) return;

            product.Liked = !product.Liked;
 
            SaveOrUpdateBookmarksCompanyRequest request = new SaveOrUpdateBookmarksCompanyRequest()
            {
                user_id = appControl.UserInfo.user_id,
                company_id = product.CompanyId,
                deleted = product.Liked ? false : true,
            };
 
            Response response = await userApiService.UpdateUserBookmarkCompanyStatus(request);

            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                IsLikedViewLiked = product.Liked;
                ShowLikedView = true;

                appControl.RefreshAllPages();
            }
        }

        private async void ComapnyClicked(CompanyModel company)
        {
            bool isWifiOn = await appControl.CheckWifi();
		    if (!isWifiOn) return;

            await Shell.Current.GoToAsync($"{nameof(UserCompanyPage)}?CompanyId={company.CompanyId}");
        }

        public async Task LoadInitialAsync()
        {
            offset = 0;
            hasMoreItems = true;
            Companies.Clear();
            
            try
            {
                IsLoading = true;

                CompanyLocationRequest request = new CompanyLocationRequest()
                {
                    radius_km = appControl.UserInfo.radius_km,
                    user_lat = appControl.UserInfo.location_latitude,//37.518313,
                    user_lon = appControl.UserInfo.location_longitude,//126.724187,
                    offset = offset,
                    pageSize = PageSize,
                };

                CompanyListResponse response = await userApiService.GetCompaniesByCurrentLocation(request);

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

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Companies.ReplaceRange(companyModels);
                    });

                    offset += PageSize;
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
                Debug.WriteLine($"[ERROR] LoadInitialAsync: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task LoadCompaniesAsync(bool isRefresh = false)
        {
            Companies.Clear();
            
            if (IsLoading || (!hasMoreItems && !isRefresh))
                return;

            try
            {
                if (isRefresh)
                {
                    IsRefreshing = true;
                    offset = 0;
                    hasMoreItems = true;
                }
                else
                {
                    IsLoading = true;
                }
                     
                CompanyLocationRequest request = new CompanyLocationRequest()
                {
                    radius_km = appControl.UserInfo.radius_km,
                    user_lat = appControl.UserInfo.location_latitude,
                    user_lon = appControl.UserInfo.location_longitude,
                    offset = offset,
                    pageSize = PageSize,
                };
 
                CompanyListResponse response = await userApiService.GetCompaniesByCurrentLocation(request);

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

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        if (isRefresh)
                        {
                            Companies.ReplaceRange(companyModels);
                        }
                        else
                        {
                            Companies.AddRange(companyModels);
                        }
                    });

                    offset += PageSize;
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
                Debug.WriteLine($"[ERROR] LoadPromotionAsync: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
                IsLoading = false;
            }
        }
        
        public ICommand LikeCompanyCommand { get; }

        public ICommand ClickCompanyCommand { get; }

        public IRelayCommand LoadMoreCommand => new RelayCommand(async () =>
        {
            bool isWifiOn = await appControl.CheckWifi();
		    if (!isWifiOn) return;

            if (IsLoading || IsRefreshing || !hasMoreItems)
                return;

            await LoadCompaniesAsync();
        });

        public IRelayCommand RefreshCommand => new RelayCommand(async () =>
        {
            bool isWifiOn = await appControl.CheckWifi();
		    if (!isWifiOn) return;

            await LoadCompaniesAsync(isRefresh: true);
        });
    }
}