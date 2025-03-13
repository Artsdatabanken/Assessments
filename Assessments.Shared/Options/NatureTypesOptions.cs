using System.ComponentModel.DataAnnotations;

namespace Assessments.Shared.Options;

public class NatureTypesOptions
{
    [Required] 
    public Uri ODataUrl { get; set; }

    [Required] 
    public string ODataApiKey { get; set; }
}