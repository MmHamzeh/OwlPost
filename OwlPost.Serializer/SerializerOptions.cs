using System.Text.Json;
using System.Text.Json.Serialization;

namespace OwlPost.Serializer;

public static class SerializerOptions
{
    public static JsonSerializerOptions JsonSerializerOptions
    {
        get
        {
            return field ??= new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = false,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
        }
    }
        
}