using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.Models
{
    public class Visit : Base
    {
        [Key]
        public Guid VisitId { get; set; }   
        public Guid CompanyId { get; set; }

        [Display(Name = "Attach Report?")]
        public bool HasReport { get; set; }
        public string? Report { get; set; }
        public DateTime Date { get; set; }  
        public string? LearnerFeedback { get; set; }
        public string? VisitPurpose { get; set; }
        public string? Mentor { get; set; }

        [NotMapped]
        public IFormFile? ReportFile { get; set; }

    }

    
}
