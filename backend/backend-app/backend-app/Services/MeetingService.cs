using backend_app.Context;
using backend_app.DTOs;
using backend_app.Enums;
using backend_app.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_app.Services
{
    public class MeetingService(ApplicationDBContext applicationDBContext)
    {
        public async Task<Meeting?> CreateMeeting(int organiserId, CreateMeetingDTO dto)
        {
            // Create the meeting
            var meeting = new Meeting
            {
                OrganiserId = organiserId,
                Title = dto.Title,
                Description = dto.Description,
                MeetingType = dto.MeetingType,
                MeetingStatus = MeetingStatus.Scheduled,
                ScheduledAt = dto.ScheduledAt,
                DurationMinutes = dto.DurationMinutes,
                LocationOrUrl = dto.LocationOrUrl
            };

            applicationDBContext.Meetings.Add(meeting);
            await applicationDBContext.SaveChangesAsync();

            // Collect all participant IDs
            // Start with the organiser, then add others avoiding duplicates
            var allParticipantIds = dto.ParticipantIds
                .Where(id => id != organiserId)
                .Distinct()
                .ToList();

            // Always add organiser first
            allParticipantIds.Insert(0, organiserId);

            // Create participant records
            var participants = allParticipantIds.Select(userId => new MeetingParticipant
            {
                MeetingId = meeting.Id,
                UserId = userId
            }).ToList();

            applicationDBContext.MeetingParticipants.AddRange(participants);
            await applicationDBContext.SaveChangesAsync();

            return await applicationDBContext.Meetings
                .Where(m => m.Id == meeting.Id)
                .FirstOrDefaultAsync();
        }
    }
}