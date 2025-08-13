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
    private LocationService locationService;
    private IKeyboardHelper keyboardHelper;

    public UserRegistrationPage(UserApiService userApiService, AppControl appControl, LocationService locationService, IKeyboardHelper keyboardHelper)
    {
        InitializeComponent();

        this.userApiService = userApiService;
        this.appControl = appControl;
        this.locationService = locationService;
        this.keyboardHelper = keyboardHelper;

        entryName.SetMaxLength(20);

        this.Loaded += (s, e) =>
        {
            entryName.Focus();
        };
    }

    private async void ButtonNext_Clicked(object sender, EventArgs e)
    {
        keyboardHelper.HideKeyboard();
        
        bool isWifiOn = await appControl.CheckWifi();
		if (!isWifiOn) return;
        
        try
        {
            string name = entryName.GetEntryText();
            if (string.IsNullOrEmpty(name))
            {
                await AlertService.ShowAlertAsync(AppResource.Failed, AppResource.PleaseEnterName);
                return;
            }

            loading.ShowLoading = true;

            Location location = await locationService.GetCurrentLocationAsync();
            if (location == null) return;

            RegisterUserRequest request = new RegisterUserRequest()
            {
                first_name = name,
                phone_number = _phoneNumber,
                location_latitude = location.Latitude,
                location_longitude = location.Longitude
            };

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