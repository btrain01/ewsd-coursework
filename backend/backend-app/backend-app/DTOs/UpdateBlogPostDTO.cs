using backend_app.Enums;

namespace backend_app.DTOs
{
    public class UpdateBlogPostDTO
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }
}