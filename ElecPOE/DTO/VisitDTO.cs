using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.DTO
{
    public class VisitDTO
    {
        public Guid VisitId { get; set; }   
        public string Company { get; set; }
        public string[] SelectedIDArray { get; set; }
        public string? SelectedEmployeeIDs { get; set; }
        public Guid PlacementId { get; set; }

        [Display(Name = "Attach Report?")]
        public bool HasReport { get; set; }

        public Guid VisitBy { get; set; }
        public string? Report { get; set; }
        public string Date { get; set; }
        public string? LearnerFeedback { get; set; }
        public string? VisitPurpose { get; set; }
        public string? Mentor { get; set; }
        public Guid CompanyId { get; set; }

        [NotMapped]
        public IFormFile? ReportFile { get; set; }
    }
}
