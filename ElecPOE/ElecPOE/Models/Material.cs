using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.Models
{
    public class Material : Base
    {
        [Key]
        public Guid MaterialId { get; set; }
        public string? Document { get; set; }
        public eMaterialType Type { get; set; }

        [DataType(DataType.MultilineText)]
        public string? Message { get; set; }
        public DateTime? DueDate { get; set; }
        public eTrade Trade { get; set; }
        public eModule? Module { get; set; }
        public string? InstructorId { get; set; }

        [NotMapped]
        public IFormFile AttachmentFile { get; set; }
    }
}
