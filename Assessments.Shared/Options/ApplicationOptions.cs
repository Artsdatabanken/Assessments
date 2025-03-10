using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Assessments.Shared.Options;

public class ApplicationOptions // configured in appsettings.*.json
{
    [Required] 
    public Uri BaseUrl { get; set; }

    [Required]
    public string FeedbackSecret { get; set; }

    [Required]
    public string SendGridApiKey { get; set; }

    [Required]
    [ValidateObjectMembers]
    public AlienSpecies2023Options AlienSpecies2023 { get; set; }

    [Required]
    [ValidateObjectMembers]
    public Species2021Options Species2021 { get; set; }

    [Required]
    [ValidateObjectMembers]
    public NatureTypesOptions NatureTypes { get; set; }
}

public class AlienSpecies2023Options
{
    [Required] 
    public bool TransformAssessments { get; set; }
}

public class Species2021Options
{
    [Required] 
    public bool TransformAssessments { get; set; }
}

public class NatureTypesOptions
{
    [Required] 
    public Uri ODataUrl { get; set; }

    [Required] 
    public string ODataApiKey { get; set; }
}