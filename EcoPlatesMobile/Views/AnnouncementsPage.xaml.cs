using System.Collections.ObjectModel;
using System.Windows.Input;
using EcoPlatesMobile.Models;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.ViewModels;

namespace EcoPlatesMobile.Views;

public partial class AnnouncementsPage : BasePage
{
    public ObservableCollection<Announcement> Announcements { get; } = new();
      
    private readonly AnnouncementsPageViewModel viewModel;
    private readonly UserSessionService userSessionService;
    private readonly AppControl appControl;

    public AnnouncementsPage(AnnouncementsPageViewModel vm,
                             UserSessionService userSessionService,
                             AppControl appControl)
    {
        InitializeComponent();

        this.viewModel = vm;
        this.userSessionService = userSessionService;
        this.appControl = appControl;

        BindingContext = viewModel;

        SizeChanged += (_, __) =>
        {
            if (Height <= 0) return;

            viewModel.SheetHeight = Math.Min(Height * 0.55, 520);
 
            if (SheetOverlay != null && !SheetOverlay.IsVisible && BottomSheet != null)
                BottomSheet.TranslationY = viewModel.SheetHeight;
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (userSessionService.Role == UserRole.User)
        {
            header.HeaderBackground = Constants.COLOR_USER;
            loading.Color = Constants.COLOR_USER;
        }
        else
        {
            header.HeaderBackground = Constants.COLOR_COMPANY;
            loading.Color = Constants.COLOR_COMPANY;
        }

        await viewModel.LoadInitialAsync();
    }
      
    private bool _sheetAnimating;

    private async Task ShowBottomSheetAsync()
    {
        if (_sheetAnimating) return;
        if (SheetOverlay == null || BottomSheet == null) return;

        // Ensure height is valid (first open sometimes happens before SizeChanged)
        if (Height > 0)
            viewModel.SheetHeight = Math.Min(Height * 0.55, 520);

        _sheetAnimating = true;

        try
        {
            SheetOverlay.IsVisible = true;

            BottomSheet.TranslationY = viewModel.SheetHeight;
            SheetOverlay.Opacity = 0;

            await Task.WhenAll(
                SheetOverlay.FadeTo(1, 160, Easing.CubicInOut),
                BottomSheet.TranslateTo(0, 0, 260, Easing.CubicOut)
            );
        }
        finally
        {
            _sheetAnimating = false;
        }
    }

    private async Task HideBottomSheetAsync()
    {
        if (_sheetAnimating) return;
        if (SheetOverlay == null || BottomSheet == null) return;

        _sheetAnimating = true;

        try
        {
            await Task.WhenAll(
                SheetOverlay.FadeTo(0, 140, Easing.CubicInOut),
                BottomSheet.TranslateTo(0, viewModel.SheetHeight, 220, Easing.CubicIn)
            );

            SheetOverlay.IsVisible = false;
            BottomSheet.TranslationY = viewModel.SheetHeight;
        }
        finally
        {
            _sheetAnimating = false;
        }
    }
    
    private async void OnBackdropTapped(object sender, EventArgs e)
    {
        await HideBottomSheetAsync();
    }

    private async void OnCloseSheetClicked(object sender, EventArgs e)
    {
        await HideBottomSheetAsync();
    }

    private async void Announcement_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is not Frame frame)
            return;

        await AnimateElementScaleDown(frame);

        var announcement = e.Parameter as Announcement ?? frame.BindingContext as Announcement;
        if (announcement == null) return;

        viewModel.SelectedAnnouncement = announcement;

        viewModel.IsLoading = true;
        bool success = await viewModel.MarkAsRead();
        viewModel.IsLoading = false;

        if (!announcement.IsRead && success)
        {
            announcement.IsRead = true;
        }

        await ShowBottomSheetAsync();
    }
}