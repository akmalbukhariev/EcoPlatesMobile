using EcoPlatesMobile;
using EcoPlatesMobile.Views.Components;
using Foundation;
using Microsoft.Maui.Controls;
using UIKit;

[assembly: Dependency(typeof(KeyboardHelper))]
namespace EcoPlatesMobile
{
    public class KeyboardHelper : IKeyboardHelper
    {
        public void HideKeyboard()
        {
            UIApplication.SharedApplication.KeyWindow?.EndEditing(true);
        }
    }
}