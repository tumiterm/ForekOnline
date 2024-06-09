using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.DTO
{
    public class WorkplaceModulesDTO
    {
        public Guid LearnerWorkplaceModulesId { get; set; }
        public string Course { get; set; }
        public Guid CourseId { get; set; }
        public Guid ModuleId { get; set; }
        public string Module { get; set; }
        public string? Student { get; set; }
        public int? Days { get; set; }
        public eStatus? Progress { get; set; }
        public string? RegisteredBy { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? ModifiedBy { get; set; }
        public Guid PlacementId { get; set; }
        public string? ModifiedOn { get; set; }
        public bool IsActive { get; set; }  
        public string? Company { get; set; } 
    }
}
