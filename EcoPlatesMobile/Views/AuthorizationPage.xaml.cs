using System.Threading.Tasks;
using EcoPlatesMobile.Models.Chat;
using EcoPlatesMobile.Models.Requests;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Views.Company.Pages;
using EcoPlatesMobile.Views.User.Pages;

namespace EcoPlatesMobile.Views;

[QueryProperty(nameof(PhoneNumber), nameof(PhoneNumber))]
public partial class AuthorizationPage : BasePage
{
	private string _phoneNumber;
	public string PhoneNumber
	{
		set
		{
			_phoneNumber = value;
		}
	}

	private string verificationCode = "";
	private CompanyApiService companyApiService;
	private UserApiService userApiService;
	private UserSessionService userSessionService;
	private AppControl appControl;
	private IKeyboardHelper keyboardHelper;
	private MessageApiService messageApiService;

	private VerifyPhoneNumberResponse response;
 
	private bool initialSent = false;                 // send once per page instance
	private bool sending = false;                     // re-entrancy guard
	private bool resendEnabled = true;                // UI state
	private DateTimeOffset? sentAt = null;            // last successful send time
	private TimeSpan remaining;                       // remaining cooldown
	private static readonly TimeSpan Cooldown = TimeSpan.FromSeconds(60);
	private IDispatcherTimer cooldownTimer;

	
	public AuthorizationPage(UserSessionService userSessionService,
							 CompanyApiService companyApiService,
							 UserApiService userApiService,
							 AppControl appControl,
							 IKeyboardHelper keyboardHelper,
							 MessageApiService messageApiService)
	{
		InitializeComponent();

		this.userSessionService = userSessionService;
		this.companyApiService = companyApiService;
		this.userApiService = userApiService;
		this.appControl = appControl;
		this.keyboardHelper = keyboardHelper;
		this.messageApiService = messageApiService;

		#region 
		// number1.NextEntry = number2;
		// number2.PreviousEntry = number1;
		// number2.NextEntry = number3;
		// number3.PreviousEntry = number2;
		// number3.NextEntry = number4;
		// number4.PreviousEntry = number3;
		#endregion

		if (userSessionService.Role == UserRole.User)
		{
			lbSend.TextColor = Constants.COLOR_COMPANY;
			header.HeaderBackground = btnNext.BackgroundColor = Constants.COLOR_USER;
			pinView.BoxFocusColor = Constants.COLOR_USER;
		}
		else
		{
			lbSend.TextColor = Constants.COLOR_USER;
			loading.ChangeColor(Constants.COLOR_COMPANY);
			pinView.BoxFocusColor = Constants.COLOR_COMPANY;
		}

		Loaded += (s, e) =>
		{
			pinView.Focus();
		};

		cooldownTimer = Application.Current.Dispatcher.CreateTimer();
		cooldownTimer.Interval = TimeSpan.FromSeconds(1);
		cooldownTimer.Tick += (_, __) =>
		{
			if (remaining > TimeSpan.Zero)
			{
				remaining -= TimeSpan.FromSeconds(1);
                lbSend.Text = AppResource.Resend + $"({remaining:mm\\:ss})";
			} 
			else
			{
				cooldownTimer.Stop();
				EnableResendUI();
			}
		};
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		lbTitle.Text = $"{AppResource.Please}, {_phoneNumber} {AppResource.ConfirmTitle}";

		if (!initialSent)
		{
			await SafeSendAsync();
			initialSent = true;
		}
		else
		{
			RestoreResendState();
		}
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		if (cooldownTimer.IsRunning) cooldownTimer.Stop();
	}

	private async void LabelSend_Tapped(object sender, TappedEventArgs e)
	{
		if (!resendEnabled || sending) return;

		await AnimateElementScaleDown(lbSend);
    	await SafeSendAsync();   
	}

	private async Task SafeSendAsync()
	{
		bool isWifiOn = await appControl.CheckWifiOrNetworkFor();
		if (!isWifiOn) return;	

		if (sending) return;
		sending = true;

		try
		{
			DisableResendUI(Cooldown);

			await SendVerificationCode();
			sentAt = DateTimeOffset.UtcNow;
		}
		catch
		{
			EnableResendUI();
			//throw;
		}
		finally
		{
			sending = false;
		}
	}
    
