using System.ComponentModel.DataAnnotations;

namespace ElecPOE.DTO
{
    public class EnrollmentHistoryViewModel
    {
        public Guid EnrollmentId { get; set; }
        public Guid StudentId { get; set; }
        public Guid? CourseId { get; set; }

        [Display(Name = "Course")]
        public string CourseTitle { get; set; }

        [Display(Name = "Course Type")]
        public string CourseType { get; set; }

        [Display(Name = "Enrollement Status")]
        public string EnrollmentStatus { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        public bool IsActive { get; set; }
    }
}
