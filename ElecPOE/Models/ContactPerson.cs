
using System.ComponentModel.DataAnnotations;

namespace ElecPOE.Models
{
    public class ContactPerson
    {
        [Key]
        public Guid ContactId { get; set; }
        public Guid AssociativeId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Cellphone { get; set; }
        public string Email { get; set; }   
    }
}
