using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.DTO
{
    public class LessonPlanDTO
    {
        public Guid LessonPlanId { get; set; }
        public string? Reference { get; set; }
        public string IdPass { get; set; }
        public bool IsActive { get; set; }
        public Guid Course { get; set; }
        public Guid Module { get; set; }
        public ePhase? Phase { get; set; }
        public eFunder Funder { get; set; }

        [Display(Name ="IsApproved")]
        public eSelection Approval { get; set; }
        public string? IsApprovedBy { get; set; }
        public string? Reason { get; set; }
        public string? Document { get; set; }
        [NotMapped]
        public IFormFile DocumentFile { get; set; }

    }
}
