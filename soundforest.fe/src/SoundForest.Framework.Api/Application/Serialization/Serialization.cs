using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoundForest.Framework.Api.Application.Serialization;
internal static class Serialization
{
    // TODO: replace with IOptions pattern
    public static JsonSerializerOptions DefaultOptions
    {
        get
        {
            var defaultOptions = new JsonSerializerOptions();
            defaultOptions.Converters.Add(new JsonStringEnumConverter());
            defaultOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            defaultOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
            defaultOptions.PropertyNameCaseInsensitive = true;
            return defaultOptions;
        }
    }
}
