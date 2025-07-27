namespace EcoPlatesMobile.Services
{ 
    public interface INotificationService
    {
        void SendNotification(string title, string body);
    }
}