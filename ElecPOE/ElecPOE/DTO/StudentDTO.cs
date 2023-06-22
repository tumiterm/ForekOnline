using ElecPOE.Enums;
using ElecPOE.Models;
using System.ComponentModel.DataAnnotations;

namespace ElecPOE.DTO
{
    public class StudentDTO : Base
    {
        [Display(Name = "Document Name")]
        public string DocumentName { get; set; }
        public string Student { get; set; }

    }
}
