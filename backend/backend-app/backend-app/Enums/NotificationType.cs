using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace backend_app.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum NotificationType
    {
        MESSAGE,
        EMAIL
    }
}
