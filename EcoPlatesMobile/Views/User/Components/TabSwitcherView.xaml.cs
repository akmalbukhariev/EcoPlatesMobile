namespace EcoPlatesMobile.Views.User.Components;

public partial class TabSwitcherView : ContentView
{
    private string _selectedTab = "Sotuvchilar";

    public TabSwitcherView()
    {
        InitializeComponent();
    }

    private async void OnTabTapped(object sender, EventArgs e)
    {
        if (sender is Label tappedLabel)
        {
            string tabName = tappedLabel.Text;

            if (_selectedTab != tabName)
            {
                _selectedTab = tabName;

                TabSotuvchilar.TextColor = tabName == "Sotuvchilar" ? Colors.Black : Colors.Gray;
                TabMahsulotlar.TextColor = tabName == "Mahsulotlar" ? Colors.Black : Colors.Gray;

                double targetX = tabName == "Sotuvchilar" ? 5 : 190;
                await SelectionIndicator.TranslateTo(targetX, 0, 200, Easing.SinInOut);
            }
        }
    }
}