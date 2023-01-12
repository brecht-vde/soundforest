using SoundForest.Clients.Auth0.Authentication.Domain;

namespace SoundForest.Clients.Auth0.Authentication.Application;
public interface IAuth0Client
{
    public Task<Token?> M2mTokenAsync(CancellationToken cancellationToken = default);

    public Task<User?> UserTokenAsync(string m2mToken, string username, CancellationToken cancellationToken = default);
}
