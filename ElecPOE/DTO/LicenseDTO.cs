using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ElecPOE.DTO
{
    public record LicenseDTO
    {
        #region Course Foreign Key
        public Guid CourseKey { get; set; }

        #endregion
        public string? LicenseName { get; set; }
        public Guid LicenseId { get; set; }
        public eTitle Title { get; set; }
        public string Name { get; set; }
        public Guid CompanyId { get; set; }

        public string LastName { get; set; }
        public string? IDNumber { get; set; }
        public DateTime DateOfIssue { get; set; }
        public DateTime DateOfExpiry { get; set; }
        public string? FileUpload { get; set; }
        public eClientType ClientType { get; set; }
        public eLicenseFrequency Frequency { get; set; }

        public string? Company { get; set; }

        [NotMapped]
        public IFormFile? IFormFileUpload { get; set; }

    }
}
