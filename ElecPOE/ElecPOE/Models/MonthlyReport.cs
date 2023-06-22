using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.Models
{
    public class MonthlyReport : Base
    {
        [Key]
        public Guid MonthlyReportId { get; set; }
        public string MonthlyReportName { get; set; }
        public string? ReferenceNumber { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
        [NotMapped]
        public IFormFile? DocumentFile { get; set; }
        public bool IsHOD { get; set; }
        public string? Recommendation { get; set; }


    }
}
