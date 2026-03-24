using backend_app.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_app.Context
{
    public class ApplicationDBContext(DbContextOptions<ApplicationDBContext> contextOptions) : DbContext(contextOptions)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
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

            /*modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.AssignedByUser)
                .WithMany()
                .HasForeignKey(ur => ur.AssignedBy)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TutorAssignment>()
                .HasOne(ta => ta.Student)
                .WithMany()
                .HasForeignKey(ta => ta.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TutorAssignment>()
                .HasOne(ta => ta.Tutor)
                .WithMany()
                .HasForeignKey(ta => ta.TutorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TutorAssignment>()
                .HasOne(ta => ta.RevokedByUser)
                .WithMany()
                .HasForeignKey(ta => ta.RevokedBy)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmailLog>()
                .HasOne(e => e.Recipient)
                .WithMany()
                .HasForeignKey(e => e.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmailLog>()
                .HasOne(e => e.Sender)
                .WithMany()
                .HasForeignKey(e => e.SenderId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DocumentComment>()
                .HasOne(dc => dc.ParentComment)
                .WithMany(dc => dc.Replies)
                .HasForeignKey(dc => dc.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BlogComment>()
                .HasOne(bc => bc.ParentComment)
                .WithMany(bc => bc.Replies)
                .HasForeignKey(bc => bc.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);*/
        }
    }
}