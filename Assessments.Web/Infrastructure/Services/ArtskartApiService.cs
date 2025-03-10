namespace Assessments.Web.Infrastructure.Services
{
    public class ArtskartApiService
    {
        private readonly HttpClient _client;
        private readonly ILogger<ArtskartApiService> _logger;

        public ArtskartApiService(HttpClient client, ILogger<ArtskartApiService> logger)
        {
            _logger = logger;
            _client = client;
            _client.BaseAddress = new Uri("https://artskart.artsdatabanken.no/appapi/api/");
            _client.Timeout = TimeSpan.FromSeconds(5);
        }

        public async Task<T> Get<T>(string path)
        {
            try
            {
                return await _client.GetFromJsonAsync<T>(path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                return default;
            }
        }
    }

    public class ArtskartTaxon
    {
        public int ScientificNameId { get; set; }
        public string PopularName { get; set; }
        public string MatchedName { get; set; }
        public string ScientificName { get; set; }
        public int TaxonCategory { get; set; }
        public Array assessments { get; set; }
    }
}