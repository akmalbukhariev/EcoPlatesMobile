using System.Text;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Views;
 
public partial class DeleteAccountPage : BasePage
{
    private UserApiService userApiService;
    private CompanyApiService companyApiService;

    public DeleteAccountPage(UserSessionService userSessionService, AppControl appControl, UserApiService userApiService, CompanyApiService companyApiService)
    {
        InitializeComponent();

        this.userSessionService = userSessionService;
        this.appControl = appControl;
        this.userApiService = userApiService;
        this.companyApiService = companyApiService;
        
        EnableTap(RowTooExpensive, CbTooExpensive);
        EnableTap(RowNotEnoughValue, CbNotEnoughValue);
        EnableTap(RowNotEnoughOffers, CbNotEnoughOffers);
        EnableTap(RowHardToUse, CbHardToUse);
        EnableTap(RowNoTime, CbNoTime);
        EnableTap(RowPreferOther, CbPreferOther);
        EnableTap(RowTechnicalIssues, CbTechnicalIssues);
        EnableTap(RowOther, CbOther);
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (userSessionService.Role == UserRole.User)
        {
            header.HeaderBackground = Constants.COLOR_USER;
            CbTooExpensive.Color = Constants.COLOR_USER;
            CbNotEnoughValue.Color = Constants.COLOR_USER;
            CbNotEnoughOffers.Color = Constants.COLOR_USER;
            CbHardToUse.Color = Constants.COLOR_USER;
            CbNoTime.Color = Constants.COLOR_USER;
            CbPreferOther.Color = Constants.COLOR_USER;
            CbTechnicalIssues.Color = Constants.COLOR_USER;
            CbOther.Color = Constants.COLOR_USER;
        }
        else
        {
            header.HeaderBackground = Constants.COLOR_COMPANY;
            CbTooExpensive.Color = Constants.COLOR_COMPANY;
            CbNotEnoughValue.Color = Constants.COLOR_COMPANY;
            CbNotEnoughOffers.Color = Constants.COLOR_COMPANY;
            CbHardToUse.Color = Constants.COLOR_COMPANY;
            CbNoTime.Color = Constants.COLOR_COMPANY;
            CbPreferOther.Color = Constants.COLOR_COMPANY;
            CbTechnicalIssues.Color = Constants.COLOR_COMPANY;
            CbOther.Color = Constants.COLOR_COMPANY;
        }
    }
   
    private string BuildReasonsText()
    {
        var parts = new List<string>();

        void Add(bool condition, string code)
        {
            if (condition) parts.Add(code);
        }

        Add(CbTooExpensive.IsChecked, "TE");
        Add(CbNotEnoughValue.IsChecked, "NEV");
        Add(CbNotEnoughOffers.IsChecked, "NEO");
        Add(CbHardToUse.IsChecked, "HTU");
        Add(CbNoTime.IsChecked, "NT");
        Add(CbPreferOther.IsChecked, "PO");
        Add(CbTechnicalIssues.IsChecked, "TI");

        if (CbOther.IsChecked)
        {
            var other = (OtherEntry.Text ?? "").Trim();

            // keep it short to avoid URL limits
            other = other.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
            if (other.Length > 80) other = other.Substring(0, 80);

            parts.Add(string.IsNullOrWhiteSpace(other) ? "O" : $"O:{other}");
        }

        return string.Join("|", parts);
    }

    private void EnableTap(Grid row, CheckBox checkBox)
    {
        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, __) =>
        {
            checkBox.IsChecked = !checkBox.IsChecked;
        };
        row.GestureRecognizers.Add(tap);
    }
    
    private void CbOther_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        OtherEntry.IsVisible = e.Value;
        OtherEntryBackground.IsVisible = e.Value;

        if (!e.Value)
            OtherEntry.Text = string.Empty;
    }

    private async void DeleteButton_Clicked(object sender, EventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            var reasons = BuildReasonsText();

            if (string.IsNullOrWhiteSpace(reasons))
            {
                await DisplayAlert(
                    AppResource.DeleteAccount_Alert_SelectReason_Title,
                    AppResource.DeleteAccount_Alert_SelectReason_Message,
                    AppResource.Ok);
                return;
            }

            if (CbOther.IsChecked && string.IsNullOrWhiteSpace(OtherEntry.Text))
            {
                await DisplayAlert(
                    AppResource.DeleteAccount_Alert_Other_Title,
                    AppResource.DeleteAccount_Alert_Other_Message,
                    AppResource.Ok);
                return;
            }

            var confirm = await DisplayAlert(
                    AppResource.DeleteAccount_Confirm_Title,
                    AppResource.DeleteAccount_Confirm_Message,
                    AppResource.DeleteAccount_Confirm_YesDelete,
                    AppResource.Cancel);

            if (!confirm) return;

            loading.ShowLoading = true;
            if (userSessionService.Role == UserRole.User)
            {
                await userApiService.DeleteUseAccount(reasons);
            }
            else
            {
                await companyApiService.DeleteUseAccount(reasons);
            }
            loading.ShowLoading = false; 
            
            await DisplayAlert(
                    AppResource.DeleteAccount_Submitted_Title,AppResource.ThankYou,
                    AppResource.Ok);

            loading.ShowLoading = true;
            if (userSessionService.Role == UserRole.User)
            {
                await appControl.LogoutUser(false);
            }
            else
            {
                await appControl.LogoutCompany(false);
            }
            loading.ShowLoading = false; 
        });
    }
}