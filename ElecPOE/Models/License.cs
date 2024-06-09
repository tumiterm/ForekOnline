using ElecPOE.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.Models
{
    public class License : Base
    {
        [Key]
        public Guid LicenseId { get; set; }
        public eTitle Title { get; set; }

        #region Course Foreign Key
        public Guid CourseKey { get; set; }

        #endregion

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Full Name")]
        [MinLength(3)]

        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        [MinLength(3)]

        public string LastName { get; set; }

        [Display(Name = "ID/Passport Number")]
        public string? IDNumber { get; set; }

        [Required(ErrorMessage = "Date of Issue is required")]
        public DateTime DateOfIssue { get; set; }

        [Required(ErrorMessage = "Date of Expiry is required")]
        public DateTime DateOfExpiry { get; set; }
        public string? FileUpload { get; set; }
        public eClientType ClientType { get; set; }
        public Guid CompanyId { get; set; }
        public eLicenseFrequency Frequency { get; set; }

        [NotMapped]
        public IFormFile? IFormFileUpload { get; set; }





    }
}
