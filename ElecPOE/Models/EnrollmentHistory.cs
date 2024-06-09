using System.ComponentModel.DataAnnotations;

namespace ElecPOE.Models
{
    public class EnrollmentHistory
    {
        public Guid EnrollmentId { get; set; }
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string CourseType { get; set; }
        public string EnrollmentStatus { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; }
    }
}
