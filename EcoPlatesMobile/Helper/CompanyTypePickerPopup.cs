
using CommunityToolkit.Maui.Views;

namespace EcoPlatesMobile.Helper;

public class CompanyTypePickerPopup : Popup
{
    public CompanyTypePickerPopup(IEnumerable<CompanyTypeModel> items)
    {
        var listView = new ListView
        {
            ItemsSource = items.ToList(),
            HeightRequest = 200,
            ItemTemplate = new DataTemplate(() =>
            {
                var cell = new TextCell();
                cell.SetBinding(TextCell.TextProperty, "Type");
                return cell;
            })
        };

        listView.ItemSelected += (s, e) =>
        {
            if (e.SelectedItem is CompanyTypeModel selected)
            {
                Close(selected);
            }
        };

        Content = new Grid
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                new Frame
                {
                    WidthRequest = 350,
                    Padding = 10,
                    CornerRadius = 12,
                    BackgroundColor = Colors.White,
                    Content = listView,
                    Shadow = new Shadow
                    {
                        Brush = Brush.Black,
                        Offset = new Point(5,5),
                        Radius = 10,
                        Opacity = 0.3f
                    }
                }
            }
        };
    }
}
