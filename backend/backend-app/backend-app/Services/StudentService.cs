using backend_app.Context;
using Microsoft.EntityFrameworkCore;

namespace backend_app.Services
{
    public class StudentService(ApplicationDBContext applicationDBContext)
    {
        public async Task<object> GetStudentDashboard(int studentId)
        {
            var student = await applicationDBContext.Users
                .Where(x => x.Id == studentId)
                .Select(x => new
                {
                    id = x.Id,
                    name = x.FirstName + " " + x.LastName,
                    email = x.Email,
                    misReference = x.Username
                })
                .FirstOrDefaultAsync();

            if (student == null)
                return null;

            return student;
        }

        public async Task<object> GetStudentTutor(int studentId)
        {
            var allocation = await applicationDBContext.TutorStudents
                .Include(ts => ts.Tutor)
                .Where(ts => ts.StudentId == studentId)
                .Select(ts => new
                {
                    tutorId = ts.Tutor.Id,
                    tutorName = ts.Tutor.FirstName + " " + ts.Tutor.LastName,
                    tutorEmail = ts.Tutor.Email,
                    notes = ts.Notes,
                    allocatedAt = ts.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (allocation == null)
                return null;

            return allocation;
        }

        public async Task<object> GetStudentMeetings(int studentId)
        {
            var allocation = await applicationDBContext.TutorStudents
                .Where(ts => ts.StudentId == studentId)
                .FirstOrDefaultAsync();

            if (allocation == null)
                return null;

            var meetings = await applicationDBContext.Meetings
                .Where(m => m.ScheduledBy == allocation.TutorId)
                .Select(m => new
                {
                    id = m.Id,
                    scheduledAt = m.ScheduledAt,
                    durationMinutes = m.DurationMinutes,
                    meetingType = m.MeetingType,
                    meetingLink = m.MeetingLink,
                    agenda = m.Agenda,
                    status = m.Status
                })
                .ToListAsync();

            return meetings;
        }

        public async Task<object> GetStudentDocuments(int studentId)
        {
            var documents = await applicationDBContext.Documents
                .Include(d => d.TutorStudent)
                .Where(d => d.TutorStudent.StudentId == studentId)
                .Select(d => new
                {
                    id = d.Id,
                    filename = d.Filename,
                    description = d.Description,
                    mimeType = d.MimeType,
                    fileSizeBytes = d.FileSizeBytes,
                    uploadedAt = d.CreatedAt
                })
                .ToListAsync();

            return documents;
        }

        public async Task<object> GetStudentMessages(int studentId)
        {
            var messages = await applicationDBContext.Messages
                .Where(m => m.RecipientId == studentId || m.SenderId == studentId)
                .Select(m => new
                {
                    id = m.Id,
                    senderId = m.SenderId,
                    recipientId = m.RecipientId,
                    subject = m.Subject,
                    body = m.Body,
                    isRead = m.IsRead,
                    createdAt = m.CreatedAt
                })
                .OrderByDescending(m => m.createdAt)
                .ToListAsync();
 
            return messages;
        }
    }
}