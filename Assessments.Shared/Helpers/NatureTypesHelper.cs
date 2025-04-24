using Assessments.Shared.DTOs.NatureTypes.Enums;

namespace Assessments.Shared.Helpers;

public static class NatureTypesHelper
{
    public static List<CategoryCriteriaType> GetCategoryCriteriaTypes(string categoryCriteria)
    {
        if (string.IsNullOrEmpty(categoryCriteria))
            return [];

        // utslagsgivende kriterier fra "Endelig kategori og kriterium"
        return [.. Array.ConvertAll(categoryCriteria[2..].Split('+'), x => x.Trim()[..1]).Distinct().ToEnumerable<CategoryCriteriaType>()];
    }
}