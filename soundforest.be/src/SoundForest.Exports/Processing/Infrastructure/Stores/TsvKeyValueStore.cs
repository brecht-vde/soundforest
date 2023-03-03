using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoundForest.Exports.Processing.Application.Stores;
using System.IO.Compression;

namespace SoundForest.Exports.Processing.Infrastructure.Stores;
internal sealed class TsvKeyValueStore : IKeyValueStore<IEnumerable<string>?>
{
    private readonly ILogger<TsvKeyValueStore> _logger;
    private readonly HttpClient _client;
    private readonly IOptions<TsvOptions> _options;
    private readonly IList<KeyValuePair<string, string>> _items = new List<KeyValuePair<string, string>>();
    private static string _filePath = Path.Combine(Path.GetTempPath(), "file.tsv");


    public TsvKeyValueStore(ILogger<TsvKeyValueStore> logger, HttpClient client, IOptions<TsvOptions> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _options = options ?? throw new ArgumentNullException(nameof(options));

        if (options?.Value?.FileUri is null)
            throw new ArgumentNullException(nameof(TsvOptions.FileUri));
    }

    public IEnumerable<string>? Find(string? key)
    {
        return _items?
            .Where(kvp => kvp.Key.Equals(key, StringComparison.OrdinalIgnoreCase))?
            .Select(kvp => kvp.Value);
    }

    public async Task LoadAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_items?.Any() is true)
                return;

            using var fs = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            if (fs.Length is 0)
            {
                using var ds = await _client.GetStreamAsync(_options?.Value?.FileUri, cancellationToken);
                using var gs = new GZipStream(ds, CompressionMode.Decompress);

                await gs.CopyToAsync(fs, cancellationToken);
                fs.Position = 0;
            }

            using var sr = new StreamReader(fs);

            var line = sr.ReadLine();

            while ((line = sr.ReadLine()) is not null)
            {
                var data = line.Split('\t');
                var episodeId = data[0];
                var titleId = data[1];

                _items?.Add(new KeyValuePair<string, string>(titleId, episodeId));
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Could not load .tsv file.");
            throw;
        }
    }
}
