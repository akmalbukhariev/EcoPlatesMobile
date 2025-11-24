#if IOS
using System.Linq;
using Microsoft.Maui.Controls;
using EcoPlatesMobile.Services;        // namespace of IKeyboardHelper
using UIKit;
using Foundation;

[assembly: Dependency(typeof(EcoPlatesMobile.Platforms.iOS.KeyboardHelper))]
namespace EcoPlatesMobile.Platforms.iOS
{
    public class KeyboardHelper : IKeyboardHelper
    {
        public void HideKeyboard()
        {
            // iOS 13+ safe way: get key window from connected scenes
            var window = UIApplication.SharedApplication
                                      .ConnectedScenes
                                      .OfType<UIWindowScene>()
                                      .SelectMany(s => s.Windows)
                                      .FirstOrDefault(w => w.IsKeyWindow);

            window?.EndEditing(true);
        }
    }
}
#endif
