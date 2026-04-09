namespace backend_app.DTOs
{
    public class CreateMessageDTO
    {
        public int RecipientId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
