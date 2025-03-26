using Assessments.Shared.DTOs.NatureTypes;
using Assessments.Shared.DTOs.NatureTypes.Statistics;
using RodlisteNaturtyper.Core.Models;
using RodlisteNaturtyper.Data.Models;

namespace Assessments.Shared.Interfaces;

public interface INatureTypesRepository
{
    IQueryable<Assessment> GetAssessments();

    Assessment GetAssessment(int id);

    List<CodeItemViewModel> GetAssessmentCodeItemViewModels(int id);

    List<Committee> GetCommittees();

    List<CommitteeUserDto> GetCommitteeUsers();

    List<Region> GetRegions();

    Task<List<CategoryStatisticsResponse>> GetCategoryStatistics(Uri uri, CancellationToken cancellationToken = default);
}