using EcoPlatesMobile.Resources.Languages;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class MapBottomSheet : ContentView
{
	public event Action EventShowResultsClicked;
	public event Action<int> EventValueDistanceChanged;
	public event Action? Dismissed;
	const uint ShowDuration = 250;
    const uint HideDuration = 220;

	bool isOpen;
	bool animating;
	double measuredHeight;
	bool dragging;
	
	double dragStartY;               // where the drag started (TranslationY at drag start)
	double dragTotalY;               // current drag total (delta since start)
	DateTime dragStartTime; 

	const double DismissDistanceRatio = 0.18; // 18% of sheet height
	const double DismissMinPixels = 28;       // minimum absolute pixels to consider
	const double DismissMinVelocity = 450;   // px/s to allow a quick flick dismiss

	public MapBottomSheet()
	{
		InitializeComponent();

		Loaded += (_, __) =>
		{
			// Ensure we start off-screen so first ShowAsync animates up
			EnsureMeasuredHeight();
			TranslationY = measuredHeight + 8; // small offset to be fully off
		};

		SizeChanged += (_, __) =>
		{
			if (dragging) return;    
			measuredHeight = 0;
			if (!isOpen)
			{
				EnsureMeasuredHeight();
				TranslationY = measuredHeight + 8;
			}
		};
	}
    
	public async Task ShowAsync(uint duration = ShowDuration)
	{
		if (isOpen || animating) return;

		animating = true;
		IsVisible = true;

		EnsureMeasuredHeight();             // measure ONCE per open
		TranslationY = measuredHeight + 8;

		await this.TranslateTo(0, 0, duration, Easing.CubicOut);

		isOpen = true;
		animating = false;
	}

	public async Task DismissAsync(uint duration = HideDuration)
	{
		if (!isOpen || animating) return;

		animating = true;

		EnsureMeasuredHeight();
		await this.TranslateTo(0, measuredHeight + 8, duration, Easing.CubicIn);

		IsVisible = false;
		isOpen = false;
		animating = false;
		
		Dismissed?.Invoke();
	}
    
	public bool IsOpen => isOpen;
	
	void EnsureMeasuredHeight()
	{
		if (measuredHeight > 0) return;

		// Measure desired height of this ContentView within current width
		var width = (Parent as VisualElement)?.Width > 0 ? (Parent as VisualElement)!.Width : this.Width;
		if (width <= 0) width = Application.Current?.MainPage?.Width ?? 400;

		var req = this.Measure(width, double.PositiveInfinity, MeasureFlags.IncludeMargins);
		measuredHeight = req.Request.Height > 0 ? req.Request.Height : (Application.Current?.MainPage?.Height ?? 800) * 0.4;
	}

	public void SetMaxValue(int maxValue)
	{
		distanceSlider.Maximum = maxValue;
	}

	public void SetValue(int km)
	{
		distanceSlider.Value = km;
		distanceLabel.Text = $"{km} {AppResource.Km}";
	}

	private void DistanceSlider_ValueChanged(object sender, ValueChangedEventArgs e)
	{
		int km = (int)Math.Round(e.NewValue);
		distanceLabel.Text = $"{km} {AppResource.Km}";

		EventValueDistanceChanged?.Invoke(km);
	}

	private async void ShowResults_Clicked(object sender, EventArgs e)
	{
		await ClickGuard.RunAsync((VisualElement)sender, async () =>
		{
			EventShowResultsClicked?.Invoke();
		});
	}

	void Handle_PanUpdated(object sender, PanUpdatedEventArgs e)
	{
		if (!isOpen) return;                // ignore if closed

		switch (e.StatusType)
		{
			case GestureStatus.Started:
                // stop any ongoing animations so TranslateTo doesn't fight the drag
                Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(this);

				dragging = true;
				animating = false;          // drag wins
				// DO NOT re-measure mid-drag; use cached measuredHeight
				EnsureMeasuredHeight();

				// we always open at Y=0; ensure a stable baseline
				dragStartY = 0;
				dragTotalY = 0;
				dragStartTime = DateTime.UtcNow;
				break;

			case GestureStatus.Running:
				if (animating || !dragging) return;

				// only allow downward motion; clamp displacement
				dragTotalY = Math.Max(0, e.TotalY);

				// clamp absolute position [0 .. measuredHeight+8]
				var maxY = measuredHeight + 8;
				var newY = Math.Min(maxY, dragStartY + dragTotalY);
				this.TranslationY = newY;
				break;

			case GestureStatus.Completed:
			case GestureStatus.Canceled:
				if (!dragging) return;
				dragging = false;

				var elapsed  = (DateTime.UtcNow - dragStartTime).TotalSeconds;
				var velocity = elapsed > 0 ? dragTotalY / elapsed : 0; // px/s

				bool farEnough  = dragTotalY >= Math.Max(DismissMinPixels, measuredHeight * DismissDistanceRatio);
				bool fastEnough = velocity   >= DismissMinVelocity;

				if (farEnough || fastEnough)
				{
					_ = DismissAsync();     // smooth close; event fires after
				}
				else
				{
					animating = true;
					_ = this.TranslateTo(0, 0, 120, Easing.CubicOut)
						.ContinueWith(_ => animating = false);
				}
				break;
		}
	}

}