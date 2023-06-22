using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.Models
{
    public class Placement : Base
    {
        public Guid PlacementId { get; set; }
        public Guid CompanyId { get; set; }
        public string Student { get; set; }
        public Guid PlacedBy { get; set; }
        public eStatus Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [ForeignKey("Module")]
        public Guid? Module { get; set; }

        [ForeignKey("Course")]
        public Guid? CourseId { get; set; }
        public int? Duration { get; set; }

        [NotMapped]
        public Guid? StudentId { get; set; }

        [NotMapped]
        [Display(Name = "Send SMS?")]
        public bool SendNotification { get; set; }  

    }
}
