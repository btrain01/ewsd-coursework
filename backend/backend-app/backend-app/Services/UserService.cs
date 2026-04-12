using backend_app.Context;
using backend_app.DTOs;
using backend_app.Enums;
using backend_app.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Collections.ObjectModel;
using System.Reflection;

namespace backend_app.Services
{ 
    public class UserService(ApplicationDBContext applicationDBContext, AuthenticationUserContext authenticationUserContext, AlertService alertService)
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

        public async Task<ICollection<StudentTutorDTO>> GetStudentsByTutorId(int userId)
        {
            var allocation = await applicationDBContext.TutorAssignments
                .Include(ts => ts.Student)
                .Where(ts => ts.TutorId == userId)
                .Select(ts => new StudentTutorDTO()
                {
                    Id = ts.Student.Id,
                    Name = $"{ts.Student.FirstName} {ts.Student.LastName}",
                    Email = ts.Student.Email,
                    Notes = ts.Notes,
                    AllocatedAt = ts.CreatedAt
                })
                .ToListAsync();

            if (allocation.Count == 0)
                return null;

            return allocation;
        }

        public async Task<TutorAssignment> AllocateTutor(AllocationDTO allocationDTO)
        {
            var allocation = new TutorAssignment()
            { 
                TutorId = allocationDTO.TutorId,
                StudentId = allocationDTO.StudentId,
                AllocatedBy = authenticationUserContext.UserId
            };

            applicationDBContext.TutorAssignments.Add(allocation);

            await applicationDBContext.SaveChangesAsync();

            await alertService.CreateAlert
            (
                [
                    new ()
                    {
                        Id = allocationDTO.StudentId,
                        Email = (await applicationDBContext.Users.FirstOrDefaultAsync(x => x.Id == allocationDTO.StudentId)).Email,
                    },
                    new ()
                    {
                        Id = allocationDTO.TutorId,
                        Email = (await applicationDBContext.Users.FirstOrDefaultAsync(x => x.Id == allocationDTO.TutorId)).Email,
                    }

                ],
                "Tutor Allocation"
            );

            return allocation;
        }

        public async Task<ICollection<TutorAssignment>> AllocateStudentsToTutor(BulkAllocationDTO bulkAllocationDTO)
        {
            var allocations = bulkAllocationDTO.Allocations.Select(a => new TutorAssignment
            {
                TutorId = a.TutorId,
                StudentId = a.StudentId,
                AllocatedBy = authenticationUserContext.UserId
            }).ToList();

            applicationDBContext.TutorAssignments.AddRange(allocations);
            await applicationDBContext.SaveChangesAsync();

            var studentIds = bulkAllocationDTO.Allocations.Select(a => a.StudentId).ToList();
            var tutorId = bulkAllocationDTO.Allocations.First().TutorId;
            var allIds = studentIds.Append(tutorId).ToList();

            var users = await applicationDBContext.Users
                .Where(u => allIds.Contains(u.Id))
                .Select(u => new { u.Id, u.Email })
                .ToListAsync();

            var userLookup = users.ToDictionary(u => u.Id, u => u.Email);

            var userDTOs = studentIds
                .Select(id => new UserDTO { Id = id, Email = userLookup[id] })
                .Append(new UserDTO { Id = tutorId, Email = userLookup[tutorId] })
                .ToList();

            await alertService.CreateAlert(userDTOs, "Tutor Allocation");

            return allocations;
        }

        public async Task<ICollection<User>> GetUnAllocatedStudents()
        {
            var unAllocated = await applicationDBContext.Users
                .Where(u => u.Role.Name == UserRole.STUDENT.GetDisplayName())
                .Where(u => !u.StudentAssignments.Any(a => a.StudentId == u.Id))
                .ToListAsync();

            if (unAllocated.Count == 0)
                return null;

            return unAllocated;
        }

        public async Task<ICollection<User>> GetInActiveUsers(int inactiveDays = 7)
        {
            var cutoff = DateTime.UtcNow.AddDays(-inactiveDays);

            var activeUserIds = await applicationDBContext.MeetingParticipants
                .Where(mp => mp.CreatedAt >= cutoff)
                .Select(mp => mp.UserId)
                .Union(
                    applicationDBContext.Messages
                        .Where(m => m.CreatedAt >= cutoff)
                        .Select(m => m.SenderId)
                )
                .Union(
                    applicationDBContext.Documents
                        .Where(d => d.CreatedAt >= cutoff)
                        .Select(d => d.UploaderId)
                )
                .ToListAsync();

            var inactiveUsers = await applicationDBContext.Users
                .Where(u => !activeUserIds.Contains(u.Id))
                .ToListAsync();

            return inactiveUsers.Count == 0 ? null : inactiveUsers;
        }

        public async Task<ICollection<User>> GetUsersByRole(int roleId)
        {
            var users = await applicationDBContext.Users
                .Where(u => u.RoleId == roleId)
                .ToListAsync();

            if (users.Count == 0)
                return null;

            return users;
        }

        public async Task<ICollection<User>> GetUsersByRole(string roleName)
        {
            var users = await applicationDBContext.Users
                .Where(u => EF.Functions.Like(u.Role.Name.ToLower(), $"%{roleName.ToLower()}%"))
                .ToListAsync();

            if (users.Count == 0)
                return null;

            return users;
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
                    FileName = d.Filename,
                    Description = d.Description,
                    MimeType = d.MimePath,
                    FileSizeBytes = d.FileSize,
                    UploadedAt = d.CreatedAt
                })
                .ToListAsync();

            return [.. documents];
        }

        public async Task<ObservableCollection<MessageResponseDTO>> GetUserMessages(int userId)
        {

            var messages = await applicationDBContext.Messages
                .Where(m => m.RecipientId == userId || m.SenderId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            var groupedMessages = messages
                .GroupBy(m =>
                {
                    var ids = new[] { m.SenderId, m.RecipientId }.Order();
                    return string.Join("_", ids);
                })
                .Select(g =>
                {
                    var otherUserId = g.First().SenderId == userId
                        ? g.First().RecipientId
                        : g.First().SenderId;

                    return new MessageResponseDTO()
                    {
                        Id = otherUserId,
                        Messages = [.. g.Select(m => new MessageDTO()
                        {
                            Id = m.Id,
                            SenderId = m.SenderId,
                            RecipientId = m.RecipientId,
                            Subject = m.Subject,
                            Body = m.Body,
                            IsRead = m.IsRead,
                            CreatedAt = m.CreatedAt
                        })]
                    };
                })
                .ToList();

            return [.. groupedMessages];
        }
    }
}