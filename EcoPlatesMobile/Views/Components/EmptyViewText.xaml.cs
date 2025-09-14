using AndroidX.Emoji2.Text.FlatBuffer;
using EcoPlatesMobile.Services;

namespace EcoPlatesMobile.Views.Components;

public partial class EmptyViewText : ContentView
{ 
    private UserSessionService userSessionService;
    public EmptyViewText()
    {
        InitializeComponent();

        userSessionService = AppService.Get<UserSessionService>();

        if (userSessionService.Role == UserRole.User)
        {
            image.Source = "empty_list_user.png";
            label.TextColor = EcoPlatesMobile.Utilities.Constants.COLOR_USER;
        }
        else
        {
            image.Source = "empty_list_company.png";
            label.TextColor = EcoPlatesMobile.Utilities.Constants.COLOR_COMPANY;
        }
        
    }
}