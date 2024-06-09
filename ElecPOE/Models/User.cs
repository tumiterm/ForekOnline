
using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations;

namespace ElecPOE.Models
{
    public partial class User : Base
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "Full Name")]
        [MaxLength(25)]
        [MinLength(3)]
        public string Name { get; set; }

        [Display(Name = "Last Name")]
        [MaxLength(25)]
        [MinLength(3)]
        public string LastName { get; set; }

        [DataType(DataType.EmailAddress)]

        [Display(Name = "Email Address")]
        public string? Username { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsEmailVerified { get; set; }
        public eDepartment? Department { get; set; }
        public Guid? ActivationCode { get; set; }
        public string? ResetPasswordCode { get; set; }

        [Display(Name = "Last Login Date")]
        public string? LastLoginDate { get; set; }

        [Display(Name= "StudentId Number")]
        public string? StudentNumber { get; set; }

        [MaxLength(10)]
        [MinLength(10)]  
        public string? Cellphone { get; set; }

        [MaxLength(13)]
        [MinLength(8)]  
        [Display(Name = "ID Number / Passport")]
        public string? IDPass { get; set; }

        [Display(Name = "Register As")]
        public eSysRole? Role { get; set; } 
    }

}
