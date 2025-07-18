using CommunityToolkit.Mvvm.ComponentModel;
using EcoPlatesMobile.Models.Responses.Chat;
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

        private CompanyApiService companyApiService;
        private UserApiService userApiService;
        private AppControl appControl;
        public ChatedUserPageViewModel(CompanyApiService companyApiService, UserApiService userApiService, AppControl appControl)
        {
            this.companyApiService = companyApiService;
            this.userApiService = userApiService;
            this.appControl = appControl;

            this.users = new ObservableRangeCollection<SenderIdInfo>();
        }

        public async Task LoadData()
        {
            IsLoading = true;

            try
            {
                ChatSenderIdResponse response = await companyApiService.GetSenderIdList(appControl.CompanyInfo.company_id);

                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    if (response.resultData != null && response.resultData.Count != 0)
                    {
                        GetUserInfoListResponse response2 = await userApiService.GetUserInfoList(response.resultData);
                        var items = response2.resultData;
                        var userInfoList = items.Select(item => new SenderIdInfo()
                        {
                            UserImage = item.profile_picture_url,
                            UserName = item.first_name,
                            chatPageModel = new Models.Chat.ChatPageModel()
                            {
                                ReceiverName = appControl.CompanyInfo.company_name,
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
                IsLoading = false;
            }
        }
    }
}
