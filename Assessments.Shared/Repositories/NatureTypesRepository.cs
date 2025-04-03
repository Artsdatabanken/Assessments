using System.Net.Http.Json;
using System.Web;
using Assessments.Shared.DTOs.NatureTypes;
using Assessments.Shared.DTOs.NatureTypes.Statistics;
using Assessments.Shared.Interfaces;
using Assessments.Shared.Options;
using Default;
using LazyCache;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OData.Client;
using RodlisteNaturtyper.Core.Models;
using RodlisteNaturtyper.Data.Models;

namespace Assessments.Shared.Repositories;

public class NatureTypesRepository : INatureTypesRepository
{
    private readonly RodlisteNaturtyperContainer _container;
    private readonly IAppCache _appCache;
    private readonly HttpClient _client;
    private readonly ILogger<NatureTypesRepository> _logger;
    private readonly IOptions<ApplicationOptions> _options;

    public NatureTypesRepository(IOptions<ApplicationOptions> options, IAppCache appCache, ILogger<NatureTypesRepository> logger, HttpClient client)
    {
        _options = options;
        _logger = logger;
        _appCache = appCache;
        _client = client;

        _client.Timeout = TimeSpan.FromSeconds(10);
        _client.DefaultRequestHeaders.Add("X-API-KEY", options.Value.NatureTypes.ODataApiKey);
        
        _container = new RodlisteNaturtyperContainer(options.Value.NatureTypes.ODataUrl)
        {
            HttpClientFactory = new HttpClientFactory(_client)
        };
    }

    public IQueryable<Assessment> GetAssessments() => _container.Assessments.Expand(x => x.Committee);
    
    public Assessment GetAssessment(int id)
    {
        return _container.Assessments
            .Expand(x => x.Committee)
            .Expand(x => x.Regions)
            .Expand(x => x.References)
            .FirstOrDefault(c => c.Id == id);
    }

    public List<CodeItemViewModel> GetAssessmentCodeItemViewModels(int id)
    {
        DataServiceQuerySingle<Assessment> assessment = _container.Assessments.ByKey(id);
        
        return [.. assessment.CodeItemViewModels()];
    }

    public List<Committee> GetCommittees()
    {
        return _appCache.GetOrAdd($"{nameof(NatureTypesRepository)}-{nameof(GetCommittees)}", () => _container.Committees.OrderBy(x => x.Name).ToList());
    }

    public List<CommitteeUserDto> GetCommitteeUsers()
    {
        return _appCache.GetOrAdd($"{nameof(NatureTypesRepository)}-{nameof(GetCommitteeUsers)}", () => _container.CommitteeUsers.OrderBy(x => x.Committee.Name).Select(x => new CommitteeUserDto
        {
            CommitteeId = (int)x.CommitteeId,
            CommitteeName = x.Committee.Name,
            Level = x.Level,
            UserLastName = x.User.LastName,
            UserFirstName = x.User.FirstName,
            UserCitationName = x.User.CitationName
        }).ToList());
    }

    public List<Region> GetRegions()
    {
        return _appCache.GetOrAdd($"{nameof(NatureTypesRepository)}-{nameof(GetRegions)}", () => _container.Regions.OrderBy(x => x.SortOrder).ToList());
    }

    public async Task<List<CategoryStatisticsResponse>> GetCategoryStatistics(Uri uri, CancellationToken cancellationToken = default)
    {
        var queryStrings = HttpUtility.ParseQueryString(new UriBuilder(uri).Query);
        var filter = queryStrings["$filter"];
        var queryStringValue = "groupby((category), aggregate($count as count))";

        if (!string.IsNullOrEmpty(filter))
        {
            queryStringValue = $"filter({filter})/{queryStringValue}";
        }

        var builder = new QueryBuilder { { "apply", queryStringValue } };

        var response = await _client.GetAsync($"{_options.Value.NatureTypes.ODataUrl}Assessments{builder.ToQueryString()}", cancellationToken);

        try
        {
            response.EnsureSuccessStatusCode();
            
            var rootResponse = await response.Content.ReadFromJsonAsync<CategoryStatisticsRootResponse>(cancellationToken);

            return rootResponse.Value;
        }
        catch (Exception ex)
        { 
            _logger.LogError("{method} failed: {message} (StatusCode: {statuscode} Path: '{path}')", nameof(GetCategoryStatistics), ex.Message, response.StatusCode, uri);

            return null;
        }
    }

    private class HttpClientFactory(HttpClient httpClient) : IHttpClientFactory
    {
        HttpClient IHttpClientFactory.CreateClient(string name) => httpClient;
    }
}