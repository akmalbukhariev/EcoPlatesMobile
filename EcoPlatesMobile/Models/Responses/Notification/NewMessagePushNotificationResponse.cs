namespace EcoPlatesMobile.Models.Responses.Notification
{
    public class NewMessagePushNotificationResponse
    {
        public NotificationType notificationType;
        public int sender_id;
        public string sender_name;
        public string sender_image;
        public string sender_type;
        public string sender_phone;
        public int receiver_id;
        public string receiver_phone;
        public string receiver_type;
        public string message;
    }
}