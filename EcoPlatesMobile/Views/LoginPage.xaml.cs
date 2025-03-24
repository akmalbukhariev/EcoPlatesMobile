using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Extended;
using Svg.Skia;
using System.Threading.Tasks;
using EcoPlatesMobile.Core;

namespace EcoPlatesMobile.Views;

public partial class LoginPage : BasePage
{
    private SKSvg svg;
    public LoginPage()
	{
		InitializeComponent();

        LoadSvg();
    }

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

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var session = AppService.Get<UserSessionService>();
        
        if (sender == btnComapny)
        {
            session?.SetUser(UserRole.Company);
        }
        else if (sender == btnUser)
        {
            session?.SetUser(UserRole.User);
        } 
        
        //await AppNavigatorService.NavigateTo($"//{AppRoutes.UserMainPage}");
         Application.Current.MainPage = new AppUserShell();
        //await AppNavigatorService.NavigateTo(AppRoutes.PhoneNumberPage);
    }   

}