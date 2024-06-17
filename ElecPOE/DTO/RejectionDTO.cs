using ElecPOE.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ElecPOE.DTO
{
    public class RejectionDTO
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }

        [ValidateNever]
        public Application Application { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? AdditionalComments { get; set; }
        public bool IsFinal { get; set; }
        public string? NextSteps { get; set; }
        public DateTime? FollowUpDate { get; set; }
    }
}
