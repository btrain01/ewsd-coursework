using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_app.Models
{
    [Table("blog_comments")]
    public class BlogComment : Actionable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PostId { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public int? ParentCommentId { get; set; }

        
        [ForeignKey(nameof(PostId))]
        public BlogPost Post { get; set; } = null!;

        [ForeignKey(nameof(AuthorId))]
        public User Author { get; set; } = null!;

        [ForeignKey(nameof(ParentCommentId))]
        public BlogComment? ParentComment { get; set; }
        public ICollection<BlogComment> Replies { get; set; } = new List<BlogComment>();

    }
}