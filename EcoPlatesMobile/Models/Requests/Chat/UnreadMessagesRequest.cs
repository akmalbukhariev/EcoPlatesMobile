public class UnreadMessagesRequest
{
    public long user_id { get; set; }
    public long receiver_id { get; set; }
    public string receiver_type { get; set; }
}