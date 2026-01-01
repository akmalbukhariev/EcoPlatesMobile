using System;
using System.Threading.Tasks;
using EcoPlatesMobile.Utilities;
using Microsoft.Maui.Controls;

namespace EcoPlatesMobile.Views.User.Components;

public partial class SortMenuPopupView : ContentView
{
    public event EventHandler<PosterSort>? SortSelected;
    public bool IsOpen => Overlay.IsVisible;

    public SortMenuPopupView()
    {
        InitializeComponent();
        UpdateChecks();
    }

    public static readonly BindableProperty SelectedSortProperty =
        BindableProperty.Create(nameof(SelectedSort), typeof(PosterSort),
            typeof(SortMenuPopupView), PosterSort.NEAR,
            propertyChanged: (b, o, n) => ((SortMenuPopupView)b).UpdateChecks());

    public PosterSort SelectedSort
    {
        get => (PosterSort)GetValue(SelectedSortProperty);
        set => SetValue(SelectedSortProperty, value);
    }

    public async Task ShowAsync(VisualElement anchor, VisualElement root)
    {
        var (ax, ay) = GetAbsolutePosition(anchor, root);

        double offsetY = anchor.Height + 10;
        double rightPadding = 12;

        double x = ax + anchor.Width - MenuCard.WidthRequest - rightPadding;
        if (x < 10) x = 10;

        double y = ay + offsetY;

        // Cancel any previous animations
        MenuCard.CancelAnimations();
        Overlay.CancelAnimations();

        // Final position
        MenuCard.TranslationX = x;
        MenuCard.TranslationY = y;

        // ✅ Dropdown feel: unfold from top
        MenuCard.AnchorY = 0;         // TOP edge
        MenuCard.AnchorX = 0.5;       // center horizontally

        // Start state
        MenuCard.ScaleY = 0.70;       // collapsed vertically
        MenuCard.ScaleX = 0.98;       // tiny horizontal pop
        MenuCard.Opacity = 0;

        Overlay.IsVisible = true;
        Overlay.Opacity = 0;

        // let it render one frame
        await Task.Delay(1);

        await Task.WhenAll(
            Overlay.FadeTo(1, 140),
            MenuCard.FadeTo(1, 120),
            MenuCard.ScaleYTo(1, 220, Easing.CubicOut),  // ✅ unfold down
            MenuCard.ScaleXTo(1, 220, Easing.CubicOut)
        );
    }

    public async Task HideAsync()
    {
        if (!Overlay.IsVisible) return;

        MenuCard.CancelAnimations();
        Overlay.CancelAnimations();

        // ✅ Close: fold up to top
        await Task.WhenAll(
            Overlay.FadeTo(0, 120),
            MenuCard.FadeTo(0, 100),
            MenuCard.ScaleYTo(0.75, 160, Easing.CubicIn),
            MenuCard.ScaleXTo(0.98, 160, Easing.CubicIn)
        );

        Overlay.IsVisible = false;

        // reset
        MenuCard.Opacity = 1;
        MenuCard.ScaleX = 1;
        MenuCard.ScaleY = 1;
    }

    static (double x, double y) GetAbsolutePosition(VisualElement element, VisualElement root)
    {
        double x = 0, y = 0;
        VisualElement? v = element;

        // Sum X/Y up the visual tree until root
        while (v != null && v != root)
        {
            x += v.X;
            y += v.Y;
            v = v.Parent as VisualElement;
        }

        return (x, y);
    }

    void UpdateChecks()
    {
        CheckNear.IsVisible = SelectedSort == PosterSort.NEAR;
        CheckCheap.IsVisible = SelectedSort == PosterSort.CHEAP;
        CheckDiscount.IsVisible = SelectedSort == PosterSort.DISCOUNT;
    }

    async void OnOverlayTapped(object sender, TappedEventArgs e) => await HideAsync();
    void OnInsideTapped(object sender, TappedEventArgs e) { /* swallow tap */ }

    async void OnNearTapped(object sender, TappedEventArgs e)
    {
        SelectedSort = PosterSort.NEAR;
        SortSelected?.Invoke(this, SelectedSort);
        await HideAsync();
    }

    async void OnCheapTapped(object sender, TappedEventArgs e)
    {
        SelectedSort = PosterSort.CHEAP;
        SortSelected?.Invoke(this, SelectedSort);
        await HideAsync();
    }

    async void OnDiscountTapped(object sender, TappedEventArgs e)
    {
        SelectedSort = PosterSort.DISCOUNT;
        SortSelected?.Invoke(this, SelectedSort);
        await HideAsync();
    }
}
