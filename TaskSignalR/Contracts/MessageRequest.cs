namespace TaskSignalR.Contracts
{
    public class MessageRequest
    {
        public string Content { get; set; }
        public int ChatId { get; set; }
        public int UserId { get; set; }
    }
}
