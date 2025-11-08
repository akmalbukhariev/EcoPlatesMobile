using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.User;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Views.User.Pages;
 
public partial class UserProfileInfoPage : BasePage
{ 
    private Stream? imageStream = null;
    private bool isNewImageSelected = false;

    private AppControl appControl;
    private UserApiService userApiService;
    private IKeyboardHelper keyboardHelper;

    private bool isPageLoaded = false;

    public UserProfileInfoPage(AppControl appControl, UserApiService userApiService, IKeyboardHelper keyboardHelper)
    {
        InitializeComponent();

        this.appControl = appControl;
        this.userApiService = userApiService;
        this.keyboardHelper = keyboardHelper;

        entryUserName.MaxLength = 20;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        imUser.Source = appControl.GetImageUrlOrFallback(appControl.UserInfo.profile_picture_url);
        fullImage.Source = imUser.Source;
        entryUserName.Text = appControl.UserInfo.first_name;
        lbPhoneNumber.Text = appControl.UserInfo.phone_number;
        //notification.IsToggled = appControl.UserInfo.notification_enabled;
        ChangeNotification(appControl.UserInfo.notification_enabled);
          
        isPageLoaded = true;
    }

    private async void BorderImage_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(borderUserImage);

        fullImage.TranslationY = -100;
        fullImage.Opacity = 0;
        fullImage.IsVisible = true;
        boxFullImage.IsVisible = true;

