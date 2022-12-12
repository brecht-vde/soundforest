using AutoMapper;
using Microsoft.Extensions.Logging;
using Spotiwood.Integrations.Omdb.Application.Abstractions;
using Spotiwood.Integrations.Omdb.Domain;

namespace Spotiwood.Integrations.Omdb.Application.Clients;
internal sealed class BaseClient : IClient
{
    private readonly ILogger<BaseClient> _logger;
    private readonly IDbClient _client;
    private readonly IMapper _mapper;

    public BaseClient(ILogger<BaseClient> logger, IDbClient client, IMapper mapper)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<SearchResultCollection> SearchAsync(string query, int page, CancellationToken cancellationToken)
    {
        try
        {
            var dto = await _client.SearchAsync(query, page, cancellationToken);
            var result = _mapper.Map<SearchResultCollection>(dto);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not execute search.");
            return new();
        }
    }
}
