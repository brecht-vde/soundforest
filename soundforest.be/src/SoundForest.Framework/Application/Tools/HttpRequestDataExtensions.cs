using Microsoft.Azure.Functions.Worker.Http;
using System.IdentityModel.Tokens.Jwt;

namespace SoundForest.Framework.Application.Tools;
public static class HttpRequestDataExtensions
{
    public static string? ExtractUserName(this HttpRequestData request)
    {
        try
        {
            request.Headers.TryGetValues("Authorization", out IEnumerable<string>? values);
            var jwt = values?.FirstOrDefault()?.Replace("Bearer", "", StringComparison.OrdinalIgnoreCase)?.Trim();
            var token = new JwtSecurityToken(jwt);
            var user = token?.Subject?.Split(":")?.LastOrDefault();
            return user;
        }
        catch
        {
            return null;
        }
    }
}
