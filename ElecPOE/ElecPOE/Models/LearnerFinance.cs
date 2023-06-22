using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.Models
{
    public class LearnerFinance : Base
    {
        [Key]
        public Guid Id { get; set; }
        public Guid StudentId { get; set; } 
        public string StudentNumber { get; set; }
        public string? File { get; set; }

        [Display(Name = "Statement Name")]
        public string? StatementName { get; set; }   

        [NotMapped]
        public IFormFile FileAttach { get; set; }

        


    }
}
