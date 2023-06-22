using ElecPOE.Enums;

namespace ElecPOE.Models
{
    public class Company : Base
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public Address Address { get; set; }
        public ContactPerson Contact { get; set;}
        public string? Phone { get; set; }
        public eSpeciality Speciality { get; set; } 
    }
}
