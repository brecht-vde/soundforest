using System.Text.Json;
using System.Text.Json.Serialization;

namespace Spotiwood.Integrations.Omdb.Application.Serialization;
internal static class Serialization
{
    // TODO: Find a better way to set defaults.
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
