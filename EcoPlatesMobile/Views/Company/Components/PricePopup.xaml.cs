using CommunityToolkit.Maui.Views;

namespace EcoPlatesMobile.Views.Company.Components;

public partial class PricePopup : Popup
{
    public event Action<int, int> OnPriceEntered;

    public PricePopup()
    {
        InitializeComponent();
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (int.TryParse(OldPriceEntry.Text, out int oldPrice) &&
            int.TryParse(NewPriceEntry.Text, out int newPrice))
        {
            OnPriceEntered?.Invoke(oldPrice, newPrice);
            Close();
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Invalid Input", "Please enter valid numbers", "OK");
        }
    }
}