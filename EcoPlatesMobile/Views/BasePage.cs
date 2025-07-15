using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.ViewModels;

namespace EcoPlatesMobile.Views
{
    public abstract class BasePage : ContentPage
    {
        protected IViewModel viewModel;
 
        protected BasePage()
        {
            Shell.SetNavBarIsVisible(this, false);
            Shell.SetTabBarIsVisible(this, false);
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
