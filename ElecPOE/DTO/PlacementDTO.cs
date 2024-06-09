using ElecPOE.Enums;

namespace ElecPOE.DTO
{
    public class PlacementDTO
    {
        public Guid PlacementId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string CompanyId { get; set; }
        public string Student { get; set; }
        public string PlacedBy { get; set; }
        public eStatus Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CompletionDate { get; set; }
    }
}
