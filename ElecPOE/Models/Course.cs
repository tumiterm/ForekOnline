using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations;

namespace ElecPOE.Models
{
    public class Course : Base
    {
        [Key]
        public Guid CourseId { get; set; }

        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [Display(Name = "Course Type")]
        public eCourseType Type { get; set; }
        public eNType? NType {get; set;}
        public double? Credit { get; set; }
        public eNQF? NQFLevel { get; set; }  
        public List<Module>? Module { get; set;}
    }
}
  