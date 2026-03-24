using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_app.Models
{
    [Table("email_logs")]
    public class EmailLog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int RecipientId { get; set; }

        public int? SenderId { get; set; }

        [Required, MaxLength(100)]
        public string EventType { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Subject { get; set; }

        [MaxLength(100)]
        public string? TemplateUsed { get; set; }

        public string? Payload { get; set; }

        public DateTime? IsDeliveredAt { get; set; }

        public string? ErrorMessage { get; set; }

        [ForeignKey(nameof(RecipientId))]
        public User Recipient { get; set; } = null!;

        [ForeignKey(nameof(SenderId))]
        public User? Sender { get; set; }
    }
}