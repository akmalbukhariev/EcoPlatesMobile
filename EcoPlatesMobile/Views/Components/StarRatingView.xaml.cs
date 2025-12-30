namespace EcoPlatesMobile.Views.Components;

public partial class StarRatingView : ContentView
{
    public static readonly BindableProperty RatingProperty =
        BindableProperty.Create(
            nameof(Rating),
            typeof(double),
            typeof(StarRatingView),
            0.0,
            propertyChanged: (_, __, ___) => ((StarRatingView)_).Render());

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(StarRatingView),
            string.Empty,
            propertyChanged: (_, __, ___) => ((StarRatingView)_).Render());

    public double Rating
    {
        get => (double)GetValue(RatingProperty);
        set => SetValue(RatingProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public StarRatingView()
    {
        InitializeComponent();
        Render();
    }

    private void Render()
    {
        if (StarRow == null) return;

        StarRow.Children.Clear();

        int filled = (int)Math.Floor(Rating);

        for (int i = 1; i <= 5; i++)
        {
            StarRow.Children.Add(new Image
            {
                HeightRequest = 18,
                WidthRequest = 18,
                Margin = new Thickness(1, 0),
                Source = (i <= filled) ? "star1.png" : "star_gray.png"
            });
        }

        if (!string.IsNullOrWhiteSpace(Text))
        {
            StarRow.Children.Add(new Label
            {
                Text = $"({Text})",
                TextColor = Colors.Black,
                FontFamily = "RobotoVar",
                Margin = new Thickness(5, 0, 0, 0),
                VerticalOptions = LayoutOptions.Center
            });
        }
    }
}
