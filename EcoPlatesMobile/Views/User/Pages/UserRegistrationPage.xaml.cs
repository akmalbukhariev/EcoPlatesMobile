using EcoPlatesMobile.Models.Requests.User;
using EcoPlatesMobile.Models.Responses;
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

    public UserRegistrationPage()
	{
		InitializeComponent();
	}

    private async void ButtonNext_Clicked(object sender, EventArgs e)
    {
        try
        {
            string name = entryName.GetEntryText();
            if (string.IsNullOrEmpty(name))
            {
                await AlertService.ShowAlertAsync("Field", "Please enter the name.");
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
            var apiService = AppService.Get<UserApiService>();
            Response response = await apiService.RegisterUser(request);
            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                await AlertService.ShowAlertAsync("Success", "Registration has been completed successfully.");
                await AppService.Get<AppControl>().LoginUser(_phoneNumber);
            }
            else
            {
                await AlertService.ShowAlertAsync("Error", response.resultMsg);
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