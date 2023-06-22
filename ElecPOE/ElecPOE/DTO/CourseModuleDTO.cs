using ElecPOE.Enums;
using ElecPOE.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ElecPOE.DTO
{
    public class CourseModuleDTO : Base
    {
        [Key]
        public Guid ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public eNQF? NQFLevel { get; set; }
        public eNType? NType { get; set; }
        public eNQF? CourseNQFLevel { get; set; }
        public double? Credit { get; set; }
        public Guid CourseId { get; set; }
        public double? CourseCredit { get; set; }
        public string Name { get; set; }
        public eCourseType Type { get; set; }
        public Guid CourseTypeFK { get; set; }
        public List<Models.Module>? Module { get; set; }
        
           
    }
}
