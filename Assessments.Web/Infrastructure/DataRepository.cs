using System.Globalization;
using System.Text.Json;
using Assessments.Mapping.AlienSpecies.Model;
using Assessments.Mapping.AlienSpecies.Source;
using Assessments.Mapping.RedlistSpecies;
using Assessments.Mapping.RedlistSpecies.Source;
using Assessments.Shared.Helpers;
using Assessments.Shared.Options;
using AutoMapper;
using Azure.Storage.Blobs;
using CsvHelper;
using CsvHelper.Configuration;
using LazyCache;
using Microsoft.Extensions.Options;

namespace Assessments.Web.Infrastructure
{
    public class DataRepository
    {
        private readonly IAppCache _appCache;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<DataRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IOptions<ApplicationOptions> _options;

        private static CsvConfiguration CsvConfiguration => new(CultureInfo.InvariantCulture) { Delimiter = ";" };

        public DataRepository(IAppCache appCache, IConfiguration configuration, IWebHostEnvironment environment, ILogger<DataRepository> logger, IMapper mapper, IOptions<ApplicationOptions> options)
        {
            _options = options;
            _mapper = mapper;
            _logger = logger;
            _environment = environment;
            _configuration = configuration;
            _appCache = appCache;
            _appCache.DefaultCachePolicy.DefaultCacheDurationSeconds = 86400; // 24 hours
        }

        public Task<IQueryable<T>> GetData<T>(string name)
        {
            return _appCache.GetOrAddAsync($"{nameof(DataRepository)}-{name}", DeserializeData);

            async Task<IQueryable<T>> DeserializeData()
            {
                var fileName = Path.Combine(_environment.ContentRootPath, Constants.CacheFolder, name);
                string fileContent;

                if (File.Exists(fileName)) // use cached file
                {
                    fileContent = await File.ReadAllTextAsync(fileName);
                    _logger.LogDebug("Using cached '{name}'", name);
                }
                else // download file
                {
                    _logger.LogDebug("Start downloading '{name}'", name);

                    var connectionString = _configuration["ConnectionStrings:AzureBlobStorage"];
                    
                    if (string.IsNullOrEmpty(connectionString)) 
                        throw new Exception("Missing required config for azure blog storage: ConnectionStrings:AzureBlobStorage");

                    var blob = new BlobContainerClient(connectionString, "assessments").GetBlobClient(name);
                    var response = await blob.DownloadContentAsync();

                    fileContent = response.Value.Content.ToString();

                    await using var writer = new StreamWriter(fileName);
                    await writer.WriteAsync(fileContent);

                    _logger.LogDebug("Download '{name}' complete, account name: {accountName}", name, blob.AccountName);
                }

                if (!name.EndsWith(".csv")) // handle json
                    return JsonSerializer.Deserialize<IList<T>>(fileContent)?.AsQueryable();

                using var reader = new StringReader(fileContent);
                using var csv = new CsvReader(reader, CsvConfiguration);

                return csv.GetRecords<T>().ToList().AsQueryable();
            }
        }

        public Task<IQueryable<SpeciesAssessment2021>> GetSpeciesAssessments()
        {
            var speciesAssessments = _appCache.GetOrAddAsync(nameof(GetSpeciesAssessments), Get);
            
            return speciesAssessments;

            async Task<IQueryable<SpeciesAssessment2021>> Get()
            {
                return _options.Value.Species2021.TransformAssessments
                    ?
                    // transformerer modell fra "Rodliste2019"
                    _mapper.Map<IEnumerable<SpeciesAssessment2021>>(await GetData<Rodliste2019>(DataFilenames.Species2021Temp)).AsQueryable() :
                    // returnerer modell som allerede er transformert
                    await GetData<SpeciesAssessment2021>(DataFilenames.Species2021);
            }
        }

        public Task<IQueryable<AlienSpeciesAssessment2023>> GetAlienSpeciesAssessments()
        {
            var alienSpeciesAssessments = _appCache.GetOrAddAsync(nameof(GetAlienSpeciesAssessments), Get);
            
            return alienSpeciesAssessments;

            async Task<IQueryable<AlienSpeciesAssessment2023>> Get()
            {
                return _options.Value.AlienSpecies2023.TransformAssessments
                    ?
                    // transformerer modell fra "FA4"
                    _mapper.Map<IEnumerable<AlienSpeciesAssessment2023>>(await GetData<FA4>(DataFilenames.AlienSpecies2023Temp)).AsQueryable() :
                    // returnerer modell som allerede er transformert
                    await GetData<AlienSpeciesAssessment2023>(DataFilenames.AlienSpecies2023);
            }
        }
    }
}