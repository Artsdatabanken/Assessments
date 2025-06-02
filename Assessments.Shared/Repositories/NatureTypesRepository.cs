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
            .Expand(x => x.AreaInformation)
            .Expand(x => x.CriteriaInformation)
            .Expand(x => x.Regions)
            .Expand(x => x.References)
            .Expand(x => x.NinCodeTopic)
            .FirstOrDefault(c => c.Id == id);
    }

    public List<CodeItemModel> GetAssessmentCodeItemModels(int id)
    {
        DataServiceQuerySingle<Assessment> assessment = _container.Assessments.ByKey(id);
        
        return [.. assessment.CodeItemModels()];
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

    public List<NinCodeTopic> GetNinCodeTopics()
    {
        return _appCache.GetOrAdd($"{nameof(NatureTypesRepository)}-{nameof(GetNinCodeTopics)}", () => _container.NinCodeTopics.OrderBy(x => x.Name).ToList());
    }

    public List<KeyValuePair<string, int>> GetNinCodeTopicSuggestions()
    {
        return _appCache.GetOrAdd($"{nameof(NatureTypesRepository)}-{nameof(GetNinCodeTopicSuggestions)}", () =>
        {
            var topics = GetNinCodeTopics();

            List<KeyValuePair<string, int>> items = [];

            items.AddRange(topics.Select(ninCodeTopic => new KeyValuePair<string, int>(new string($"{ninCodeTopic.Name} (Tema: {ninCodeTopic.Description})"), ninCodeTopic.Id)));
            
            return items;
        });
    }

    public List<CodeItem> GetCodeItems()
    {
        return _appCache.GetOrAdd($"{nameof(NatureTypesRepository)}-{nameof(GetCodeItems)}", () => _container.CodeItems.ToList());
    }

    public List<KeyValuePair<string, int>> GetCodeItemSuggestions()
    {
        return _appCache.GetOrAdd($"{nameof(NatureTypesRepository)}-{nameof(GetCodeItemSuggestions)}", () =>
        {
            var codeItems = GetCodeItems();

            List<KeyValuePair<string, int>> items = [];

            foreach (var element in codeItems.Where(x => x.ParentId != 0))
            {
                var codeItem = element;
                var description = codeItem.Description;
                var parentDescriptions = new List<string>();

                while (codeItem != null && codeItem.ParentId != 0)
                {
                    var parent = codeItems.FirstOrDefault(a => a.Id == codeItem.ParentId);
                    if (parent != null && !string.IsNullOrEmpty(parent.Description))
                        parentDescriptions.Add(parent.Description);

                    codeItem = parent;
                }

                items.Add(new KeyValuePair<string, int>($"{description} (Påvirkningsfaktor: {string.Join(" > ", parentDescriptions)})", element.Id));
            }

            return items;
        });
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