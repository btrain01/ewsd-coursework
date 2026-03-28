using AutoMapper;
using System.Text.Json;

namespace backend_app.TypeConverters
{
    public class JsonToObjectConverter<T> : ITypeConverter<string, T>
    {
        public T Convert(string source, T destination, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source))
                return default;

            var deserialized = JsonSerializer.Deserialize<T>(source, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return deserialized;
        }
    }
}
