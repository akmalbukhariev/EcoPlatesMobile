using System.ComponentModel;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.ViewModels.User;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserMainPage : BasePage
{
	private UserMainPageViewModel viewModel;
    Components.TypeItem typeItem = null;

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

        Shell.SetTabBarIsVisible(this, true);
        AppService.Get<AppControl>().ShowCompanyMoreInfo = true;

        if (typeItem == null)
        {
            viewModel.BusinessType = Utilities.BusinessType.RESTAURANT;
        }
        else
        {
            viewModel.BusinessType = typeItem.Type;
            companyTypeList.InitType(typeItem);
        }

        lbHeader.Text = $"Sizga yaqin — {AppService.Get<AppControl>().UserInfo.radius_km} km atrofda.";
        await viewModel.LoadInitialAsync();
    }

    private async void CompanyTypeList_EventTypeClick(Components.TypeItem item)
    {
        typeItem = item;
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

    private async void Search_Tapped(object sender, TappedEventArgs e)
    {
        await AppNavigatorService.NavigateTo(nameof(UserMainSearchPage));
    }
}