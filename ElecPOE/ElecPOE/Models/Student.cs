
namespace ElecPOE.Models
{
    public class Student
    {
        public Guid StudentId { get; set; }
        public string StudentNumber { get; set; }
        public DateTime AdmissionDate { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string IDNumber { get; set; }
        public string StudyPermitNumber { get; set; }
        public string PassportNumber { get; set; }
        public DateTime DateofBirth { get; set; }
        public string Gender { get; set; }
        public string PlaceofBirth { get; set; }
        public string Nationality { get; set; }
        public string Language { get; set; }
        public object AdmissionCategory { get; set; }
        public string StreetAddressLine1 { get; set; }
        public string StreetAddressLine2 { get; set; }
        public string Cellphone { get; set; }
        public string Email { get; set; }
        public string HighestGrade { get; set; }
        public string NameofSchool { get; set; }
        public bool IsActive { get; set; }
        public bool Deregistered { get; set; }
        public List<EnrollmentHistory>? EnrollmentHistory { get; set; }
    }
}



