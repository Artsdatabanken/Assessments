using Assessments.Shared.DTOs.NatureTypes;
using RodlisteNaturtyper.Data.Models;

namespace Assessments.Shared.Interfaces;

public interface INatureTypesRepository
{
    IQueryable<Assessment> GetAssessments();

    Task<Assessment> GetAssessment(int id, CancellationToken cancellationToken = default);

    Task<List<CodeItemNodeDto>> GetAssessmentCodeItemNodes(int assessmentId, CancellationToken cancellationToken = default);

    Task<List<CommitteeUser>> GetCommitteeUsers(CancellationToken cancellationToken = default);

    Task<List<Region>> GetRegions(CancellationToken cancellationToken = default);

    Task<List<NinCodeTopic>> GetNinCodeTopics(CancellationToken cancellationToken = default);

    Task<List<KeyValuePair<string, int>>> GetNinCodeTopicSuggestions(CancellationToken cancellationToken = default);
    
    Task<List<CodeItem>> GetCodeItems(CancellationToken cancellationToken = default);
    
    Task<List<CodeItem>> GetMainCodeItems(CancellationToken cancellationToken = default);
}