using backend_app.Context;
using backend_app.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace backend_app.Services
{ 
    public class UserService(ApplicationDBContext applicationDBContext)
    {
        public async Task<UserDTO> GetUserDashboard(int id)
        {
            var student = await applicationDBContext.Users
                .Where(x => x.Id == id)
                .Select(x => new UserDTO()
                {
                    Id = x.Id,
                    FullName = $"{x.FirstName} {x.LastName}",
                    Email = x.Email,
                    Username = x.Username
                })
                .FirstOrDefaultAsync();

            if (student == null)
                return null;

            return student;
        }

        public async Task<StudentTutorDTO> GetTutorByStudentId(int userId)
        {
            var allocation = await applicationDBContext.TutorAssignments
                .Include(ts => ts.Tutor)
                .Where(ts => ts.StudentId == userId)
                .Select(ts => new StudentTutorDTO ()
                {
                    Id = ts.Tutor.Id,
                    Name = $"{ts.Tutor.FirstName} {ts.Tutor.LastName}",
                    Email = ts.Tutor.Email,
                    Notes = ts.Notes,
                    AllocatedAt = ts.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (allocation == null)
                return null;

            return allocation;
        }

        public async Task<StudentTutorDTO> GetStudentsByTutorId(int userId)
        {
            var allocation = await applicationDBContext.TutorAssignments
                .Include(ts => ts.Student)
                .Where(ts => ts.TutorId == userId)
                .Select(ts => new StudentTutorDTO ()
                {
                    Id = ts.Student.Id,
                    Name = $"{ts.Student.FirstName} {ts.Student.LastName}",
                    Email = ts.Student.Email,
                    Notes = ts.Notes,
                    AllocatedAt = ts.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (allocation == null)
                return null;

            return allocation;
        }

        public async Task<ObservableCollection<MeetingDTO>> GetUserMeetings(int userId)
        {
            var meetings = await applicationDBContext.Meetings
                .Where(m => m.Participants.FirstOrDefault(p => p.UserId == userId) != null)
                .Select(m => new MeetingDTO()
                {
                    Id = m.Id,
                    ScheduledAt = m.ScheduledAt,
                    DurationInMinutes = m.DurationMinutes,
                    MeetingType = m.MeetingType,
                    MeetingLink = m.LocationOrUrl,
                    Agenda = m.Title,
                    MeetingStatus = m.MeetingStatus
                })
                .ToListAsync();

            return [.. meetings];
        }

        public async Task<ObservableCollection<DocumentDTO>> GetUserDocuments(int userId)
        {
            var documents = await applicationDBContext.Documents
                .Include(d => d.Author)
                .Where(d => d.Author.Id == userId)
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

        public async Task<ObservableCollection<MessageDTO>> GetUserMessages(int userId)
        {
            var messages = await applicationDBContext.Messages
                .Where(m => m.ReceiverId == userId || m.SenderId == userId)
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