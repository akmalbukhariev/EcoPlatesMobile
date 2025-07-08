using SkiaSharp;
using SkiaSharp.Views.Maui;
using Svg.Skia;
using EcoPlatesMobile.Services;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Views;

public partial class LoginPage : BasePage
{
    private SKSvg svg;
    private AppStoreService appStoreService;
    private AppControl appControl;
    private UserSessionService userSessionService;

    public LoginPage(AppStoreService appStoreService, AppControl appControl, UserSessionService userSessionService)
	{
		InitializeComponent();

        this.appStoreService = appStoreService;
        this.appControl = appControl;
        this.userSessionService = userSessionService;

        //LoadSvg();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        UserRole role = appStoreService.Get(AppKeys.UserRole, UserRole.None);
        bool isLoggedIn = appStoreService.Get(AppKeys.IsLoggedIn, false);
        string phoneNumber = appStoreService.Get(AppKeys.PhoneNumber, "");

        loading.ShowLoading = true;
        if (isLoggedIn)
        {
            if (role == UserRole.Company)
            {
                await appControl.LoginCompany(phoneNumber);
            }
            else if (role == UserRole.User)
            { 
                await appControl.LoginUser(phoneNumber);
            }
        }
        loading.ShowLoading = false;
    }

    /*
    private async void LoadSvg()
    {
        try
        {
            using Stream stream = await FileSystem.OpenAppPackageFileAsync("sale_top_logo.svg");

            svg = new SKSvg();
            svg.Load(stream);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                skCanvasView.InvalidateSurface();
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error loading SVG: {ex.Message}");
        }
    }
 
    private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        if (svg?.Picture == null)
        {
            Console.WriteLine("❌ SVG is not loaded correctly.");
            return;
        }

        // Get canvas size
        var info = e.Info;
        float canvasWidth = info.Width;
        float canvasHeight = info.Height;

        // Get SVG size
        var svgBounds = svg.Picture.CullRect;
        float svgWidth = svgBounds.Width;
        float svgHeight = svgBounds.Height;

        // Scale to fit within SKCanvasView size
        float scaleX = canvasWidth / svgWidth;
        float scaleY = canvasHeight / svgHeight;
        float scale = Math.Min(scaleX, scaleY); // Maintain aspect ratio

        // Center the SVG
        float xOffset = (canvasWidth - (svgWidth * scale)) / 2;
        float yOffset = (canvasHeight - (svgHeight * scale)) / 2;

        canvas.Save();
        canvas.Translate(xOffset, yOffset);
        canvas.Scale(scale);
        canvas.DrawPicture(svg.Picture);
        canvas.Restore();
    }
    */
    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (sender == btnComapny)
        {
            userSessionService.SetUser(UserRole.Company);
        }
        else if (sender == btnUser)
        {
            userSessionService.SetUser(UserRole.User);
        }

        await AppNavigatorService.NavigateTo(nameof(PhoneNumberRegisterPage));
    }
}