using System.ComponentModel.DataAnnotations;

namespace ElecPOE.DTO
{
    public class UserViewModel
    {
        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email ID required!")]
        public string StudentNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me?")]
        public bool RememberMe { get; set; }
        public string? ReturnUrl
        {
            get; set;
        }
    }
}
