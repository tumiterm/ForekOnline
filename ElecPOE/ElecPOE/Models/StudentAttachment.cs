using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.Models
{
    public class StudentAttachment : Base
    {
        [Key]
        public Guid AttachmentId { get; set; }

        [Display(Name = "Document Name")]
        public eLearnerAdministration DocumentName { get; set; }
        public string? Document { get; set; }

        [Display(Name = "StudentId Number")]
        public string? StudentNumber { get; set; }
        public Guid? StudentId { get; set; }

        [NotMapped]
        [Display(Name = "File")]
        public IFormFile AttachmentFile { get; set; }

    }
}
