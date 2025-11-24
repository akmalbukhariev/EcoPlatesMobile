using System.Runtime.CompilerServices;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.ViewModels;

using Microsoft.Maui.Controls.PlatformConfiguration;
//using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

namespace EcoPlatesMobile.Views
{
    public abstract class BasePage : ContentPage
    {
        protected IViewModel viewModel;
        protected CancellationTokenSource? cts;
        protected BasePage()
        {
            Shell.SetNavBarIsVisible(this, false);
            Shell.SetTabBarIsVisible(this, false); 
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected void CancelAndDisposeCts()
        {
            try { cts?.Cancel(); }
            catch { }
            finally { cts?.Dispose(); cts = null; }
        }

        protected void SetViewModel(IViewModel viewModel)
        {
            this.viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected Task AnimateElementScaleUp(VisualElement element)
        {
            return Task.Run(async () =>
            {
                await element.ScaleTo(1.3, 100, Easing.CubicOut);
                await element.ScaleTo(1.0, 100, Easing.CubicIn);
            });
        }

        protected Task AnimateElementScaleDown(VisualElement element)
        {
            return Task.Run(async () =>
            {
                await element.ScaleTo(0.9, 100, Easing.CubicOut);
                await element.ScaleTo(1.0, 100, Easing.CubicIn);
            });
        }

        private bool isAnimating;
        protected async Task AnimateSelectAllBarAsync(HorizontalStackLayout selectAllBar, bool show)
        {
            if (isAnimating) return;
            isAnimating = true;

            const uint duration = 220;
            var easing = Easing.SinOut;

            // Cancel any in-flight animations just in case
            selectAllBar.AbortAnimation("TranslationX");
            selectAllBar.AbortAnimation("FadeTo");

            if (show)
            {
                selectAllBar.IsVisible = true;
                selectAllBar.TranslationX = -60; // start a bit left
                selectAllBar.Opacity = 0;

                var fadeIn = selectAllBar.FadeTo(1, duration, easing);
                var slideIn = selectAllBar.TranslateTo(0, 0, duration, easing);

                await Task.WhenAll(fadeIn, slideIn);
            }
            else
            {
                // slide/fade out to the left, then hide
                var fadeOut = selectAllBar.FadeTo(0, duration, easing);
                var slideOut = selectAllBar.TranslateTo(-60, 0, duration, easing);

                await Task.WhenAll(fadeOut, slideOut);
                selectAllBar.IsVisible = false;
            }

            isAnimating = false;
        }

        protected async Task Back()
        {
            await AppNavigatorService.NavigateTo("..");
        }

        /// <summary>
        /// Resolves the ViewModel through the DI container.
        /// </summary>
        /// <typeparam name="T">The type of the ViewModel to resolve.</typeparam>
        /// <returns>The resolved ViewModel instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the ViewModel cannot be resolved.</exception>
        /*protected T ResolveViewModel<T>() where T : class, IViewModel
        {
            var viewModelFactory = AppService.Get<IViewModelFactory>();

            if (viewModelFactory == null)
            {
                throw new InvalidOperationException($"ViewModelFactory is not registered in the DI container.");
            }

            var viewModel = viewModelFactory.CreateViewModel<T>();

            if (viewModel == null)
            {
                throw new InvalidOperationException($"Unable to resolve ViewModel of type {typeof(T).Name}");
            }

            SetViewModel(viewModel);
            return viewModel;
        }
        */
    }
}
