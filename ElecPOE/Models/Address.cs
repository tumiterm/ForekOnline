using ElecPOE.Enums;

namespace ElecPOE.Models
{
    public class Address
    {
        public Guid AddressId { get; set; } 
        public string? StreetName { get; set; }
        public string? Line1 { get; set; }
        public string? City { get; set; }
        public eProvince? Province { get; set; }
        public string? PostalCode { get; set; }
        public Guid AssociativeId { get; set; } 

    }
}
