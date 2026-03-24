using System.ComponentModel.DataAnnotations.Schema;

namespace backend_app.Models
{
    [Table("users")]
    public class User : Actionable
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {get; set;}
        public string Username {get; set;} = string.Empty;
        public string FirstName {get; set;} = string.Empty;
        public string LastName {get; set;} = string.Empty;
        public string Email {get; set;} = string.Empty;
        public string Password {get; set;} = string.Empty;
        public bool IsActive {get; set;}

        public ICollection<UserRole> UserRoles { get; set; } = [];
        public ICollection<Message> SentMessages { get; set; } = [];
        public ICollection<Message> ReceivedMessages { get; set; } = [];
        public ICollection<Meeting> OrganisedMeetings { get; set; } = [];
        public ICollection<MeetingParticipant> MeetingParticipants { get; set; } = [];
        public ICollection<Notification> Notifications { get; set; } = [];
        public ICollection<BlogPost> BlogPosts { get; set; } = [];
        public ICollection<BlogComment> BlogComments { get; set; } = [];
        public ICollection<DocumentComment> DocumentComments { get; set; } = [];
        public ICollection<Document> Documents { get; set; } = [];
        public ICollection<Document> UploadedDocuments { get; set; } = [];
        public ICollection<AuditLog> AuditLogs { get; set; } = [];
    }
}