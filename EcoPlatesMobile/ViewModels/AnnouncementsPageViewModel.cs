using System.Diagnostics;
using Android.App;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcoPlatesMobile.Models;
using EcoPlatesMobile.Models.Requests;
using EcoPlatesMobile.Models.Requests.User;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.Announcement;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Views.Company.Components;

namespace EcoPlatesMobile.ViewModels
{  
    public partial class AnnouncementsPageViewModel : ObservableObject
    {
        [ObservableProperty] string category;
        [ObservableProperty] private Announcement selectedAnnouncement;
        [ObservableProperty] private ObservableRangeCollection<Announcement> announcements;
        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private bool isRefreshing;
        [ObservableProperty] private double sheetHeight = 420;

        private int offset = 0;
        private const int PageSize = 6;
        private bool hasMoreItems = true;

        private UserApiService userApiService;
        private CompanyApiService companyApiService;
        private UserSessionService userSessionService;
        private AppControl appControl;

        public AnnouncementsPageViewModel(UserSessionService userSessionService,
                                          UserApiService userApiService,
                                          CompanyApiService companyApiService,
                                          AppControl appControl)
        {
            this.userSessionService = userSessionService;
            this.userApiService = userApiService;
            this.companyApiService = companyApiService;
            this.appControl = appControl;

            Announcements = new ObservableRangeCollection<Announcement>();

            LoadMoreCommand = new AsyncRelayCommand(LoadMoreAsync);
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        }
        
        public async Task LoadInitialAsync()
        {
            offset = 0;
            hasMoreItems = true;
            Announcements.Clear();

            try
            {
                IsLoading = true;

                GetAnnouncementsRequest request = new GetAnnouncementsRequest
                {
                    offset = offset,
                    pageSize = PageSize,
                    actorType = userSessionService.Role == UserRole.Company ? ActorType.COMPANY.GetValue() : ActorType.USER.GetValue(),
                    actorId = userSessionService.Role == UserRole.Company ? appControl.CompanyInfo.company_id : appControl.UserInfo.user_id
                };

                AnnouncementInfoResponse response = await companyApiService.GetAnnouncements(request);
                bool isOk = await appControl.CheckUserState(response);
                if (!isOk)
                {
                    await appControl.LogoutUser();
                    return;
                }

                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    var items = response.resultData;

                    if (items == null || items.Count == 0)
                    {
                        hasMoreItems = false;
                        return;
                    }
 
                    var announcementModels = items.Select(item => new Announcement
                    {
                        AnnouncementId = item.announcement_id,
                        Title = item.title ?? string.Empty,
                        Preview = item.preview ?? string.Empty,
                        Body = item.body ?? string.Empty,
                        ImageUrl = "",
                        CreatedAtUtc = DateTimeOffset.FromUnixTimeMilliseconds(item.created_at_utc).UtcDateTime,
                        IsRead = item.is_read == 1
                    }).ToList();
 
                    Announcements.AddRange(announcementModels);

                    offset += PageSize;
                    if (announcementModels.Count < PageSize)
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

        public async Task<bool> MarkAsRead()
        {
            MarkAnnouncementReadRequest request = new MarkAnnouncementReadRequest()
            {
                announcementId = (int)SelectedAnnouncement.AnnouncementId,
                actorType = userSessionService.Role == UserRole.Company ? ActorType.COMPANY.GetValue() : ActorType.USER.GetValue(),
                actorId = userSessionService.Role == UserRole.Company ? appControl.CompanyInfo.company_id : appControl.UserInfo.user_id
            };
            Response response = await companyApiService.MarkAnnouncementAsRead(request);

            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                return true;
            }

            return false;
        }

        public async Task LoadAnnouncemenAsync(bool isRefresh = false)
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
                    Announcements.Clear();
                }
                else
                {
                    IsLoading = true;
                }

                GetAnnouncementsRequest request = new GetAnnouncementsRequest
                {
                    offset = offset,
                    pageSize = PageSize,
                    actorType = userSessionService.Role == UserRole.Company ? ActorType.COMPANY.GetValue() : ActorType.USER.GetValue(),
                    actorId = userSessionService.Role == UserRole.Company ? appControl.CompanyInfo.company_id : appControl.UserInfo.user_id
                };

                AnnouncementInfoResponse response = await companyApiService.GetAnnouncements(request);
                bool isOk = await appControl.CheckUserState(response);
                if (!isOk)
                {
                    await appControl.LogoutUser();
                    return;
                }

                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    var items = response.resultData;

                    if (items == null || items.Count == 0)
                    {
                        hasMoreItems = false;
                        return;
                    }

                    var announcementModels = items.Select(item => new Announcement
                    {
                        AnnouncementId = item.announcement_id,
                        Title = item.title ?? string.Empty,
                        Preview = item.preview ?? string.Empty,
                        Body = item.body ?? string.Empty,
                        ImageUrl = "",
                        CreatedAtUtc = DateTimeOffset.FromUnixTimeMilliseconds(item.created_at_utc).UtcDateTime,
                        IsRead = item.is_read == 1
                    }).ToList();

                    Announcements.AddRange(announcementModels);

                    offset += PageSize;
                    if (Announcements.Count < PageSize)
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
 
        public IAsyncRelayCommand LoadMoreCommand { get; }
        public IAsyncRelayCommand RefreshCommand { get; }

        private async Task LoadMoreAsync()
        {
            if (IsLoading || IsRefreshing || !hasMoreItems) return;
            bool isWifiOn = await appControl.CheckWifiOrNetwork();
            if (!isWifiOn)
            {
                IsRefreshing = false;
                IsLoading = false;
                return;
            }
            await LoadAnnouncemenAsync();
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
            await LoadAnnouncemenAsync(isRefresh: true);
        }
    } 
}