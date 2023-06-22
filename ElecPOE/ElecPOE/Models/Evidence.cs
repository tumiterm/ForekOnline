using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.Models
{
    public class Evidence : Base
    {
        [Key]
        public Guid EvidenceId { get; set; }
        public eModule Module { get; set; }
        public string? StudentNumber { get; set; }
        public Guid? StudentId { get; set; }
        public string? Photo { get; set; }

        [NotMapped]
        public IFormFile PhotoFile { get; set; }


    }
}
