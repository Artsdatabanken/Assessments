using System.ComponentModel.DataAnnotations;

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
    public Uri NinKodeApiUrl { get; set; }

    // TODO: må få nytt navn i keyvault før tas i bruk (etter lansering rln)
    // gammelt navn: "ApplicationOptions--NatureTypes--TemporaryAccessKey", flyttes til "ApplicationOptions--TemporaryAccessKey"?
    public string TemporaryAccessKey { get; set; }
}