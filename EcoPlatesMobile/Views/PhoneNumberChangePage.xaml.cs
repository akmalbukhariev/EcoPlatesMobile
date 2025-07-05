using EcoPlatesMobile.Resources.Languages;

namespace EcoPlatesMobile.Views;

public partial class PhoneNumberChangePage : BasePage
{
	public PhoneNumberChangePage()
	{
		InitializeComponent();

        lbTitle.Text = AppResource.ChangeNumberTitle;
    }

    private async void Next_Clicked(object sender, EventArgs e)
    {
        await AppNavigatorService.NavigateTo(nameof(PhoneNumberNewPage));
    }
}