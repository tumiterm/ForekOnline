using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.Models
{
    public class Training : Base
    {
        [Key]
        public Guid TrainingId { get; set; }
        public string? StudentNumber { get; set; }
        public Guid? StudentId { get; set; }
        public string? Document { get; set; }

        [Display(Name = "Document Type")]
        public eTrainingAdministration Type { get; set; }

        [NotMapped]
        public IFormFile DocumentFile { get; set; }

    }
}
