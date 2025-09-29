using EcoPlatesMobile.Services;

namespace EcoPlatesMobile.Views;

public partial class BlockedPage : BasePage
{
	private readonly LanguageService languageService;
    private readonly AppControl appControl1;
    public BlockedPage(LanguageService languageService, AppControl appControl)
    {
        InitializeComponent();

        this.languageService = languageService;
        this.appControl1 = appControl;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        lbUntillBlock.Text = appControl1.StrBlockUntill;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        appControl1.StrBlockUntill = "";
    }

	private async void ContactSupport_Clicked(object sender, EventArgs e)
    {
        await AnimateElementScaleDown(sender as Button);
        await Launcher.OpenAsync("https://t.me/SaleTopTicketBot");
    }

	private async void OpenHelpCenter_Clicked(object sender, EventArgs e)
    {
        await AnimateElementScaleDown(sender as Button);
 
        string currentLang = languageService.GetCurrentLanguage();

        string url = "http://95.182.118.233/backend/help-center-uz.html";

        switch (currentLang.ToLower())
        {
            case "uz":
                url = "http://95.182.118.233/backend/help-center-uz.html";
                break;
            case "ru":
                url = "http://95.182.118.233/backend/help-center-ru.html";
                break;
            case "en":
                url = "http://95.182.118.233/backend/help-center-en.html";
                break;
        }

        await Launcher.OpenAsync(url);
    }
     
	private async void BackToLogin_Clicked(object sender, EventArgs e)
	{
		await AnimateElementScaleDown(sender as Button);

        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            await AppNavigatorService.NavigateTo("..");
        });
    }
}