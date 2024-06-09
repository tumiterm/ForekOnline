using ElecPOE.Enums;
using ElecPOE.Models;
using System.ComponentModel.DataAnnotations;

namespace ElecPOE.DTO
{
    public class TrainingDTO : Base
    {
        public string Student { get; set; }

        [Display(Name = "Document Type")]
        public string Type
        {
            get; set;
        }
    }
}
