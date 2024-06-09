using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.Models
{
    public class NatedEngineering : Base
    {
        [Key]
        public Guid NatedEngineeringId { get; set; }

        [ForeignKey("Course")]
        public Guid CourseId { get; set; }

        [ForeignKey("Module")]
        public Guid ModuleId { get; set; }  
        public eFileSelection FileSelection { get; set; }

        [Display(Name = "File Name")]
        public string? FileName { get; set; }

        [Display(Name = "Icass Task 1")]
        public string? Icass1 { get; set; }

        [Display(Name = "Icass Task 2")]
        public string? Icass2 { get; set; }

        [Display(Name = "Assessment File")]
        public string? AssessmentFile { get; set; }


        [NotMapped]
        public IFormFile FileUpload { get; set; }
        [NotMapped]
        public IFormFile Icass1FileUpload { get;set; }
        [NotMapped]
        public IFormFile Icass2FileUpload { get; set; }

        [NotMapped]
        public IFormFile AssessmentFileUpload { get; set; }


    }
}
