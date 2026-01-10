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
    private readonly AnnouncementsPageViewModel viewModel;

    public AnnouncementsPage(AnnouncementsPageViewModel vm)
    {
        InitializeComponent();

        this.viewModel = vm;

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

            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                boxBackground.Opacity = 0.5;      // show dark overlay
                boxBackground.InputTransparent = false; // start catching taps
            }
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

            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                boxBackground.Opacity = 0;        // hide overlay
                boxBackground.InputTransparent = true;  // let taps pass through again
            }
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

    private async void Announcement_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
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
        });
    }

    private async void OnCloseSheetClicked(object sender, EventArgs e)
    {
        await HideBottomSheetAsync();
    }

    private double _panStartTranslationY;
    private bool _isPanning;

    private async void OnSheetPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (_sheetAnimating) return;
        if (BottomSheet == null || SheetOverlay == null) return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _isPanning = true;
                _panStartTranslationY = BottomSheet.TranslationY; // usually 0 when opened
                break;

            case GestureStatus.Running:
                if (!_isPanning) return;

                // Only allow dragging DOWN
                var newY = _panStartTranslationY + e.TotalY;
                if (newY < 0) newY = 0;

                BottomSheet.TranslationY = newY;

                // Optional: fade background while dragging down
                // (0 -> fully visible, SheetHeight -> hidden)
                var progress = Math.Clamp(newY / viewModel.SheetHeight, 0, 1);
                SheetOverlay.Opacity = 1 - (0.35 * progress); // subtle fade
                if (DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    boxBackground.Opacity = 0.5 * (1 - progress);
                }
                break;

            case GestureStatus.Canceled:
            case GestureStatus.Completed:
                _isPanning = false;

                // Decide: close vs snap back
                var closeThreshold = viewModel.SheetHeight * 0.22; // 22% down closes
                var shouldClose = BottomSheet.TranslationY > closeThreshold || e.TotalY > 120;

                if (shouldClose)
                {
                    await HideBottomSheetAsync();
                }
                else
                {
                    // Snap back to open position
                    _sheetAnimating = true;
                    try
                    {
                        await Task.WhenAll(
                            BottomSheet.TranslateTo(0, 0, 160, Easing.CubicOut),
                            SheetOverlay.FadeTo(1, 120, Easing.CubicInOut)
                        );
                        if (DeviceInfo.Platform == DevicePlatform.iOS)
                        {
                            boxBackground.Opacity = 0.5;
                        }
                    }
                    finally
                    {
                        _sheetAnimating = false;
                    }
                }
                break;
        }
    }
}