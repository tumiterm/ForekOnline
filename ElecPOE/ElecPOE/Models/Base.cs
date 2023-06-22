namespace ElecPOE.Models
{
    public class Base
    {
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedOn { get; set; }
        public string? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
