
using CommunityToolkit.Mvvm.ComponentModel;
using EcoPlatesMobile.Models.Chat;

public partial class SenderIdInfo : ObservableObject
{
    [ObservableProperty] private string userImage;
    [ObservableProperty] private string userName;
    public ChatPageModel chatPageModel; 
}