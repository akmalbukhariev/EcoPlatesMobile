namespace EcoPlatesMobile.Views.User.Pages;

using EcoPlatesMobile.Models.User;
using Microsoft.Maui.Controls; 

public partial class DetailProductPage : ContentPage
{ 
    private ProductModel dataModel;
    public DetailProductPage(ProductModel dataModel)
	{
		InitializeComponent();
        this.dataModel = dataModel;

        Shell.SetNavBarIsVisible(this, false);
        Shell.SetTabBarIsVisible(this, false);
    }

    protected override void OnAppearing()
	{
		base.OnAppearing();

         
    }

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void Share_Tapped(object sender, TappedEventArgs e)
    {
         
    }

    private void Like_Tapped(object sender, TappedEventArgs e)
    {
        
    }
}