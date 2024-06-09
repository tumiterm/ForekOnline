using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.Models
{
    public class LearnerWorkplaceModules : Base
    {
        [Key]
        public Guid LearnerWorkplaceModulesId { get; set; }

        [ForeignKey("Course")]
        public Guid CourseId { get; set; }

        [ForeignKey("Module")]
        public Guid ModuleId { get; set; }
        public string? Student { get; set; }

        [ForeignKey("Placement")]
        public Guid PlacementId { get; set; }
        public int? Days { get; set; }
        public eStatus? Progress { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set;}
    }
}
