using System.ComponentModel;
using EcoPlatesMobile.Models.Chat;
using EcoPlatesMobile.Models.Responses.Notification;
using EcoPlatesMobile.Models.Responses.User;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.ViewModels.User;
using EcoPlatesMobile.Views.Chat;
using Newtonsoft.Json.Linq;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserMainPage : BasePage
{
    Components.TypeItem typeItem = null;

    private UserMainPageViewModel viewModel;
    private AppControl appControl;

    public UserMainPage(UserMainPageViewModel vm, AppControl appControl)
	{
		InitializeComponent();
        this.viewModel = vm;
        this.appControl = appControl;
        
        viewModel.PropertyChanged += ViewModel_PropertyChanged;
        companyTypeList.EventTypeClick += CompanyTypeList_EventTypeClick;

        appControl.NotificationSubscriber = this;

#if ANDROID
        MessagingCenter.Subscribe<MainActivity, NotificationData>(appControl.NotificationSubscriber, Constants.NOTIFICATION_BODY, OnNotificationReceived);
#endif
        BindingContext = viewModel;
    }
     
    protected override async void OnAppearing()
	{
		base.OnAppearing();

        Shell.SetTabBarIsVisible(this, true);
 
        bool isWifiOn = await appControl.CheckWifi();
		if (!isWifiOn) return;
        
        if (typeItem == null)
        {
            viewModel.BusinessType = BusinessType.SUPERMARKET;
        }
        else
        {
            viewModel.BusinessType = typeItem.Type;
            companyTypeList.InitType(typeItem);
        }

        lbHeader.Text = $"{AppResource.NearbyWithin} {appControl.UserInfo.radius_km} km {AppResource.Around}.";

        if (appControl.NotificationData != null && appControl.NotificationData.body != string.Empty)
        {
            await MoveToPageUsingNotification(appControl.NotificationData);
        }
        else if (appControl.RefreshMainPage)
        {
            await viewModel.LoadInitialAsync();
            appControl.RefreshMainPage = false;
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
            case NotificationType.NEW_POSTER:
                var posterData = jObject.ToObject<NewPosterPushNotificationResponse>();
                var product = new ProductModel
                {
                    PromotionId = posterData.promotion_id
                };
                
                await Shell.Current.GoToAsync(nameof(DetailProductPage), new Dictionary<string, object>
                {
                    ["ProductModel"] = product
                });
                break;

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

    private async void CompanyTypeList_EventTypeClick(Components.TypeItem item)
    {
        bool isWifiOn = await appControl.CheckWifi();
        if (!isWifiOn) return;

        typeItem = item;
        viewModel.BusinessType = item.Type;
        await viewModel.LoadInitialAsync();
    }

    private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(viewModel.ShowLikedView) && viewModel.ShowLikedView)
        {
            await likeView.DisplayAsAnimation();
            viewModel.ShowLikedView = false;
        }
    }

    private async void Search_Tapped(object sender, TappedEventArgs e)
    {
        await AppNavigatorService.NavigateTo(nameof(UserMainSearchPage));
    }
}