namespace SoundForest.Exports.Processing.Infrastructure.Exporters.Services;
internal interface ITokenService
{
    public Task<string?> GetUserAccessToken(string username, CancellationToken cancellationToken = default);
    public Task<string?> GetM2mAccesToken(CancellationToken cancellationToken = default);

}
