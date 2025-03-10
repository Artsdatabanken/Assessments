using System.ComponentModel.DataAnnotations;

namespace Assessments.Web.Infrastructure.RedlistSpecies.Enums
{
    public enum RedlistSpeciesAssessment2021EuropeanPopulation
    {
        [Display(Name = "< 5 %")]
        Lt5,

        [Display(Name = "5 - 25 %")]
        Fr5To25,

        [Display(Name = "25 - 50 %")]
        Fr25To50,

        [Display(Name = "> 50 %")]
        Gt50,
    }
}
