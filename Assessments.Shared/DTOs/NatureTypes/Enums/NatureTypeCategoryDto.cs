using System.ComponentModel;
// ReSharper disable InconsistentNaming

namespace Assessments.Shared.DTOs.NatureTypes.Enums;

public enum NatureTypeCategoryDto
{
    [Description("Gått tapt")]
    CO = 0,

    [Description("Kritisk truet")]
    CR = 1,

    [Description("Sterkt truet")]
    EN = 2,

    [Description("Sårbar")]
    VU = 3,

    [Description("Nær truet")]
    NT = 4,

    [Description("Datamangel")]
    DD = 5,

    [Description("Uten risiko")]
    LC = 6,

    [Description("Ikke egnet")]
    NA = 7,

    [Description("Ikke vurdert")]
    NE = 8
}