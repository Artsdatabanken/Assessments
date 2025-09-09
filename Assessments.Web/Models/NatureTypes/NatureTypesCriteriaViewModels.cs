using Assessments.Shared.DTOs.NatureTypes.Enums;
using RodlisteNaturtyper.Data.Models.Enums;

namespace Assessments.Web.Models.NatureTypes;

public record CriteriaCategoryViewModel(string Description, CriteriaCategory? Category, CriteriaCategoryType Type, bool IsActive);

public record CriteriaResultViewModel(string Description, CriteriaCategory Category, bool IsActive);

public record CriteriaCategoryChangeViewModel(string Description, CriteriaCategoryChange Category, bool IsActive);

public record CriteriaCategoryImpactViewModel(string Description, CriteriaCategoryImpact Category, bool IsActive);

public record CriteriaCategoryThreatDefinedlocationViewModel(string Description, CriteriaCategoryThreatDefinedlocation Category, bool IsActive);

public record CriteriaMatrixViewModel(string Description, CategoryCriteriaType CriteriaType, CriteriaCategory CriteriaArea, CriteriaCategory Criteria, bool IsActive);