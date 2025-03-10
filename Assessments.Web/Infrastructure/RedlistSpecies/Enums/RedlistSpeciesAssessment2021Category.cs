using System.ComponentModel.DataAnnotations;

namespace Assessments.Web.Infrastructure.RedlistSpecies.Enums
{
    public enum RedlistSpeciesAssessment2021Category
    {
        // ReSharper disable InconsistentNaming
        [Display(Name = "regionalt utdødd")]
        RE,

        [Display(Name = "kritisk truet")]
        CR,

        [Display(Name = "sterkt truet")]
        EN,

        [Display(Name = "sårbar")]
        VU,

        [Display(Name = "nær truet")]
        NT,

        [Display(Name = "datamangel")]
        DD,

        [Display(Name = "livskraftig")]
        LC,

        [Display(Name = "ikke egnet")]
        NA,

        [Display(Name = "ikke vurdert")]
        NE,

        [Display(Name = "Merk alle rødlistearter")]
        RED,

        [Display(Name = "Marker alle truede arter")]
        END,
    }
}
