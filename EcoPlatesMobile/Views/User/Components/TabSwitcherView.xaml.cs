namespace EcoPlatesMobile.Views.User.Components;

public partial class TabSwitcherView : ContentView
{
    public event EventHandler<string>? TabChanged;

    public TabSwitcherView()
    {
        InitializeComponent();
    }

    private void OnTabClicked(object sender, EventArgs e)
    {
        if (sender is Button clickedButton)
        {
            if (clickedButton == SellersTab)
            {
                SetActiveTab(SellersTab);
                TabChanged?.Invoke(this, "Sotuvchilar");
            }
            else if (clickedButton == ProductsTab)
            {
                SetActiveTab(ProductsTab);
                TabChanged?.Invoke(this, "Mahsulotlar");
            }
        }
    }

    private void SetActiveTab(Button activeButton)
    {
        SellersTab.BackgroundColor = activeButton == SellersTab ? Colors.White : Colors.Transparent;
        SellersTab.TextColor = activeButton == SellersTab ? Colors.Black : Colors.Gray;

        ProductsTab.BackgroundColor = activeButton == ProductsTab ? Colors.White : Colors.Transparent;
        ProductsTab.TextColor = activeButton == ProductsTab ? Colors.Black : Colors.Gray;
    }
}