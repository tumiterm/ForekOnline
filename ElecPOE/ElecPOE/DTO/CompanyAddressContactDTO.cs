using ElecPOE.Enums;

namespace ElecPOE.DTO
{
    public class CompanyAddressContactDTO
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string? Phone { get; set; }
        public eSpeciality Speciality { get; set; }
        public string? StreetName { get; set; }
        public string? Line1 { get; set; }
        public string? City { get; set; }
        public eProvince? Province { get; set; }
        public string? PostalCode { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Cellphone { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }

    }
}
