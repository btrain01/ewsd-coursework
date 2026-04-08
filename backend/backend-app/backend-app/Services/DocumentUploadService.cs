using AutoMapper;
using backend_app.Context;
using backend_app.DTOs;
using backend_app.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_app.Services
{
    public class DocumentUploadService(ApplicationDBContext applicationDBContext, AuthenticationUserContext authenticationUserContext, IMapper mapper)
    {

        public async Task<string> UploadFile(UploadRequest uploadRequest)
        {
            var file = uploadRequest.File;

            byte[] fileContentAsByteArray = [];

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                fileContentAsByteArray = ms.ToArray();
            } 

            var document = new Document()
            {
                UploaderId = authenticationUserContext.UserId,
                MeetingId = uploadRequest.MeetingId,
                Filename = file.FileName,
                MimePath = file.ContentType,
                Content = Convert.ToBase64String(fileContentAsByteArray),
            };

            applicationDBContext.Documents.Add(document);

            await applicationDBContext.SaveChangesAsync();

            return "Success";
        }

        public async Task<Document> DownloadFile(int documentId)
        {
            var fileContent = await applicationDBContext.Documents
                .FirstOrDefaultAsync(file => file.Id == documentId);

            if (fileContent == null) return null;

            return fileContent;
        }

        public async Task<List<DocumentResponse>> GetAllDocumentRecords()
        {
            var fileContent = await applicationDBContext.Documents
                .Select(document => new DocumentResponse()
                {
                    Id = document.Id,
                    UploaderId = document.UploaderId,
                    MeetingId = document.MeetingId,
                    FileName = document.Filename,
                    MimePath = document.MimePath,
                    CreatedAt = document.CreatedAt,
                    UpdatedAt = document.UpdatedAt
                })
                .Where(file => file.UploaderId == authenticationUserContext.UserId)
                .ToListAsync();

            return fileContent;
        }

        public async Task<List<DocumentResponse>> GetAllDocumentRecordsByMeetingId(int meetingId)
        {
            var fileContent = await applicationDBContext.Documents
                .Select(document => new DocumentResponse()
                {
                    Id = document.Id,
                    UploaderId = document.UploaderId,
                    MeetingId = document.MeetingId,
                    FileName = document.Filename,
                    MimePath = document.MimePath,
                    CreatedAt = document.CreatedAt,
                    UpdatedAt = document.UpdatedAt
                })
                .Where(file => file.MeetingId == meetingId)
                .ToListAsync();

            return fileContent;
        }
    }
}
