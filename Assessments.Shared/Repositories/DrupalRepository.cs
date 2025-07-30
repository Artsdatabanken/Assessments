using Assessments.Shared.DTOs.Drupal;
using Assessments.Shared.DTOs.Drupal.Enums;
using Assessments.Shared.Helpers;
using Assessments.Shared.Interfaces;
using LazyCache;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;

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

    public async Task<List<ContentByLongCodeResponseDto>> ContentByLongCode(string longCode, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{nameof(DrupalRepository)}-{nameof(ContentByLongCode)}-{longCode}";

        var model = await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            var response = await _client.GetAsync($"nin/v3?code={longCode}", cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            var responseDtos = await response.Content.ReadFromJsonAsync<List<ContentByLongCodeResponseDto>>(cancellationToken: cancellationToken);

            return responseDtos.Count != 0 ? responseDtos : null;
        });

        return model;
    }

    public async Task<List<ImageModelDto>> ImageModelsByLongCode(string longCode)
    {
        var contentByLongCode = await ContentByLongCode(longCode);

        var dtos = contentByLongCode?.Where(x => x.Type.Equals("image")).ToList();

        var models = new List<ImageModelDto>();

        if (dtos == null)
            return models;

        foreach (var dto in dtos.Take(10))
        {
            var imageModelDto = new ImageModelDto
            {
                LongCode = longCode,
                Url = new Uri(dto.Id.Replace("Nodes/", "https://artsdatabanken.no/Media/")),
                Link = new Uri(dto.Id.Replace("Nodes/", "https://artsdatabanken.no/Pages/")),
                Text = dto.Fields.FirstOrDefault(x => x.Name.Equals("annotation"))?.Values.FirstOrDefault().StripHtml()
            };

            var authorReferences = dto.Fields.FirstOrDefault(x => x.Name.Equals("metadata"))?.Fields.FirstOrDefault(x => x.Name.Equals("reference"))?.References;

            if (authorReferences != null)
                imageModelDto.Authors = await GetAuthors(authorReferences);

            var licenseReference = dto.Fields.FirstOrDefault(x => x.Name.Equals("license"))?.References.FirstOrDefault(x => x.StartsWith("Nodes/T"));

            if (licenseReference != null)
                imageModelDto.License = licenseReference.Split("/").Last().ToEnum(ImageLicenseEnum.Unknown);

            models.Add(imageModelDto);
        }

        return models;
    }

    private async Task<List<string>> GetAuthors(List<string> references)
    {
        var authors = new List<string>();

        foreach (var author in references)
        {
            if (!int.TryParse(author.Split("/").Last(), out var nodeId))
                continue;

            var contentById = await ContentById(nodeId);
            authors.Add(contentById.Title);
        }

        return authors;
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