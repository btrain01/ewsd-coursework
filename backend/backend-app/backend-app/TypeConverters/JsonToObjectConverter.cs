using AutoMapper;
using System.Text.Json;

namespace backend_app.TypeConverters
{
    public class JsonToObjectConverter<T> : ITypeConverter<string, T>
    {

        private static readonly JsonSerializerOptions options = new ()
        {
            PropertyNameCaseInsensitive = true
        };

        public T Convert(string source, T destination, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source))
                return default;

            return JsonSerializer.Deserialize<T>(source, options);
        }
    }
}
