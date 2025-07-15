using Android.Views;
using EcoPlatesMobile.Platforms.Android;
using EcoPlatesMobile.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Graphics;
using Color = Android.Graphics.Color;
using Android.OS;
using AndroidX.Core.View;

[assembly: Dependency(typeof(StatusBarService))]
namespace EcoPlatesMobile.Platforms.Android
{
    public class StatusBarService : IStatusBarService
    {
        public void SetStatusBarColor(string hexColor, bool darkStatusBarTint)
        {
            var activity = Platform.CurrentActivity;
            if (activity == null) return;

            var window = activity.Window;
            window.SetStatusBarColor(Color.ParseColor(hexColor));

            // Modern way using AndroidX
            var decorView = window.DecorView;
            var insetsController = ViewCompat.GetWindowInsetsController(decorView);

            if (insetsController != null)
            {
                insetsController.AppearanceLightStatusBars = darkStatusBarTint;
            }
        }
    }
}