        await Task.WhenAll(
            fullImage.TranslateTo(0, 0, 250, Easing.SinIn),
            fullImage.FadeTo(1, 250, Easing.SinIn)
        );
    }

    private async void ChangeImage_Clicked(object sender, EventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            string action = await DisplayActionSheet(AppResource.ChooseOption,
                                                     AppResource.Cancel,
                                                     null,
                                                     AppResource.SelectGallery,
                                                     AppResource.TakePhoto);

            FileResult result = null;

            if (action == AppResource.SelectGallery)
            {
                if (!await appControl.EnsureGalleryPermissionAsync())
                    return;

                result = await appControl.TryPickPhotoAsync();
            }
            else if (action == AppResource.TakePhoto)
            {
                if (!await appControl.EnsureCameraPermissionAsync())
                    return;

                result = await appControl.TryCapturePhotoAsync();
            }

            if (result != null)
            {
                string localFilePath = Path.Combine(FileSystem.CacheDirectory, result.FileName);

                using (Stream sourceStream = await result.OpenReadAsync())
                using (FileStream localFileStream = File.Create(localFilePath))
                {
                    await sourceStream.CopyToAsync(localFileStream);
                }

                imUser.Source = ImageSource.FromFile(localFilePath);
                fullImage.Source = imUser.Source;

                imageStream = File.OpenRead(localFilePath);

                isNewImageSelected = true;
            }
        });
    }

    private async void Cancel_Clicked(object sender, EventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            await Shell.Current.GoToAsync("..", true);
        });
    }

    private async void Done_Clicked(object sender, EventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            keyboardHelper.HideKeyboard();

            bool isWifiOn = await appControl.CheckWifiOrNetwork();
            if (!isWifiOn) return;

            try
            {
                string enteredName = entryUserName.Text?.Trim();
                if (string.IsNullOrEmpty(enteredName))
                {
                    await AlertService.ShowAlertAsync(AppResource.UpdateProfile, AppResource.EnterName);
                    return;
                }

                UserInfo userInfo = appControl.UserInfo;

                bool isSame = enteredName == userInfo.first_name?.Trim();

                if (!isSame || isNewImageSelected)
                {
                    var additionalData = new Dictionary<string, string>
                    {
                        { "user_id", userInfo.user_id.ToString() },
                        { "first_name", enteredName },
                    };

                    if (!isNewImageSelected)
                    {
                        imageStream = null;
                    }

                    loading.ShowLoading = true;
                    Response response = await userApiService.UpdateUserProfileInfo(imageStream, additionalData);
                    bool isOk = await appControl.CheckUserState(response);
                    if (!isOk)
                    {
                        await appControl.LogoutUser();
                        return;
                    }
                    
                    if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                    {
                        appControl.RefreshUserProfilePage = true;

                        await AlertService.ShowAlertAsync(AppResource.UpdateProfile, AppResource.Success);
                        await Shell.Current.GoToAsync("..", true);
                    }
                    else
                    {
                        await AlertService.ShowAlertAsync(AppResource.Error, response.resultMsg);
                    }
                }
                else
                {
                    await Shell.Current.GoToAsync("..", true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                loading.ShowLoading = false;
            }
        });
    }

    private async void PhoneNumber_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            await AnimateElementScaleDown(grdPhoneNumber);
            await AppNavigatorService.NavigateTo(nameof(PhoneNumberChangePage));
        });
    }

    private async void OnImage_Swiped(object sender, SwipedEventArgs e)
    {
        await Task.WhenAll(
            fullImage.TranslateTo(0, -100, 250, Easing.SinOut),
            fullImage.FadeTo(0, 250, Easing.SinOut)
        );

        boxFullImage.IsVisible = false;
        fullImage.IsVisible = false;
        fullImage.Opacity = 1;
        fullImage.TranslationY = 0;
    }

    private async void OnImage_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            boxFullImage.IsVisible = false;
            fullImage.IsVisible = false;
        });
    }

    /*private bool _suppressToggle;
    private async void Notitifation_Toggled(object sender, ToggledEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            if (_suppressToggle || !isPageLoaded) return;

            keyboardHelper.HideKeyboard();

            if (e.Value)
            {
                bool allowed = await NotificationPermissionHelper.EnsureEnabledAsync(this);
                if (!allowed)
                {
                    _suppressToggle = true;
                    notification.IsToggled = false;
                    _suppressToggle = false;
                    return;
                }
            }

            bool isWifiOn = await appControl.CheckWifiOrNetwork();
            if (!isWifiOn) return;

            try
            {
                var additionalData = new Dictionary<string, string>
                {
                    { "user_id", appControl.UserInfo.user_id.ToString() },
                    { "notification_enabled", notification.IsToggled.ToString() }
                };

                loading.ShowLoading = true;

                Response response = await userApiService.UpdateUserProfileInfo(null, additionalData);
                bool isOk = await appControl.CheckUserState(response);
                if (!isOk)
                {
                    await appControl.LogoutUser();
                    return;
                }
                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    appControl.RefreshUserProfilePage = true;
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
                loading.ShowLoading = false;
            }
        });
    }*/

    private async void Notification_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            keyboardHelper.HideKeyboard();

            bool allowed = await NotificationPermissionHelper.EnsureEnabledAsync(this);
            if (!allowed)
            {
                ChangeNotification(false);
                return;
            }

            bool isWifiOn = await appControl.CheckWifiOrNetwork();
            if (!isWifiOn) return;

            ChangeNotification(!notification_on);
            try
            {
                var additionalData = new Dictionary<string, string>
                {
                    { "user_id", appControl.UserInfo.user_id.ToString() },
                    { "notification_enabled", notification_on.ToString() }
                };

                loading.ShowLoading = true;

                Response response = await userApiService.UpdateUserProfileInfo(null, additionalData);
                bool isOk = await appControl.CheckUserState(response);
                if (!isOk)
                {
                    await appControl.LogoutUser();
                    return;
                }
                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    appControl.RefreshUserProfilePage = true;
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
                loading.ShowLoading = false;
            }
        });
    }

    private bool notification_on = false;
    private void ChangeNotification(bool on)
    {
        notification_on = on;

        if (on)
        {
            notification.Source = "user_on.png";
        }
        else
        {
            notification.Source = "off.png";
        }
    }

    private async void ButtonLogOut_Clicked(object sender, EventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            bool isWifiOn = await appControl.CheckWifiOrNetwork();
            if (!isWifiOn) return;

            bool answer = await AlertService.ShowConfirmationAsync(
                                    AppResource.Confirm,
                                    AppResource.MessageConfirm,
                                    AppResource.Yes,
                                    AppResource.No);
            if (!answer) return;

            loading.ShowLoading = true;
            await appControl.LogoutUser(false);
            loading.ShowLoading = false;
        });
    }
}