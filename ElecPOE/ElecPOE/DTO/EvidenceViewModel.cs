using ElecPOE.Enums;

namespace ElecPOE.DTO
{
    public class EvidenceViewModel
    {
        public eModule Module { get; set; }
        public string? StudentNumber { get; set; }
        public Guid? StudentId { get; set; }
        public string? Photo { get; set; }
    }
}
