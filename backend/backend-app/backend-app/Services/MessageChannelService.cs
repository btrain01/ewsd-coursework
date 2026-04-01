using backend_app.DTOs;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace backend_app.Services
{
    public class MessageChannelService
    {
        private readonly ConcurrentDictionary<int, Channel<CreateMessageDTO>> _userChannels = new();

        public ChannelWriter<CreateMessageDTO> GetGlobalWriter()
            => throw new InvalidOperationException(
                "Use GetWriterForUser(userId) instead of injecting ChannelWriter directly.");

        public ChannelWriter<CreateMessageDTO> GetWriterForUser(int userId)
            => GetOrCreate(userId).Writer;

        public ChannelReader<CreateMessageDTO> GetReaderForUser(int userId)
            => GetOrCreate(userId).Reader;

        public void CloseUserChannel(int userId)
        {
            if (_userChannels.TryRemove(userId, out var channel))
                channel.Writer.TryComplete();
        }

        private Channel<CreateMessageDTO> GetOrCreate(int userId)
        {
            return _userChannels.GetOrAdd(userId, _ =>
                Channel.CreateUnbounded<CreateMessageDTO>(new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = false
                }));
        }
    }
}
