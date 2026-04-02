namespace backend_app.DTOs
{
    public class CreateBlogCommentDTO
    {
        public string Content { get; set; } = string.Empty;
        public int? ParentCommentId { get; set; }
    }
}