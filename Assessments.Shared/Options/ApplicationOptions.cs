using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Assessments.Shared.Options;

public class ApplicationOptions // configured in appsettings.*.json
{
    [Required] 
    public Uri BaseUrl { get; set; }

    public bool TransformAssessments { get; set; }

    [Required]
    public string FeedbackSecret { get; set; }

    [Required]
    public string SendGridApiKey { get; set; }

    [Required]
    public string ApiKey { get; set; }

    [Required]
    [ValidateObjectMembers]
    public NatureTypesOptions NatureTypes { get; set; }

    [Required]
    public Uri NinKodeApiUrl { get; set; }
}