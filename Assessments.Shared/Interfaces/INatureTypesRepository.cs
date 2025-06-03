using Assessments.Shared.DTOs.NatureTypes;
using Assessments.Shared.DTOs.NatureTypes.Statistics;
using RodlisteNaturtyper.Core.Models;
using RodlisteNaturtyper.Data.Models;

namespace Assessments.Shared.Interfaces;

public interface INatureTypesRepository
{
    IQueryable<Assessment> GetAssessments();

    Assessment GetAssessment(int id);

    List<CodeItemModel> GetAssessmentCodeItemModels(int id);

    List<Committee> GetCommittees();

    List<CommitteeUserDto> GetCommitteeUsers();

    List<Region> GetRegions();

    List<NinCodeTopic> GetNinCodeTopics();

    List<KeyValuePair<string, int>> GetNinCodeTopicSuggestions();
    
    List<CodeItem> GetCodeItems();
    
    Task<List<CategoryStatisticsResponse>> GetCategoryStatistics(Uri uri, CancellationToken cancellationToken = default);
}