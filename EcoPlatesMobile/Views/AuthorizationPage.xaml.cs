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

	private IDispatcherTimer timer;
	private TimeSpan elapsed;
	private VerifyPhoneNumberResponse response;

	private bool _initialSent = false;                       // send once per page instance	
	private DateTimeOffset? _sentAt = null;                  // when last SMS was sent
	private static readonly TimeSpan Cooldown = TimeSpan.FromSeconds(59);

	public AuthorizationPage(UserSessionService userSessionService, CompanyApiService companyApiService, UserApiService userApiService, AppControl appControl, IKeyboardHelper keyboardHelper, MessageApiService messageApiService)
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
			header.HeaderBackground = btnNext.BackgroundColor = Constants.COLOR_USER;
		}
		else
		{
			loading.ChangeColor(Constants.COLOR_COMPANY);
		}

		this.Loaded += (s, e) =>
		{
			pinView.Focus();
		};

		timer = Application.Current.Dispatcher.CreateTimer();
		timer.Interval = TimeSpan.FromSeconds(1);
		timer.Tick += OnTick;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		lbTitle.Text = $"{AppResource.Please}, {_phoneNumber} {AppResource.ConfirmTitle}";

		if (!_initialSent)
		{
			await SendVerificationCode();
			_initialSent = true;
			_sentAt = DateTimeOffset.UtcNow;
			ShowTimeAndStart();                 // start fresh countdown once
		}
		else
		{
			RestoreCooldownUI();                // just restore UI; don't resend
		}
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		StopTimer();
	}

	private async void LabelSend_Tapped(object sender, TappedEventArgs e)
	{
		await AnimateElementScaleDown(lbSend);
		
		_sentAt = DateTimeOffset.UtcNow;        // mark the new send time
		ShowTimeAndStart();                     // show timer first for snappy UX
		await SendVerificationCode();           // then actually send
	}

	private async Task SendVerificationCode()
	{
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

	private void ShowTimeAndStart()
	{
		// start from 00:01 immediately
		elapsed = TimeSpan.FromSeconds(1);
		lblTimer.Text = elapsed.ToString(@"mm\:ss");

		stackTime.IsVisible = true;
		stackSend.IsVisible = false;

		if (!timer.IsRunning)
			timer.Start();
	}

	private void StopTimer()
	{
		if (timer.IsRunning)
			timer.Stop();
	}

	private void OnTick(object? sender, EventArgs e)
	{
		elapsed = elapsed.Add(TimeSpan.FromSeconds(1));
		lblTimer.Text = elapsed.ToString(@"mm\:ss");

		// When 00:59 reached -> hide timer, show resend
		if (elapsed >= TimeSpan.FromSeconds(59))
		{
			StopTimer();
			stackTime.IsVisible = false;
			stackSend.IsVisible = true;
		}
	}

	private async void ButtonNext_Clicked(object sender, EventArgs e)
	{
		keyboardHelper.HideKeyboard();

		bool isWifiOn = await appControl.CheckWifi();
		if (!isWifiOn) return;
 
		if (!CheckVerificationCode())
		{
			await AlertService.ShowAlertAsync("Verification number", "Verification code is wrong!");
			return;
		}

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
	}

	private void RestoreCooldownUI()
	{
		if (_sentAt is null)
		{
			// Never sent on this instance → show resend
			stackTime.IsVisible = false;
			stackSend.IsVisible = true;
			return;
		}

		var elapsedSinceSend = DateTimeOffset.UtcNow - _sentAt.Value;

		if (elapsedSinceSend >= Cooldown)
		{
			// Cooldown finished → show resend
			stackTime.IsVisible = false;
			stackSend.IsVisible = true;
			StopTimer();
		}
		else
		{
			// Continue the existing countdown
			ShowTimeAndStart(elapsedSinceSend);
		}
	}

	private void ShowTimeAndStart(TimeSpan? alreadyElapsed = null)
	{
		// if resuming, use the passed elapsed; otherwise start from 00:01
		elapsed = alreadyElapsed.HasValue
			? alreadyElapsed.Value
			: TimeSpan.FromSeconds(1);

		if (elapsed < TimeSpan.FromSeconds(1))
			elapsed = TimeSpan.FromSeconds(1);

		if (elapsed > Cooldown)
			elapsed = Cooldown;

		lblTimer.Text = elapsed.ToString(@"mm\:ss");

		stackTime.IsVisible = true;
		stackSend.IsVisible = false;

		if (!timer.IsRunning)
			timer.Start();
	}
    
	private bool CheckVerificationCode()
	{
		if (string.IsNullOrEmpty(pinView.PINValue)) return false;

		return string.Equals(pinView.PINValue, verificationCode);
	}
}