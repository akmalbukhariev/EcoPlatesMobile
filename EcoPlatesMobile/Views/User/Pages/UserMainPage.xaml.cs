using System.ComponentModel;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.ViewModels.User;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserMainPage : BasePage
{
	private UserMainPageViewModel viewModel;

    public UserMainPage()
	{
		InitializeComponent();

        viewModel = ResolveViewModel<UserMainPageViewModel>();
        SetViewModel(viewModel);
         
        viewModel.PropertyChanged += ViewModel_PropertyChanged;
        companyTypeList.EventTypeClick += CompanyTypeList_EventTypeClick;
    }
     
    protected override async void OnAppearing()
	{
		base.OnAppearing();

        AppService.Get<AppControl>().ShowCompanyMoreInfo = true;

        viewModel.BusinessType = Utilities.BusinessType.RESTAURANT;
        await viewModel.LoadInitialAsync();
    }

    private async void CompanyTypeList_EventTypeClick(Components.TypeItem item)
    {
        viewModel.BusinessType = item.Type;
        await viewModel.LoadInitialAsync();
    }

    private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(viewModel.ShowLikedView) && viewModel.ShowLikedView)
        {
            await likeView.DisplayAsAnimation();
            viewModel.ShowLikedView = false;
        }
    }
}