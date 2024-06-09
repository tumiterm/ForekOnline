using ElecPOE.Enums;
using ElecPOE.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.DTO
{
    public class ApplyDTO
    {
        public Guid ApplicantId { get; set; }
        public string? ReferenceNumber { get; set; }
        public eChoice Selection { get; set; }
        public string? PassportNumber { get; set; }
        public eCategory StudyPermitCategory { get; set; }
        public string? IDNumber { get; set; } 
        public string? Email { get; set; }
        public string Cellphone { get; set; }
        public Guid AddressId { get; set; }
        public string? StreetName { get; set; }
        public string? Line1 { get; set; }
        public string? City { get; set; }
        public eProvince? Province { get; set; }
        public string? PostalCode { get; set; }
        public Guid AddressAssociativeId { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantSurname { get; set; }
        public eFunderType FunderType { get; set; }
        public string? StatusReason { get; set; }
        public eTitle ApplicantTitle { get; set; }
        public eGender Gender { get; set; }
        public HighestQualification HighestQualification { get; set; }
        public string? IDPassDoc { get; set; }
        public string? HighestQualDoc { get; set; }
        public string? ResidenceDoc { get; set; }
        public ApplicationStatus Status { get; set; }
        public Guid GuardianId { get; set; }
        public Guid GuardianApplicationId { get; set; }
        public string GuardianFirstName { get; set; }
        public string GuardianLastName { get; set; }
        public eRelationship GuardianRelationship { get; set; }
        public string GuardianCellphone { get; set; }
        public string? GuardianIDDoc { get; set; }
        public string? MedicalName { get; set; }
        public string? MemberNumber { get; set; }
        public string? Telephone { get; set; }
        public string? Disability { get; set; }

        [NotMapped]
        public IFormFile? GuardianIDFile { get; set; }
        public Guid CourseId { get; set; }

        [NotMapped]
        public IFormFile? ApplicantIDPassFile { get; set; }

        [NotMapped]
        public IFormFile? AplicantHighestQualFile { get; set; }
        [NotMapped]
        public IFormFile? ApplicantResidenceFile { get; set; }
    }
}
