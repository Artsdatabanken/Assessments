using System.Net;
using System.Net.Http.Json;
using Assessments.Shared.DTOs.NinKode;
using Assessments.Shared.Interfaces;
using Assessments.Shared.Options;
using LazyCache;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Assessments.Shared.Repositories;

public class NinKodeRepository : INinKodeRepository
{
    private readonly HttpClient _client;
    private readonly IAppCache _cache;
    private readonly ILogger<NinKodeRepository> _logger;

    public NinKodeRepository(HttpClient client, IOptions<ApplicationOptions> options, IAppCache cache, ILogger<NinKodeRepository> logger)
    {
        _logger = logger;
        _cache = cache;
        _client = client;
        _client.Timeout = TimeSpan.FromSeconds(10);
        _client.BaseAddress = options.Value.NinKodeApiUrl;
    }

    public async Task<List<VariablerResponseDto>> VariablerAlleKoder(CancellationToken cancellationToken = default)
    {
        const string cacheKey = nameof(VariablerAlleKoder);
        const string path = "variabler/allekoder";

        HttpStatusCode responseStatusCode = new();
        
        try
        {
            return await _cache.GetOrAddAsync(cacheKey, async () =>
            {
                var response = await _client.GetAsync(path, cancellationToken);

                responseStatusCode = response.StatusCode;
                response.EnsureSuccessStatusCode();

                var responseDto = await response.Content.ReadFromJsonAsync<NinKodeResponseDto>(cancellationToken: cancellationToken);
                
                return responseDto?.Variabler;
            });
        }
        catch (Exception ex)
        {
            _cache.Remove(cacheKey);
            _logger.LogCritical("Could not get '{path}' (StatusCode: {statuscode}, Message: {message})", path, responseStatusCode, ex.Message);

            return null;
        }
    }
}