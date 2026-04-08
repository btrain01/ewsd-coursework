using backend_app.Enums;
using backend_app.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_app.Context
{
    public class ApplicationDBContext(DbContextOptions<ApplicationDBContext> contextOptions) : DbContext(contextOptions)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> UserRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<TutorAssignment> TutorAssignments { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<MeetingParticipant> MeetingParticipants { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentComment> DocumentComments { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}