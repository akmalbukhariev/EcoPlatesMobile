namespace EcoPlatesMobile.Views.User.Pages;

using Microsoft.Maui.Controls; 

public partial class DetailProductPage : ContentPage
{ 
    public DetailProductPage()
	{
		InitializeComponent();

        
        Shell.SetNavBarIsVisible(this, false);
        Shell.SetTabBarIsVisible(this, false);
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

    private async void Company_Tapped(object sender, TappedEventArgs e)
    {
        await gridCompany.ScaleTo(0.95, 100, Easing.CubicOut);
        await gridCompany.ScaleTo(1.0, 100, Easing.CubicIn);
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}