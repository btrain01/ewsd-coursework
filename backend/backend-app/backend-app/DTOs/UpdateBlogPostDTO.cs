using backend_app.Enums;

namespace backend_app.DTOs
{
    public class UpdateBlogPostDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public BlogPostStatus Status { get; set; }
    }
}