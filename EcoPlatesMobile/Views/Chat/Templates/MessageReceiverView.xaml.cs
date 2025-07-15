namespace EcoPlatesMobile.Views.Chat.Templates;

public partial class MessageReceiverView : ContentView
{
	public MessageReceiverView()
	{
		InitializeComponent();
	}

    private async void Message_Tapped(object sender, EventArgs e)
    {
        Grid grid = (Grid)sender;
        await grid.ScaleTo(0.9, 200);
        await grid.ScaleTo(1, 200, Easing.SpringOut);
    }
}