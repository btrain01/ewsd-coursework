using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_app.Models
{
    [Table("audit_logs")]
    public class AuditLog : Actionable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public int ActorId { get; set; }

        [Required, MaxLength(100)]
        public string Action { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string TargetTable { get; set; } = string.Empty;

        public int? TargetId { get; set; }

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ActorId))]
        public User Actor { get; set; } = null!;
    }
}