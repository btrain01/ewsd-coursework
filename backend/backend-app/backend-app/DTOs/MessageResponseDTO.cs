namespace backend_app.DTOs
{
    public class MessageResponseDTO
    {
        public int Id { get; set; }

        public List<MessageDTO> Messages { get; set; }  
    }
}
