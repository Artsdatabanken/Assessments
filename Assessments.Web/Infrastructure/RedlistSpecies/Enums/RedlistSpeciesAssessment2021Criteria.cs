using System.ComponentModel.DataAnnotations;

namespace Assessments.Web.Infrastructure.RedlistSpecies.Enums
{
    public enum RedlistSpeciesAssessment2021Criteria
    {
        [Display(Name = "populasjonsreduksjon")]
        A,

        [Display(Name = "lite areal")]
        B,

        [Display(Name = "liten populasjon")]
        C,

        [Display(Name = "svært liten populasjon eller forekomst")]
        D,
    }
}
