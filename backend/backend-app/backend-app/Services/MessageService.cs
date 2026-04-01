using AutoMapper;
using backend_app.Context;
using backend_app.DTOs;
using backend_app.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace backend_app.Services
{
    public class MessageService(
        ApplicationDBContext applicationDBContext,
        AuthenticationUserContext authenticationUserContext,
        MessageChannelService messageChannelService,
        IMapper mapper)
    {
        public async Task<ActionResponseDTO> CreateMessage(CreateMessageDTO messageDTO)
        {
            var message = mapper.Map<Message>(messageDTO);

            applicationDBContext.Messages.Add(message);
            
            await applicationDBContext.SaveChangesAsync();

            var writer = messageChannelService.GetWriterForUser(messageDTO.RecipientId);

            await writer.WriteAsync(messageDTO);

            return new()
            {
                Result = true,
                Message = "Message successfully sent."
            };
        }

        /*public async Task<ActionResponseDTO> MarkMessageAsRead(ReadMessageDTO readMessageDTO)
        {
            var message = await applicationDBContext.Messages
                .Where(m => m.Id == readMessageDTO.MessageId && m.RecipientId == readMessageDTO.RecipientId)
                .FirstOrDefaultAsync();

            if (message == null)
                return null;
            
            message.IsRead = true;

            await applicationDBContext.SaveChangesAsync();

            return new()
            {
                Result = true,
                Message = "Message Marked as Read."
            };

        }

        public IResult OpenStream(ChannelReader<CreateMessageDTO> channelReader, CancellationToken cancellationToken)
        {
            var messages = GetUserMessages(channelReader, cancellationToken);
            
            return Results.ServerSentEvents(messages, eventType: "messages");
        }

        private async IAsyncEnumerable<CreateMessageDTO> GetUserMessages(ChannelReader<CreateMessageDTO> channelReader, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var userId = authenticationUserContext.UserId;

            await foreach (var message in channelReader.ReadAllAsync(cancellationToken))
            {
                if (message.RecipientId == userId)
                    yield return message;
            }
        }*/

        public IResult OpenStream(CancellationToken cancellationToken)
        {
            var userId = authenticationUserContext.UserId;
            var reader = messageChannelService.GetReaderForUser(userId);
            var messages = StreamForUser(reader, userId, cancellationToken);

            return Results.ServerSentEvents(messages, eventType: "messages");
        }

        private async IAsyncEnumerable<CreateMessageDTO> StreamForUser(
            ChannelReader<CreateMessageDTO> reader,
            int userId,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            try
            {
                await foreach (var msg in reader.ReadAllAsync(cancellationToken))
                    yield return msg;
            }
            finally
            {
                messageChannelService.CloseUserChannel(userId);
            }
        }

        public async Task<ActionResponseDTO> MarkMessageAsRead(ReadMessageDTO dto)
        {
            var message = await applicationDBContext.Messages
                .Where(m => m.Id == dto.MessageId && m.RecipientId == dto.RecipientId)
                .FirstOrDefaultAsync();

            if (message == null) return null;

            message.IsRead = true;
            await applicationDBContext.SaveChangesAsync();

            return new() { Result = true, Message = "Message marked as read." };
        }
    }
}
