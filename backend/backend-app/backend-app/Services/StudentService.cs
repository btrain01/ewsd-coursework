using backend_app.Context;
using backend_app.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace backend_app.Services
{
    public class StudentService(ApplicationDBContext applicationDBContext)
    {
        public async Task<StudentDTO> GetStudentDashboard(int studentId)
        {
            var student = await applicationDBContext.Users
                .Where(x => x.Id == studentId)
                .Select(x => new StudentDTO()
                {
                    Id = x.Id,
                    FullName = $"{x.FirstName} {x.FirstName}",
                    Email = x.Email,
                    Username = x.Username
                })
                .FirstOrDefaultAsync();

            if (student == null)
                return null;

            return student;
        }

        public async Task<StudentTutorDTO> GetStudentTutor(int studentId)
        {
            var allocation = await applicationDBContext.TutorAssignments
                .Include(ts => ts.Tutor)
                .Where(ts => ts.StudentId == studentId)
                .Select(ts => new StudentTutorDTO ()
                {
                    TutorId = ts.Tutor.Id,
                    TutorName = $"{ts.Tutor.FirstName} {ts.Tutor.FirstName}",
                    TutorEmail = ts.Tutor.Email,
                    TutorNotes = ts.Notes,
                    AllocatedAt = ts.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (allocation == null)
                return null;

            return allocation;
        }

        public async Task<ObservableCollection<MeetingDTO>> GetStudentMeetings(int studentId)
        {
            var meetings = await applicationDBContext.Meetings
                .Where(m => m.Participants.FirstOrDefault(p => p.UserId == studentId) != null)
                .Select(m => new MeetingDTO()
                {
                    Id = m.Id,
                    ScheduledAt = m.ScheduledAt,
                    DurationInMinutes = m.DurationMins,
                    MeetingType = m.MeetingType,
                    MeetingLink = m.LocationOrUrl,
                    Agenda = m.Title,
                    MeetingStatus = m.MeetingStatus
                })
                .ToListAsync();

            return [.. meetings];
        }

        public async Task<ObservableCollection<DocumentDTO>> GetStudentDocuments(int studentId)
        {
            var documents = await applicationDBContext.Documents
                .Include(d => d.Author)
                .Where(d => d.Author.Id == studentId)
                .Select(d => new DocumentDTO()
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    Description = d.Description,
                    MimeType = d.MimePath,
                    FileSizeBytes = d.FileSize,
                    UploadedAt = d.CreatedAt
                })
                .ToListAsync();

            return [.. documents];
        }

        public async Task<ObservableCollection<MessageDTO>> GetStudentMessages(int studentId)
        {
            var messages = await applicationDBContext.Messages
                .Where(m => m.ReceiverId == studentId || m.SenderId == studentId)
                .Select(m => new MessageDTO()
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    RecipientId = m.ReceiverId,
                    Subject = m.Subject,
                    Body = m.Content,
                    IsRead = m.IsRead,
                    CreatedAt = m.CreatedAt
                })
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
 
            return [.. messages];
        }
    }
}