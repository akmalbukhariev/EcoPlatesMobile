
using EcoPlatesMobile.ViewModels;

namespace EcoPlatesMobile.Services
{
    public interface IViewModelFactory
    {
        T CreateViewModel<T>() where T : class, IViewModel;
    }

    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewModelFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T CreateViewModel<T>() where T : class, IViewModel
        {
            var viewModel = _serviceProvider.GetRequiredService<T>();
            //viewModel.Initialize();
            return viewModel;
        }
    }
}
