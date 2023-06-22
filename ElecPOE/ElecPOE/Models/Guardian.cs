using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.Models
{
    public class Guardian
    {
        [Key]
        public Guid GuardianId { get; set; }

        [ForeignKey("Application")]
        public Guid ApplicationId { get; set; }
        public string FirstName { get; set; }    
        public string LastName { get; set; }
        public eRelationship Relationship { get; set; }
        public string Cellphone { get; set; }   
        public string? IDDoc { get; set; }

        [NotMapped]
        public IFormFile? IDFile { get; set; }  

    }
}
