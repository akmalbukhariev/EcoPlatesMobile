using System.Collections.ObjectModel;
using System.Windows.Input;
using EcoPlatesMobile.Models;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Views;

public partial class AnnouncementsPage : BasePage
{
    public ObservableCollection<Announcement> Announcements { get; } = new();

    private Announcement? _selectedAnnouncement;
    public Announcement? SelectedAnnouncement
    {
        get => _selectedAnnouncement;
        set
        {
            _selectedAnnouncement = value;
            OnPropertyChanged();
        }
    }

    public ICommand RefreshCommand { get; }
    public ICommand OpenAnnouncementCommand { get; }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            _isBusy = value;
            OnPropertyChanged();
        }
    }

    private double _sheetHeight = 420;
    public double SheetHeight
    {
        get => _sheetHeight;
        set
        {
            _sheetHeight = value;
            OnPropertyChanged();
        }
    }

    private readonly UserSessionService userSessionService;
    private readonly AppControl appControl;
    private readonly UserApiService userApiService;
    private readonly CompanyApiService companyApiService;

    public AnnouncementsPage(UserSessionService userSessionService,
                             AppControl appControl,
                             UserApiService userApiService,
                             CompanyApiService companyApiService)
    {
        InitializeComponent();

        this.userSessionService = userSessionService;
        this.appControl = appControl;
        this.userApiService = userApiService;
        this.companyApiService = companyApiService;

        BindingContext = this;

        RefreshCommand = new Command(async () => await LoadAnnouncementsAsync());
        OpenAnnouncementCommand = new Command<Announcement>(OnOpenAnnouncement);

        // Initial load
        //_ = LoadAnnouncementsAsync();

        SizeChanged += (_, __) =>
        {
            if (Height <= 0) return;

            SheetHeight = Math.Min(Height * 0.55, 520);

            // Keep hidden off-screen when overlay not visible
            if (SheetOverlay != null && !SheetOverlay.IsVisible && BottomSheet != null)
                BottomSheet.TranslationY = SheetHeight;
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (userSessionService.Role == UserRole.User)
        {
            header.HeaderBackground = Constants.COLOR_USER;
            loading.ChangeColor(Constants.COLOR_USER);
        }
        else
        {
            header.HeaderBackground = Constants.COLOR_COMPANY;
            loading.ChangeColor(Constants.COLOR_COMPANY);
        }
    }

    private async Task LoadAnnouncementsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            await Task.Delay(600);

            Announcements.Clear();

            Announcements.Add(new Announcement
            {
                AnnouncementId = 1,
                Title = "Holiday Sale Coming Soon!",
                Preview = "Get ready for our big holiday sale next week.",
                Body = "Enjoy discounts on a wide range of products. Stay tuned!",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
                IsRead = false
            });

            Announcements.Add(new Announcement
            {
                AnnouncementId = 2,
                Title = "Updated Seller Guidelines",
                Preview = "Please review the new policy changes.",
                Body = "We updated our seller guidelines to improve quality.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-3),
                IsRead = true
            });

            Announcements.Add(new Announcement
            {
                AnnouncementId = 3,
                Title = "Maintenance Notice",
                Preview = "Scheduled maintenance on Dec 5th.",
                Body = "The app may be temporarily unavailable.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-7),
                IsRead = true
            });
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnOpenAnnouncement(Announcement announcement)
    {
        if (announcement == null) return;

        if (!announcement.IsRead)
        {
            announcement.IsRead = true;
            OnPropertyChanged(nameof(Announcements));
        }

        SelectedAnnouncement = announcement;
        await ShowBottomSheetAsync();
    }

    private bool _sheetAnimating;

    private async Task ShowBottomSheetAsync()
    {
        if (_sheetAnimating) return;
        if (SheetOverlay == null || BottomSheet == null) return;

        // Ensure height is valid (first open sometimes happens before SizeChanged)
        if (Height > 0)
            SheetHeight = Math.Min(Height * 0.55, 520);

        _sheetAnimating = true;

        try
        {
            SheetOverlay.IsVisible = true;

            BottomSheet.TranslationY = SheetHeight;
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
                BottomSheet.TranslateTo(0, SheetHeight, 220, Easing.CubicIn)
            );

            SheetOverlay.IsVisible = false;
            BottomSheet.TranslationY = SheetHeight;
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
}