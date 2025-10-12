using EcoPlatesMobile.Models.Requests.Company;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.User;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.ViewModels.Company;
using EcoPlatesMobile.Models.Responses.Notification;
using Newtonsoft.Json.Linq;
using EcoPlatesMobile.Models.Chat;
using EcoPlatesMobile.Views.Chat;
using ViewExt = Microsoft.Maui.Controls.ViewExtensions;

namespace EcoPlatesMobile.Views.Company.Pages;

[QueryProperty(nameof(ShowBackQuery), nameof(ShowBackQuery))]
[QueryProperty(nameof(ShowTabBarQuery), nameof(ShowTabBarQuery))]

public partial class PendingProductPage : BasePage
{
    private bool ShowBack { get; set; } = false;
    private bool ShowTabBar { get; set; } = true;

    public string ShowBackQuery
    {
        set
        {
            if (bool.TryParse(value, out var result))
                ShowBack = result;
        }
    }

    public string ShowTabBarQuery
    {
        set
        {
            if (bool.TryParse(value, out var result))
                ShowTabBar = result;
        }
    }

    private PendingProductPageViewModel viewModel;
    private AppControl appControl;
    private CompanyApiService companyApiService;

    public PendingProductPage(PendingProductPageViewModel vm, CompanyApiService companyApiService, AppControl appControl)
    {
        InitializeComponent();

        this.viewModel = vm;
        this.companyApiService = companyApiService;
        this.appControl = appControl;

        this.BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetTabBarIsVisible(this, ShowTabBar);

        header.ShowBack = ShowBack;

        bool isWifiOn = await appControl.CheckWifiOrNetwork();
        if (!isWifiOn) return;

        await viewModel.LoadInitialAsync();
    }
}