namespace EcoPlatesMobile.Views.Chat.Templates;

public partial class MessageSenderView : ContentView
{
	public MessageSenderView()
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