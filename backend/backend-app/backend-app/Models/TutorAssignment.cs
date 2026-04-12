using Newtonsoft.Json;
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
        public int TutorId { get; set; }

        [Required]
        public int StudentId { get; set; }

        public int? AllocatedBy { get; set; }

        public string? Notes { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(StudentId))]
        [InverseProperty(nameof(User.StudentAssignments))]
        public User Student { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(TutorId))]
        [InverseProperty(nameof(User.TutorAssignments))]
        public User Tutor { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(AllocatedBy))]
        [InverseProperty(nameof(User.AllocatedAssignments))]
        public User? AllocatedByUser { get; set; }
    }
}