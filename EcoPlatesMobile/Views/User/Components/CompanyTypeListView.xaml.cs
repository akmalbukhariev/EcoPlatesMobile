using EcoPlatesMobile.Models;
using System.Collections.ObjectModel;

namespace EcoPlatesMobile.Views.User.Components;

public class TypeItem : BaseModel
{
    public string Name { get { return GetValue<string>(); } set => SetValue(value); }
    public string Image { get { return GetValue<string>(); } set => SetValue(value); }
}

public partial class CompanyTypeListView : ContentView
{
    public ObservableCollection<TypeItem> Items { get; set; }

    public CompanyTypeListView()
	{
		InitializeComponent();
        
        Items = new ObservableCollection<TypeItem>
        {
            new TypeItem { Name = "Restaurant", Image = "restaurant.png" },
            new TypeItem { Name = "Bakery", Image = "bakery.png" },
            new TypeItem { Name = "Fast Food", Image = "fast_food.png" },
            new TypeItem { Name = "Cafe", Image = "cafe.png" },
            new TypeItem { Name = "Supermarket", Image = "market.png" }
        };

        BindingContext = this;
    }
}