using EcoPlatesMobile.Models.Requests.User;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Views.User.Pages;

[QueryProperty(nameof(PhoneNumber), nameof(PhoneNumber))]
public partial class UserRegistrationPage : BasePage
{
    private string _phoneNumber;
    public string PhoneNumber
    {
        set
        {
            _phoneNumber = value;
        }
    }

    private UserApiService userApiService;
    private AppControl appControl;

    public UserRegistrationPage(UserApiService userApiService, AppControl appControl)
	{
		InitializeComponent();

        this.userApiService = userApiService;
        this.appControl = appControl;
	}

    private async void ButtonNext_Clicked(object sender, EventArgs e)
    {
        try
        {
            string name = entryName.GetEntryText();
            if (string.IsNullOrEmpty(name))
            {
                await AlertService.ShowAlertAsync(AppResource.Failed, AppResource.PleaseEnterName);
                return;
            }

            RegisterUserRequest request = new RegisterUserRequest()
            {
                first_name = name,
                phone_number = _phoneNumber,
                location_latitude = 37.518313,
                location_longitude = 126.724187
            };

            loading.ShowLoading = true;
            //var apiService = AppService.Get<UserApiService>();
            Response response = await userApiService.RegisterUser(request);
            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                await AlertService.ShowAlertAsync(AppResource.Success, AppResource.RegistrationCompleted);
                await appControl.LoginUser(_phoneNumber);
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
    }
}