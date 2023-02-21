using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoundForest.UnitTests.Common;
public static class DefaultJsonSerializerOptions
{
    public static JsonSerializerOptions Options
    {
        get
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.PropertyNameCaseInsensitive = true;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
            return options;
        }
    }
}
