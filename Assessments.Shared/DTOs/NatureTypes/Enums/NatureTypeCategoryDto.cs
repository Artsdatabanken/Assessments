using System.ComponentModel.DataAnnotations;
// ReSharper disable InconsistentNaming

namespace Assessments.Shared.DTOs.NatureTypes.Enums;

public enum NatureTypeCategoryDto
{
    [Display(Name = "gått tapt")]
    CO = 0,

    [Display(Name = "kritisk truet")]
    CR = 1,

    [Display(Name = "sterkt truet")]
    EN = 2,

    [Display(Name = "sårbar")]
    VU = 3,

    [Display(Name = "nær truet")]
    NT = 4,

    [Display(Name = "datamangel")]
    DD = 5,

    [Display(Name = "uten risiko")]
    LC = 6,

    [Display(Name = "ikke vurdert")]
    NE = 8,

    [Display(Name = "Merk alle *sett inn tekst")] // TODO: sett inn tekster for gruppering
    RED = 9,

    [Display(Name = "Marker alle *sett inn tekst")]
    END = 10
}