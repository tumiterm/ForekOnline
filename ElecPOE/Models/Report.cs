using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.Models
{
    public class Report : Base
    {
        [Key]
        public Guid ReportId { get; set; }
        public string? Reference { get; set; }
        public string? IdPass { get; set; }  

        [Display(Name = "Report Type")]
        public ReportType ReportType { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
        public string? Module { get; set; }

        [Display(Name = "Activity Report")]
        public string? ActivityReport { get; set; }
        public string? Challenges { get; set; }
        public Guid? FacilitatorId { get; set; }
        public string? Recommendation { get; set; }
        public eUrgency? Urgency { get; set; }
        public eOperation Operation { get; set; }
        public string? Document { get; set; }

        [NotMapped]
        public IFormFile? DocumentFile { get; set; }

    }
}
