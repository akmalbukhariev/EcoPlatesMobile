using System.ComponentModel;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.ViewModels.User;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class UserMainPage : BasePage
{
    Components.TypeItem typeItem = null;

    private UserMainPageViewModel viewModel;
    private AppControl appControl;

    public UserMainPage(UserMainPageViewModel vm, AppControl appControl)
	{
		InitializeComponent();
        this.viewModel = vm;
        this.appControl = appControl;

        //viewModel = ResolveViewModel<UserMainPageViewModel>();
         
        viewModel.PropertyChanged += ViewModel_PropertyChanged;
        companyTypeList.EventTypeClick += CompanyTypeList_EventTypeClick;

        BindingContext = viewModel;
    }
     
    protected override async void OnAppearing()
	{
		base.OnAppearing();

        Shell.SetTabBarIsVisible(this, true);

        if (typeItem == null)
        {
            viewModel.BusinessType = Utilities.BusinessType.RESTAURANT;
        }
        else
        {
            viewModel.BusinessType = typeItem.Type;
            companyTypeList.InitType(typeItem);
        }

        lbHeader.Text = $"{AppResource.NearbyWithin} {appControl.UserInfo.radius_km} km {AppResource.Around}.";

        if (appControl.RefreshMainPage)
        {
            await viewModel.LoadInitialAsync();
            appControl.RefreshMainPage = false;
        }
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