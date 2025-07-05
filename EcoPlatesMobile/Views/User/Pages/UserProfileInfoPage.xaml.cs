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
    public UserProfileInfoPage(AppControl appControl, UserApiService userApiService)
	{
		InitializeComponent();

        this.appControl = appControl;
        this.userApiService = userApiService;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        //UserInfo info = appControl.UserInfo;
        imUser.Source = appControl.UserInfo.profile_picture_url;
        fullImage.Source = appControl.UserInfo.profile_picture_url;
        entryUserName.Text = appControl.UserInfo.first_name;
        lbPhoneNumber.Text = appControl.UserInfo.phone_number;
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
        string action = await DisplayActionSheet(AppResource.ChooseOption,
                                                 AppResource.Cancel,
                                                 null,
                                                 AppResource.SelectGallery,
                                                 AppResource.TakePhoto);

        FileResult result = null;

        if (action == AppResource.SelectGallery)
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                result = await MediaPicker.PickPhotoAsync();
            }
        }
        else if (action == AppResource.TakePhoto)
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                result = await MediaPicker.CapturePhotoAsync();
            }
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
    }

    private async void Cancel_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..", true);
    }

    private async void Done_Clicked(object sender, EventArgs e)
    {
        try
        {
            string enteredName = entryUserName.Text?.Trim();
            if (string.IsNullOrEmpty(enteredName))
            {
                await AlertService.ShowAlertAsync(AppResource.UpdateProfile,AppResource.EnterName);
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
                //var apiService = AppService.Get<UserApiService>();
                Response response = await userApiService.UpdateUserProfileInfo(imageStream, additionalData);

                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    appControl.RefreshUserProfilePage = true;

                    await AlertService.ShowAlertAsync(AppResource.UpdateProfile,AppResource.Success);
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
    }

    private async void PhoneNumber_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(grdPhoneNumber);
        await AppNavigatorService.NavigateTo(nameof(PhoneNumberChangePage));
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

    private void OnImage_Tapped(object sender, TappedEventArgs e)
    {
        boxFullImage.IsVisible = false;
        fullImage.IsVisible = false;
    }

    private async void ButtonLogOut_Clicked(object sender, EventArgs e)
    {
        bool answer = await AlertService.ShowConfirmationAsync(
                                AppResource.Confirm,
                                AppResource.MessageConfirm,
                                AppResource.Yes,
                                AppResource.No);
        if (!answer) return;

        loading.ShowLoading = true;
        await appControl.LogoutUser();
        loading.ShowLoading = false;
    }
}