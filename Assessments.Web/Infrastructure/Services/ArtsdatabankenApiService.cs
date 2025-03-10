using LazyCache;

namespace Assessments.Web.Infrastructure.Services
{
    public class ArtsdatabankenApiService
    {
        private readonly HttpClient _client;
        private readonly IAppCache _appCache;
        private readonly ILogger<ArtsdatabankenApiService> _logger;

        public ArtsdatabankenApiService(HttpClient client, IAppCache appCache, ILogger<ArtsdatabankenApiService> logger)
        {
            _logger = logger;
            _client = client;
            _client.BaseAddress = new Uri("https://artsdatabanken.no/api/");
            _client.Timeout = TimeSpan.FromSeconds(15);
            
            _appCache = appCache;
            _appCache.DefaultCachePolicy.DefaultCacheDurationSeconds = 3600; // 1 time mellomlager
        }

        public async Task<T> Get<T>(string path)
        {
            try
            {
                return await _appCache.GetOrAddAsync(path, () => _client.GetFromJsonAsync<T>(path));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                return default;
            }
        }
    }

    public class ArtsdatabankenApiContent
    {
        public ArtsdatabankenApiContentReference[] References { get; set; }
    }

    public class ArtsdatabankenApiContentReference
    {
        public string Heading { get; set; }
        
        public string Url { get; set; }
    }
}