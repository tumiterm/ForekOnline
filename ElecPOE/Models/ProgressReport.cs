using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.Models
{
    public class ProgressReport : Base
    {
        [Key]
        public Guid ReportId { get; set; }  
        public Guid StudentId { get; set; }
        public string StudentNumber { get; set; }
        public eTrade Course { get; set; }
     
        [Display(Name = "Report Name")]
        public string ReportName { get; set; }
        public string? AttachReport { get; set; }

        [NotMapped]
        public IFormFile ReportFile { get; set; }   

        

    }
}
