using CommunityToolkit.Mvvm.ComponentModel;
using EcoPlatesMobile.Models;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Utilities;
using System.Collections.ObjectModel;

namespace EcoPlatesMobile.Views.User.Components;

public partial class TypeItem : ObservableObject
{
    [ObservableProperty] string name;
    [ObservableProperty] string image;
    [ObservableProperty] Color titleColor;
    [ObservableProperty] TextDecorations titleDecoration;
    public BusinessType Type;

    public TypeItem()
    {
        Type = BusinessType.SUPERMARKET;
        Unclick();
    }

    public void Click()
    {
        TitleColor = Constants.COLOR_USER;
        TitleDecoration = TextDecorations.Underline;
    }

    public void Unclick()
    {
        TitleColor = Colors.Black;
        TitleDecoration = TextDecorations.None;
    }
}

public partial class CompanyTypeListView : ContentView
{
    public event Action<TypeItem> EventTypeClick;
    public ObservableCollection<TypeItem> Items { get; set; }

    public CompanyTypeListView()
	{
		InitializeComponent();
        
        Items = new ObservableCollection<TypeItem>
        {
            new TypeItem { Name = AppResource.All, Image = "all.png", Type = BusinessType.OTHER },
            new TypeItem { Name = AppResource.Supermarket, Image = "market.png", Type = BusinessType.SUPERMARKET },
            new TypeItem { Name = AppResource.Restaurant, Image = "restaurant.png", Type = BusinessType.RESTAURANT },
            new TypeItem { Name = AppResource.Bakery, Image = "bakery.png", Type = BusinessType.BAKERY },
            new TypeItem { Name = AppResource.FastFood, Image = "fast_food.png", Type = BusinessType.FAST_FOOD },
            new TypeItem { Name = AppResource.Cafe, Image = "cafe.png", Type = BusinessType.CAFE },
        };
        Items[0].Click();

        BindingContext = this;
    }

    public void InitType(TypeItem item)
    {
        foreach (TypeItem tItem in Items)
        {
            tItem.Unclick();
        }

        item.Click();
    }

    private async void Item_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            if (sender is VisualElement element)
            {
                await element.ScaleTo(0.8, 100, Easing.CubicOut);
                await element.ScaleTo(1.0, 100, Easing.CubicIn);
            }

            foreach (TypeItem tItem in Items)
            {
                tItem.Unclick();
            }

            if (e.Parameter is TypeItem item)
            {
                item.Click();
                EventTypeClick?.Invoke(item);
            }
        });
    }
}