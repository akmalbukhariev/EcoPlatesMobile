using System;
using Microsoft.Maui.Controls;
using EcoPlatesMobile.Resources.Languages;
using System.Reflection;

namespace EcoPlatesMobile.Helper
{
    // This behavior is attached to Entry (not CustomEntry)
    public class DoneKeyboardBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.HandlerChanged += OnHandlerChanged;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.HandlerChanged -= OnHandlerChanged;
        }

        private void OnHandlerChanged(object? sender, EventArgs e)
        {
#if IOS
        // iOS-specific code only. On Android this whole block is removed at compile time.
        if (sender is not Entry entry)
            return;

        var handler = entry.Handler;
        if (handler == null)
            return;

        // Get native UITextField
        if (handler.PlatformView is not UIKit.UITextField textField)
            return;

        var width = UIKit.UIScreen.MainScreen.Bounds.Width;
        var toolbar = new UIKit.UIToolbar(new CoreGraphics.CGRect(0, 0, width, 44))
        {
            Translucent = true
        };

        var flexible = new UIKit.UIBarButtonItem(UIKit.UIBarButtonSystemItem.FlexibleSpace);

        var doneButton = new UIKit.UIBarButtonItem(
            AppResource.Done, // localized text ("Done", "Tayyor", "Готово", etc.)
            UIKit.UIBarButtonItemStyle.Done,
            (s, ev) =>
            {
                // 🔥 trigger Entry.Completed exactly like the internal logic
                TrySendCompleted(entry);

                // hide keyboard
                textField.ResignFirstResponder();
            });

        toolbar.SetItems(new[] { flexible, doneButton }, false);

        textField.InputAccessoryView = toolbar;
#endif
            // Other platforms: do nothing
        }

#if IOS
    private static void TrySendCompleted(Entry entry)
    {
        try
        {
            // In MAUI, SendCompleted is defined on InputView (base class of Entry)
            var method = typeof(InputView).GetMethod(
                "SendCompleted",
                BindingFlags.Instance | BindingFlags.NonPublic);

            method?.Invoke(entry, null);
        }
        catch
        {
            // If reflection fails, we just skip; at worst only Completed won't fire.
        }
    }
#endif
    }
}