	private void DisableResendUI(TimeSpan forHowLong)
	{
		resendEnabled = false;
		remaining = forHowLong;

		lbSend.InputTransparent = true; 
		lbSend.Opacity = 0.5;
        lbSend.Text = AppResource.Resend + $"({remaining:mm\\:ss})";

        if (!cooldownTimer.IsRunning) cooldownTimer.Start();
	}

	private void EnableResendUI()
	{
		resendEnabled = true;

		lbSend.InputTransparent = false;
		lbSend.Opacity = 1;
		lbSend.Text = AppResource.Resend;
	}

	private void RestoreResendState()
	{
		if (sentAt is null) { EnableResendUI(); return; }

		var elapsed = DateTimeOffset.UtcNow - sentAt.Value;
		var left = Cooldown - elapsed;
		if (left <= TimeSpan.Zero)
		{
			EnableResendUI();
		}
		else
		{
			DisableResendUI(left);
		}
	}
   
	private async Task SendVerificationCode()
	{
		return;
		try
		{
			VerifyPhoneNumberRequest data = new VerifyPhoneNumberRequest()
			{
				phone_number = _phoneNumber
			};

			loading.ShowLoading = true;
			response = await messageApiService.VerifyNumber(data);
			if (response?.resultCode == ApiResult.SUCCESS.GetCodeToString())
			{
				verificationCode = response.resultData.code;
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

	private async void ButtonNext_Clicked(object sender, EventArgs e)
	{
		await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
			keyboardHelper.HideKeyboard();

			bool isWifiOn = await appControl.CheckWifiOrNetwork();
			if (!isWifiOn) return;

			/*if (!CheckVerificationCode())
			{
				await AlertService.ShowAlertAsync(AppResource.Info, AppResource.MessageVerificationNumber);
				return;
			}*/

			loading.ShowLoading = true;

			try
		{
			if (appControl.IsPhoneNumberRegisterPage)
			{
				if (userSessionService.Role == UserRole.Company)
				{
					if (userSessionService.IsCompanyRegistrated)
					{
						await appControl.LoginCompany(_phoneNumber);
					}
					else
					{
						appControl.LocationForRegister = null;
						await AppNavigatorService.NavigateTo($"{nameof(CompanyRegistrationPage)}?PhoneNumber={_phoneNumber}");
					}
				}
				else if (userSessionService.Role == UserRole.User)
				{
					if (userSessionService.IsUserRegistrated)
					{
						await appControl.LoginUser(_phoneNumber);
					}
					else
					{
						await AppNavigatorService.NavigateTo($"{nameof(UserRegistrationPage)}?PhoneNumber={_phoneNumber}");
					}
				}
			}
			else
			{
				Response response = null;
				if (userSessionService.Role == UserRole.User)
				{
					response = await userApiService.UpdateUserPhoneNumber(_phoneNumber);
					if (response?.resultCode == ApiResult.SUCCESS.GetCodeToString())
					{
						await AlertService.ShowAlertAsync(AppResource.Success, AppResource.MessageLoginAgain);

						var store = AppService.Get<AppStoreService>();
						store.Set(AppKeys.PhoneNumber, _phoneNumber);

						((App)Application.Current).ReloadAppShell();
					}
					else
						await AlertService.ShowAlertAsync(AppResource.Error, response?.resultMsg);
				}
				else if (userSessionService.Role == UserRole.Company)
				{
					response = await companyApiService.UpdateCompanyPhoneNumber(_phoneNumber);
					if (response?.resultCode == ApiResult.SUCCESS.GetCodeToString())
					{
						await AlertService.ShowAlertAsync(AppResource.Success, AppResource.MessageLoginAgain);

						var store = AppService.Get<AppStoreService>();
						store.Set(AppKeys.PhoneNumber, _phoneNumber);

						((App)Application.Current).ReloadAppShell();
					}
					else
						await AlertService.ShowAlertAsync(AppResource.Error, response?.resultMsg);
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
		finally
		{
			loading.ShowLoading = false;
		}
		});
	}
    
	private bool CheckVerificationCode()
	{
		if (string.IsNullOrEmpty(pinView.PINValue)) return false;

		return string.Equals(pinView.PINValue, verificationCode);
	}
}