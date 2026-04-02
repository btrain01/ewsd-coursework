namespace backend_app.DTOs
{
    public class CreateBlogPostDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}