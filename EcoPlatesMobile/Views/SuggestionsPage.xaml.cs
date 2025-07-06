using EcoPlatesMobile.Models.Requests.Company;
using EcoPlatesMobile.Models.Requests.User;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Views;

[QueryProperty(nameof(IsUser), nameof(IsUser))]
public partial class SuggestionsPage : BasePage
{
    private bool _isUser = false;
    public bool IsUser
    {
        get => _isUser;
        set
        {
            _isUser = value;
        }
    }

    public List<string> FeedbackTypes { get; set; } = new() { AppResource.Suggestions, AppResource.Complaints };
    public string SelectedType { get; set; }

    private UserSessionService userSessionService;
    private AppControl appControl;
    private UserApiService userApiService;
    private CompanyApiService companyApiService;

    public SuggestionsPage(UserSessionService userSessionService, AppControl appControl, UserApiService userApiService, CompanyApiService companyApiService)
	{
		InitializeComponent();

        this.userSessionService = userSessionService;
        this.appControl = appControl;
        this.userApiService = userApiService;
        this.companyApiService = companyApiService;

        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (userSessionService.Role == UserRole.User)
        {
            header.HeaderBackground = btnSubmit.BackgroundColor = Colors.Green;
            imFeedBack.Source = "user_feedback_icon.png";
            loading.ChangeColor(Colors.Green);
        }
        else
        {
            header.HeaderBackground = btnSubmit.BackgroundColor = Color.FromArgb("#8338EC");
            imFeedBack.Source = "company_feedback_icon.png";
            loading.ChangeColor(Color.FromArgb("#8338EC"));
        }
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SelectedType) || string.IsNullOrWhiteSpace(messageEditor.Text))
        {
            await AlertService.ShowAlertAsync(AppResource.MissingInfo, AppResource.MessageMissingInfo, AppResource.Ok);
            return;
        }

        try
        {
            loading.ShowLoading = true;
            if (userSessionService.Role == UserRole.User)
            {
                UserFeedbackInfoRequest request = new UserFeedbackInfoRequest()
                {
                    user_id = appControl.UserInfo.user_id,
                    feedback_text = messageEditor.Text,
                    feedback_type = SelectedType.ToUpper(),
                };
 
                Response response = await userApiService.RegisterUserFeedBack(request);

                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    await AlertService.ShowAlertAsync(AppResource.ThankYou, AppResource.MessageFeedback, AppResource.Close);
                    await AppNavigatorService.NavigateTo("..");
                }
                else
                {
                    await AlertService.ShowAlertAsync(AppResource.Error, response.resultMsg, AppResource.Close);
                }
            }
            else
            {
                CompanyFeedbackInfoRequest request = new CompanyFeedbackInfoRequest()
                {
                    company_id = appControl.CompanyInfo.company_id,
                    feedback_text = messageEditor.Text,
                    feedback_type = SelectedType.ToUpper(),
                };
 
                Response response = await companyApiService.RegisterCompanyFeedBack(request);

                if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
                {
                    await AlertService.ShowAlertAsync(AppResource.ThankYou, AppResource.MessageFeedback, AppResource.Close);
                    await AppNavigatorService.NavigateTo("..");
                }
                else
                {
                    await AlertService.ShowAlertAsync(AppResource.Error, response.resultMsg, AppResource.Close);
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
}