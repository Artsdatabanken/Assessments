using Assessments.Web.Infrastructure.Enums;
using X.PagedList;

namespace Assessments.Web.Models;

public class ListViewViewModel
{
    public AssessmentType AssessmentType { get; set; }

    public string NoResultString { get; set; }

    public IPagedList<Result> Results { get; set; }

    public string View { get; set; }

    public class Result
    {
        public bool IsDoorKnocker { get; set; }

        public string EvaluationContext { get; set; }

        public bool HasEffectWithoutReproduction { get; set; }

        public string Category { get; set; }

        public string CategoryShort { get; set; }

        public string Degrees { get; set; }

        public int Id { get; set; }

        public string ScientificNameFormatted { get; set; }

        public string SpeciesGroup { get; set; }

        public string SpeciesGroupIconUrl { get; set; }

        public string VernacularName { get; set; }
    }
}