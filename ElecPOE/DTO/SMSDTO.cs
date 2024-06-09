using ElecPOE.Models;

namespace ElecPOE.DTO
{
    public class SMSDTO
    {
        public string message { get; set; }
        public List<Recipient> recipients { get; set; }
        public string? scheduledTime { get; set; }
        public int? maxSegments { get; set; }
    }
}
