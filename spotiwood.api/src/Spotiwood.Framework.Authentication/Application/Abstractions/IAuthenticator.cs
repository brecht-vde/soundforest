namespace Spotiwood.Framework.Authentication.Application.Abstractions;
public interface IAuthenticator
{
    public Task<bool> ValidateAsync(string? token, CancellationToken cancellationToken);
}
