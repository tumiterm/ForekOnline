using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.Models
{
    public class Application
    {
        [Key]
        public Guid ApplicationId { get; set; } 
        public string? ReferenceNumber { get; set; }
        public eChoice Selection { get; set; }  
        public string? PassportNumber { get; set; }
        public eCategory StudyPermitCategory { get; set; }   
        public string? IDNumber { get; set; }
        public string? Email { get; set; }
        public string Cellphone { get; set; }
        public Address ApplicantAddress { get; set; }  
        public string ApplicantName { get; set;}
        public string ApplicantSurname { get; set;}
        public eTitle ApplicantTitle { get; set; }
        public eGender Gender { get; set; }
        public Guardian ApplicantGuardian { get; set; }
        public HighestQualification HighestQualification { get; set; }
        public eFunderType FunderType { get; set; }
        public string? StatusReason { get; set; }
        public string? IDPassDoc { get; set; }
        public string? HighestQualDoc { get; set; }
        public string? ResidenceDoc { get; set; }
        public ApplicationStatus Status { get; set; }

        [ForeignKey(nameof(Course))]   
        public Guid CourseId { get; set; }

        [NotMapped]
        public IFormFile? IDPassFile { get; set; }

        [NotMapped]
        public IFormFile? HighestQualFile { get; set; }
        [NotMapped]
        public IFormFile? ResidenceFile { get;set; }
    }
}
