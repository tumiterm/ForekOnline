using ElecPOE.Enums;
using ElecPOE.Models;
using System.ComponentModel.DataAnnotations;

namespace ElecPOE.DTO
{
    public class CourseDTO : Base
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public string? Type { get; set; }
        public string? NType { get; set; }
        public double? Credit { get; set; }
        public string? NQFLevel { get; set; }
    }
}
