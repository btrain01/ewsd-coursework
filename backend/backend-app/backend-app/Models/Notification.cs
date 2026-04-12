using backend_app.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_app.Models
{
    [Table("notifications")]
    public class Notification : Actionable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Column(TypeName = "notification_type")]
        public NotificationType Type { get; set; }

        [Required, MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;
    }
}