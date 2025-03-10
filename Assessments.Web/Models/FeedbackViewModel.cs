using Assessments.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Assessments.Web.Models;

public class FeedbackViewModel
{
    [Required(ErrorMessage = "{0} er påkrevd")]
    [MinLength(5, ErrorMessage = "{0} må ha minst {1} tegn")]
    [MaxLength(100, ErrorMessage = "{0} kan ha maks {1} tegn")]
    [Display(Name = "Navn")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "{0} er påkrevd")]
    [MaxLength(200, ErrorMessage = "{0} kan ha maks {1} tegn")]
    [EmailAddress(ErrorMessage = "{0} må være en gyldig e-postadresse")]
    [Display(Name = "E-postadresse")]
    public string Email { get; set; }

    [Required(ErrorMessage = "{0} er påkrevd")]
    [MinLength(5, ErrorMessage = "{0} må ha minst {1} tegn")]
    [MaxLength(10000, ErrorMessage = "{0} kan ha maks {1} tegn")]
    [Display(Name = "Kommentar")]
    public string Comment { get; set; }

    [Display(Name = "Godta vilkår")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "Du må godkjenne vilkårene")]
    public bool Terms { get; set; }

    [Display(Name = "Filer")]
    public List<IFormFile> FormFiles { get; set; }

    public int AssessmentId { get; set; }

    public string AssessmentName { get; set; }

    public int Year { get; set; }

    public string ExpertGroup { get; set; }

    public FeedbackType Type { get; set; }

    [Required(ErrorMessage = "{0} er påkrevd")] 
    public bool HasBackToTopLink { get; set; }
}