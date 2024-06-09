using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations;

namespace ElecPOE.Models
{
    public class Module
    {
        [Key]
        public Guid ModuleId { get; set; }
        public string? ModuleName { get; set; } 
        public Guid CourseIdFK { get; set; }
        public eNQF? NQFLevel { get; set; }  
        public double? Credit { get; set; }
        public bool IsActive { get; set; }
    }
}
