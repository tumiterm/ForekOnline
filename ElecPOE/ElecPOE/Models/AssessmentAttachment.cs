 using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.Models
{
    public class AssessmentAttachment : Base
    {
        [Key]
        public Guid AttachmentId { get; set; }
        public string? Document { get; set; }

        [Display(Name = "StudentId Number")]
        public string? StudentNumber { get; set; }
        public Guid? StudentId { get; set; }
        public Guid? CourseId { get; set; }  
        public eModule? Module { get; set; }
        public double? Percentage { get; set; }
        public eAttempts Attempts { get; set; } 
		public eAssessmentAdministration Type { get; set; }

        [NotMapped]
        [Display(Name = "File")]
        public IFormFile AttachmentFile { get; set; }
    }
}
