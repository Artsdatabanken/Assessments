using System.ComponentModel.DataAnnotations;

namespace Assessments.Web.Infrastructure.Enums
{
    public enum AssessmentType
    {
        [Display(Name = "Fremmedartslista 2023")]
        AlienSpecies2023,

        [Display(Name = "Rødlista for arter 2021")]
        RedlistSpecies2021
    }

    public enum ListOrAssessmentView
    {
        Index,
        Assessment
    }
}
