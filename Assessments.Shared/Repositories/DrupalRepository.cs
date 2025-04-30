using Assessments.Shared.Interfaces;
using LazyCache;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;
using Assessments.Shared.DTOs.Drupal;
using Assessments.Shared.DTOs.Drupal.Enums;

namespace Assessments.Shared.Repositories;

public class DrupalRepository : IDrupalRepository
{
    private readonly HttpClient _client;
    private readonly IAppCache _cache;
    private readonly ILogger<DrupalRepository> _logger;
    
    public DrupalRepository(HttpClient client, IAppCache cache, ILogger<DrupalRepository> logger)
    {
        _logger = logger;
        _cache = cache;
        _client = client;
        _client.Timeout = TimeSpan.FromSeconds(5);
        _client.BaseAddress = new Uri("https://artsdatabanken.no/api/");
    }

    public async Task<ContentRootResponseDto> ContentById(int id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{nameof(DrupalRepository)}-{nameof(ContentById)}-{id}";
        
        HttpStatusCode responseStatusCode = new();
        
        try
        {
            return await _cache.GetOrAddAsync(cacheKey, async () =>
            {
                var response = await _client.GetAsync($"content/{id}", cancellationToken);

                responseStatusCode = response.StatusCode;
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<ContentRootResponseDto>(cancellationToken: cancellationToken);
            });
        }
        catch (Exception ex)
        {
            _cache.Remove(cacheKey);
            _logger.LogError(ex, "Could not get content by id: {id} (StatusCode: {statuscode}, Message: {message})", id, responseStatusCode, ex.Message);
            return null;
        }
    }

    public Task<ContentRootResponseDto> ContentByType(ContentModelType modelType, CancellationToken cancellationToken = default)
    {
        var contentId = DrupalNodeIdMapping.FirstOrDefault(x => x.Key == modelType).Value;

        if (contentId == 0)
            throw new ArgumentOutOfRangeException($"{modelType}");

        return ContentById(contentId, cancellationToken);
    }

    // hardkodede node id'er som benyttes til forskjellig innhold
    private static Dictionary<ContentModelType, int> DrupalNodeIdMapping => new()
    {
        { ContentModelType.HeaderMenu, 341039 }, // "Header meny - (ny grafisk profil 2023)"
        { ContentModelType.FooterMain, 342287 }, // "Footer hovedfelt - (ny grafisk profil 2023)"
        { ContentModelType.FooterSome, 342288 }, // "Footer sosiale media - (ny grafisk profil 2023)"
        { ContentModelType.FooterLinks, 341268 } // "Footer nederste felt - (ny grafisk profil 2023)"
    };
}