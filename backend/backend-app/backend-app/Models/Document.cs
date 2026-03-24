using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_app.Models
{
    [Table("documents")]
    public class Document : Actionable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UploaderId { get; set; }
        [Required]

        public int MeetingId { get; set; }
        
        [Required]
        public string FileName { get; set; }
        
        [Required]
        public string MimePath { get; set; }
        
        [Required]
        public int FileSize { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime? UploadedAt { get; set; }

        [ForeignKey(nameof(UploaderId))]
        public User Author { get; set; } = null!;

        [ForeignKey(nameof(MeetingId))]
        public Meeting Meeting { get; set; } = null!;
    }
}
