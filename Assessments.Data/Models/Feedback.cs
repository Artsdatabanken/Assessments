using System.ComponentModel.DataAnnotations;
using ClosedXML.Attributes;

namespace Assessments.Data.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;

        public int AssessmentId { get; set; }

        [MaxLength(250)]
        public string AssessmentName { get; set; }

        public int Year { get; set; }

        public FeedbackType Type { get; set; }
            
        [Required, MaxLength(100)]
        public string ExpertGroup { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [Required, MaxLength(200)]
        public string Email { get; set; }

        [Required, MaxLength(10000)]
        public string Comment { get; set; }

        [XLColumn(Ignore = true)]
        public List<FeedbackAttachment> Attachments { get; set; } = new();
    }

    public enum FeedbackType
    {
        AlienSpecies = 0,
        NatureTypes = 1
    }
}
