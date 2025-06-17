using CommunityToolkit.Mvvm.ComponentModel;
using EcoPlatesMobile.Models;
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
        Type = BusinessType.RESTAURANT;
        Unclick();
    }

    public void Click()
    {
        TitleColor = Colors.Green;
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
            new TypeItem { Name = "Restaurant", Image = "restaurant.png", Type = BusinessType.RESTAURANT },
            new TypeItem { Name = "Bakery", Image = "bakery.png", Type = BusinessType.BAKERY },
            new TypeItem { Name = "Fast Food", Image = "fast_food.png", Type = BusinessType.FAST_FOOD },
            new TypeItem { Name = "Cafe", Image = "cafe.png", Type = BusinessType.CAFE },
            new TypeItem { Name = "Supermarket", Image = "market.png", Type = BusinessType.SUPERMARKET }
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
    }
}