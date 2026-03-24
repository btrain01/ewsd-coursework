using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_app.Models
{
    [Table("tutor_assignments")]
    public class TutorAssignment : Actionable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int TutorId { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public DateTime? RevokedAt { get; set; }

        public int? RevokedBy { get; set; }

        public bool IsActive { get; set; } = true;

        public string? Notes { get; set; }

        [ForeignKey(nameof(StudentId))]
        public User Student { get; set; } = null!;

        [ForeignKey(nameof(TutorId))]
        public User Tutor { get; set; } = null!;

        [ForeignKey(nameof(RevokedBy))]
        public User? RevokedByUser { get; set; }
    }
}