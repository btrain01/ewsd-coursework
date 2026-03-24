using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_app.Models
{
    [Table("document_comments")]
    public class DocumentComment : Actionable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int DocumentId { get; set; }

        [MaxLength(150)]
        public string? Author { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public int? ParentCommentId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(DocumentId))]
        public Document Document { get; set; } = null!;

        [ForeignKey(nameof(AuthorId))]
        public User AuthorUser { get; set; } = null!;

        [ForeignKey(nameof(ParentCommentId))]
        public DocumentComment? ParentComment { get; set; }

        public ICollection<DocumentComment> Replies { get; set; } = new List<DocumentComment>();
    }
}