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
using ViewExt = Microsoft.Maui.Controls.ViewExtensions;

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
                if (messageData.sender_type == UserRole.Company.ToString().ToUpper()) return;
                
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
            product.IsThisActivePage = true;
            product.CompanyId = (long)appControl.CompanyInfo.company_id;
            await AppNavigatorService.NavigateTo(nameof(CompanyEditProductPage), new Dictionary<string, object>
            {
                ["ProductModel"] = product
            });
        }
    }

    private async void InActiveProduct_Invoked(object sender, EventArgs e)
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

                bool isOk = await appControl.CheckUserState(response);
                if (!isOk)
                {
                    await appControl.LogoutCompany();
                    return;
                }

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

    private async void InActiveAll_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(sender as Image);

        bool answer = await AlertService.ShowConfirmationAsync(
                                AppResource.Confirm,
                                AppResource.MessageDeactiveAllProducts,
                                AppResource.Yes, AppResource.No);

        if (!answer) return;

        try
        {
            viewModel.IsLoading = true;

            var selected = viewModel.Products.Where(p => p.IsCheckedProduct).ToList();

            var request = new ChangePosterDeletionListRequest
            {
                dataList = selected.Select(p => new ChangePosterDeletionRequest
                {
                    poster_id = p.PromotionId,
                    deleted = true
                }).ToList()
            };

            Response response = await companyApiService.ChangePosterDeletionStatusList(request);
            bool isOk = await appControl.CheckUserState(response);
            if (!isOk)
            {
                await appControl.LogoutCompany();
                return;
            }

            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    foreach (var item in selected)
                        viewModel.Products.Remove(item);
                });

                appControl.RefreshCompanyProfilePage = true;
                StCheckProductTapped(null, null);

                if (viewModel.Products.Count == 0)
                    viewModel.IsShowChekProduct = false;

                //await AlertService.ShowAlertAsync(AppResource.InactiveProducts, AppResource.Success);
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

    private async void Add_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            await AnimateElementScaleDown(sender as Image);
            await AppNavigatorService.NavigateTo(nameof(CompanyAddProductPage));
        });
    }

    private async void StCheckProductTapped(object sender, TappedEventArgs e)
    {
        checkProduct.IsChecked = !checkProduct.IsChecked;
        viewModel.ShowCheckProduct(checkProduct.IsChecked);

        viewModel.checkAllCheckedAlready = true;
        viewModel.IsCheckedAllProduct = false;
        viewModel.checkAllCheckedAlready = false;

        await AnimateSelectAllBarAsync(selectAllBar, checkProduct.IsChecked);
    }

    private async void StSelectAllProductTapped(object sender, TappedEventArgs e)
    {
        viewModel.checkAllCheckedAlready = true;
        checkAllProducts.IsChecked = !checkAllProducts.IsChecked;
        CheckAllProducts(checkAllProducts.IsChecked);
        viewModel.checkAllCheckedAlready = false;
    }

    private async void CheckProduct_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        viewModel.StackBottomEnabled = false;
        viewModel.ShowCheckProduct(checkProduct.IsChecked);
        if (checkProduct.IsChecked && currentlyOpenSwipeView != null)
        {
            currentlyOpenSwipeView.Close();
        }

        await AnimateSwapAsync(showAdd: !e.Value);
        viewModel.IsShowChekAllProducts = e.Value;

        viewModel.checkAllCheckedAlready = true;
        viewModel.IsCheckedAllProduct = false;
        viewModel.checkAllCheckedAlready = false;

        await AnimateSelectAllBarAsync(selectAllBar, checkProduct.IsChecked);
    }

    private async void CheckAllProduct_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (viewModel.checkAllCheckedAlready) return;

        CheckAllProducts(checkAllProducts.IsChecked);
    }

    private void CheckAllProducts(bool check)
    {
        if (viewModel?.Products == null) return;

        bool anyChecked = false;

        foreach (var product in viewModel.Products)
        {
            product.IsCheckedProduct = check;
            product.IsNonActiveProduct = check;

            if (product.IsCheckedProduct) anyChecked = true;
        }

        viewModel.StackBottomEnabled = anyChecked;
        viewModel.InActiveImage = anyChecked ? "inactive1.png" : "inactive_gray1.png";
    } 
    
    bool isAnimating;
    async Task AnimateSwapAsync(bool showAdd)
    {
        if (isAnimating) return;
        isAnimating = true;

        const uint duration = 220;
        var easing = Easing.SinOut;

        if (showAdd)
        {
            imAdd.IsVisible = true;

            imAdd.TranslationX = 60;
            imAdd.Opacity = 0;

            var fadeInAdd = imAdd.FadeTo(1, duration, easing);
            var slideInAdd = imAdd.TranslateTo(0, 0, duration, easing);

            var fadeOutInactive = imInactive.FadeTo(0, duration, easing);
            var slideOutInactive = imInactive.TranslateTo(60, 0, duration, easing);

            await Task.WhenAll(fadeInAdd, slideInAdd, fadeOutInactive, slideOutInactive);

            imInactive.IsVisible = false;
        }
        else
        {
            imInactive.IsVisible = true;

            imInactive.TranslationX = 60;
            imInactive.Opacity = 0;

            var fadeInInactive = imInactive.FadeTo(1, duration, easing);
            var slideInInactive = imInactive.TranslateTo(0, 0, duration, easing);

            var fadeOutAdd = imAdd.FadeTo(0, duration, easing);
            var slideOutAdd = imAdd.TranslateTo(60, 0, duration, easing);

            await Task.WhenAll(fadeInInactive, slideInInactive, fadeOutAdd, slideOutAdd);

            imAdd.IsVisible = false;
        }

        isAnimating = false;
    }
    
    private SwipeView currentlyOpenSwipeView;
    void OnRowSwipe(object sender, EventArgs e)
    {
        var swipeView = (SwipeView)sender;

        if (currentlyOpenSwipeView != null && currentlyOpenSwipeView != swipeView)
        {
            currentlyOpenSwipeView.Close();
        }

        if (!viewModel.AllowSwipe)
            ((SwipeView)sender).Close();

        currentlyOpenSwipeView = swipeView;
    }
}