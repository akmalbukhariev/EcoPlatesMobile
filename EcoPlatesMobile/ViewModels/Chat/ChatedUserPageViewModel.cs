using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EcoPlatesMobile.Models.Responses.Chat;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Models.Responses.User;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.ViewModels.Chat
{
    public partial class ChatedUserPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableRangeCollection<SenderIdInfo> users;
        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private bool isRefreshing;

        private CompanyApiService companyApiService;
        private UserApiService userApiService;
        private ChatApiService chatApiService;
        private AppControl appControl;
        private UserSessionService userSessionService;
        public ChatedUserPageViewModel(CompanyApiService companyApiService, UserApiService userApiService, ChatApiService chatApiService, AppControl appControl, UserSessionService userSessionService)
        {
            this.companyApiService = companyApiService;
            this.userApiService = userApiService;
            this.chatApiService = chatApiService;
            this.appControl = appControl;
            this.userSessionService = userSessionService;

            users = new ObservableRangeCollection<SenderIdInfo>();
        }

        public async Task LoadUsersData()
        {
            //IsLoading = true;
            IsRefreshing = true;

            try
            {
                UnreadMessagesRequest request = new UnreadMessagesRequest()
                {
                    receiver_id = appControl.CompanyInfo.company_id,
                    receiver_type = UserRole.Company.ToString().ToUpper()
                };

                ChatSenderIdResponse response = await chatApiService.GetSendersWithUnread(request);

                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    if (response.resultData != null && response.resultData.Count != 0)
                    {
                        List<long> idList = response.resultData.Select(item => item.sender_id).ToList();

                        UserInfoListResponse response2 = await userApiService.GetUserInfoList(idList);
                        var items = response2.resultData;

                        var userInfoList = items.Select(item => new SenderIdInfo()
                        {
                            UserImage = item.profile_picture_url,
                            UserName = item.first_name,
                            RightImage = response.resultData.Any(sender => sender.sender_id == (long)item.user_id && sender.has_unread)
                                        ?  "unread_company_msg.png" : "right.png",

                            chatPageModel = new Models.Chat.ChatPageModel()
                            {
                                ReceiverName = item.first_name,
                                ReceiverPhone = item.phone_number,
                                ReceiverImage = item.profile_picture_url,

                                SenderId = appControl.CompanyInfo.company_id,
                                SenderType = UserRole.Company.ToString().ToUpper(),
                                ReceiverId = item.user_id,
                                ReceiverType = UserRole.User.ToString().ToUpper(),
                            }
                        });

                        Users.Clear();
                        Users.AddRange(userInfoList);
                    }
                }
                else
                {
                    await AlertService.ShowAlertAsync(AppResource.Error, response.resultMsg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //IsLoading = false;
                IsRefreshing = false;
            }
        }

        public async Task LoadCompaniesData()
        {
            //IsLoading = true;
            IsRefreshing = true;

            try
            {
                UnreadMessagesRequest request = new UnreadMessagesRequest()
                {
                    receiver_id = appControl.UserInfo.user_id,
                    receiver_type = UserRole.User.ToString().ToUpper()
                };

                ChatSenderIdResponse response = await chatApiService.GetSendersWithUnread(request);
                
                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    if (response.resultData != null && response.resultData.Count != 0)
                    {
                        List<long> idList = response.resultData.Select(item => item.sender_id).ToList();

                        CompanyListResponse response2 = await companyApiService.GetCompanyInfoList(idList);
                        var items = response2.resultData;

                        var userInfoList = items.Select(item => new SenderIdInfo()
                        {
                            UserImage = item.logo_url,
                            UserName = item.company_name,
                            RightImage = response.resultData.Any(sender => sender.sender_id == (long)item.company_id && sender.has_unread)
                                        ?  "unread_user_msg.png" : "right.png",

                            chatPageModel = new Models.Chat.ChatPageModel()
                            {
                                ReceiverName = item.company_name,
                                ReceiverPhone = item.phone_number,
                                ReceiverImage = item.logo_url,

                                SenderId = appControl.UserInfo.user_id,
                                SenderType = UserRole.User.ToString().ToUpper(),
                                ReceiverId = item.company_id,
                                ReceiverType = UserRole.Company.ToString().ToUpper(),
                            }
                        });

                        Users.Clear();
                        Users.AddRange(userInfoList);
                    }
                }
                else
                {
                    await AlertService.ShowAlertAsync(AppResource.Error, response.resultMsg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //IsLoading = false;
                IsRefreshing = false;
            }
        }

        public IRelayCommand RefreshCommand => new RelayCommand(async () =>
        {
            bool isWifiOn = await appControl.CheckWifi();
		    if (!isWifiOn) return;

            if (userSessionService.Role == UserRole.User)
                await LoadCompaniesData();
            else
                await LoadUsersData();
        });
    }
}
