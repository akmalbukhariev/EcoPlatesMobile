using EcoPlatesMobile.Models.Requests.Company;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.ViewModels.Company;
using EcoPlatesMobile.Models.Responses.Notification;
using Newtonsoft.Json.Linq;
using EcoPlatesMobile.Models.Chat;
using EcoPlatesMobile.Views.Chat;

namespace EcoPlatesMobile.Views.Company.Pages;

[QueryProperty(nameof(ShowBackQuery), nameof(ShowBackQuery))]
[QueryProperty(nameof(ShowTabBarQuery), nameof(ShowTabBarQuery))]
public partial class ActiveProductPage : BasePage
{
    private bool ShowBack { get; set; } = false;
    private bool ShowTabBar { get; set; } = true;

    public string ShowBackQuery
    {
        set
        {
            if (bool.TryParse(value, out var result))
                ShowBack = result;
        }
    }

    public string ShowTabBarQuery
    {
        set
        {
            if (bool.TryParse(value, out var result))
                ShowTabBar = result;
        }
    }

    private ActiveProductPageViewModel viewModel;
    private AppControl appControl;
    private CompanyApiService companyApiService;

    public ActiveProductPage(ActiveProductPageViewModel vm, CompanyApiService companyApiService, AppControl appControl)
    {
        InitializeComponent();

        this.viewModel = vm;
        this.companyApiService = companyApiService;
        this.appControl = appControl;

        appControl.NotificationSubscriber = this;
        
#if ANDROID
        MessagingCenter.Subscribe<MainActivity, NotificationData>(appControl.NotificationSubscriber, Constants.NOTIFICATION_BODY, OnNotificationReceived);
#endif

        this.BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetTabBarIsVisible(this, ShowTabBar);

        header.ShowBack = ShowBack;

        bool isWifiOn = await appControl.CheckWifi();
        if (!isWifiOn) return;

        if (appControl.NotificationData != null && appControl.NotificationData.body != string.Empty)
        {
            await MoveToPageUsingNotification(appControl.NotificationData);
        }
        else
        {
            await viewModel.LoadInitialAsync();
        }
    }

#if ANDROID
    private async void OnNotificationReceived(MainActivity sender, NotificationData notificationData)
    {
        await MoveToPageUsingNotification(notificationData);
    }
#endif

    private async Task MoveToPageUsingNotification(NotificationData notificationData)
    {
        var jObject = JObject.Parse(notificationData.body);
        var notificationTypeValue = jObject["notificationType"]?.ToString();

        appControl.NotificationData = null;
        if (!Enum.TryParse(notificationTypeValue, out NotificationType notificationType))
        {
            Console.WriteLine("Unknown notification type.");
            return;
        }

        switch (notificationType)
        {
            case NotificationType.NEW_MESSAGE:
                var messageData = jObject.ToObject<NewMessagePushNotificationResponse>();
                ChatPageModel chatPageModel = new ChatPageModel()
                {
                    ReceiverName = messageData.sender_name,
                    ReceiverPhone = messageData.sender_phone,
                    ReceiverImage = messageData.sender_image,

                    SenderId = messageData.receiver_id,
                    SenderType = messageData.receiver_type,
                    ReceiverId = messageData.sender_id,
                    ReceiverType = messageData.sender_type,
                };

                await Shell.Current.GoToAsync(nameof(ChattingPage), new Dictionary<string, object>
                {
                    ["ChatPageModel"] = chatPageModel
                });

                break;

            default:
                Console.WriteLine("Unhandled notification type.");
                break;
        }
    }

    private async void EditProduct_Invoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem &&
            swipeItem.Parent is SwipeItems swipeItems &&
            swipeItems.Parent is SwipeView swipeView &&
            swipeView.BindingContext is ProductModel product)
        {
            product.CompanyId = (long)appControl.CompanyInfo.company_id;
            await AppNavigatorService.NavigateTo(nameof(CompanyEditProductPage), new Dictionary<string, object>
            {
                ["ProductModel"] = product
            });
        }
    }

    private async void NoActiveProduct_Invoked(object sender, EventArgs e)
    {
        bool isWifiOn = await appControl.CheckWifi();
		if (!isWifiOn) return;
        
        if (sender is SwipeItem swipeItem &&
            swipeItem.Parent is SwipeItems swipeItems &&
            swipeItems.Parent is SwipeView swipeView &&
            swipeView.BindingContext is ProductModel product)
        {

            bool answer = await AlertService.ShowConfirmationAsync(
                                AppResource.Confirm,
                                AppResource.MessageConfirm,
                                AppResource.Yes, AppResource.No);

            if (!answer) return;

            try
            {
                viewModel.IsLoading = true;
                ChangePosterDeletionRequest request = new ChangePosterDeletionRequest()
                {
                    poster_id = product.PromotionId,
                    deleted = true
                };

                Response response = await companyApiService.ChangePosterDeletionStatus(request);
                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    viewModel.Products.Remove(product);
                    viewModel.UpdateTitle();
                    appControl.RefreshCompanyProfilePage = true;
                }
                else
                {
                    await AlertService.ShowAlertAsync(AppResource.Error, response.resultMsg);
                }
            }
            catch (Exception ex)
            {
                await AlertService.ShowAlertAsync(AppResource.Error, ex.Message);
            }
            finally
            {
                viewModel.IsLoading = false;
            }
        }
    }

    private async void Add_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(sender as Image);
        await AppNavigatorService.NavigateTo(nameof(CompanyAddProductPage));
    }
}