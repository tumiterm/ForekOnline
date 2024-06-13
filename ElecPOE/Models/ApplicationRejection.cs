namespace ElecPOE.Models
{
    public class ApplicationRejection : Base
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; } 
        public Application Application { get; set; }
        public string Reason { get; set; } = string.Empty;  
        public string? AdditionalComments { get; set; } 
        public bool IsFinal { get; set; } 
        public string? NextSteps { get; set; } 
        public DateTime? FollowUpDate { get; set; } 
    }
}
