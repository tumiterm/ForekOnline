namespace ElecPOE.DTO
{
    public class ApplicationsDTO
    {
        public Guid ApplicationId { get; set; }
        public string Course { get; set; }  
        public string Status { get; set; }
        public string Names { get; set; }
        public string? Email { get; set; }
        public string Reference { get; set; }
        public string IDNumber { get; set; }    
        public string Cellphone { get; set; }
        public string IDPassDoc { get; set; }
        public string QualificationDoc { get; set; }  
        public bool IsSMS { get; set; }
        public bool IsEmail { get; set; }   

        public DateTime SubmittedDate { get; set; }
    }
}
