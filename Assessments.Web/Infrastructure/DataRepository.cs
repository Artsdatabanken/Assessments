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

namespace Assessments.Web.Infrastructure;

public class DataRepository(IAppCache appCache, IConfiguration configuration, IWebHostEnvironment environment, ILogger<DataRepository> logger, IMapper mapper, IOptions<ApplicationOptions> options)
{
    private static CsvConfiguration CsvConfiguration => new(CultureInfo.InvariantCulture) { Delimiter = ";" };

    public Task<IQueryable<T>> GetData<T>(string name)
    {
        return appCache.GetOrAddAsync($"{nameof(DataRepository)}-{name}", DeserializeData);

        async Task<IQueryable<T>> DeserializeData()
        {
            var fileName = Path.Combine(environment.ContentRootPath, Constants.CacheFolder, name);
            string fileContent;

            if (File.Exists(fileName)) // use cached file
            {
                fileContent = await File.ReadAllTextAsync(fileName);
                logger.LogDebug("Using cached '{name}'", name);
            }
            else // download file
            {
                logger.LogDebug("Start downloading '{name}'", name);

                var connectionString = configuration["ConnectionStrings:AzureBlobStorage"];
                    
                if (string.IsNullOrEmpty(connectionString)) 
                    throw new Exception("Missing required config for azure blog storage: ConnectionStrings:AzureBlobStorage");

                var blob = new BlobContainerClient(connectionString, "assessments").GetBlobClient(name);
                var response = await blob.DownloadContentAsync();

                fileContent = response.Value.Content.ToString();

                await using var writer = new StreamWriter(fileName);
                await writer.WriteAsync(fileContent);

                logger.LogDebug("Download '{name}' complete, account name: {accountName}", name, blob.AccountName);
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
        var speciesAssessments = appCache.GetOrAddAsync(nameof(GetSpeciesAssessments), Get);
            
        return speciesAssessments;

        async Task<IQueryable<SpeciesAssessment2021>> Get()
        {
            return options.Value.TransformAssessments
                ?
                // transformerer modell fra "Rodliste2019"
                mapper.Map<IEnumerable<SpeciesAssessment2021>>(await GetData<Rodliste2019>(DataFilenames.Species2021Temp)).AsQueryable() :
                // returnerer modell som allerede er transformert
                await GetData<SpeciesAssessment2021>(DataFilenames.Species2021);
        }
    }

    public Task<IQueryable<AlienSpeciesAssessment2023>> GetAlienSpeciesAssessments()
    {
        var alienSpeciesAssessments = appCache.GetOrAddAsync(nameof(GetAlienSpeciesAssessments), Get);
            
        return alienSpeciesAssessments;

        async Task<IQueryable<AlienSpeciesAssessment2023>> Get()
        {
            return options.Value.TransformAssessments
                ?
                // transformerer modell fra "FA4"
                mapper.Map<IEnumerable<AlienSpeciesAssessment2023>>(await GetData<FA4>(DataFilenames.AlienSpecies2023Temp)).AsQueryable() :
                // returnerer modell som allerede er transformert
                await GetData<AlienSpeciesAssessment2023>(DataFilenames.AlienSpecies2023);
        }
    }
}