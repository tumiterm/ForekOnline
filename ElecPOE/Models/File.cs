using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.Models
{
    public class File : Base
    {
        public Guid FileId { get; set; }
        public eFileType Type { get; set; }
        public ePhase? Phase { get; set; }
        public Guid UserId { get; set; }
        public DateTime? StartDate { get; set; }    
        public DateTime? EndDate { get; set;}
        public string? Attachment { get; set; }

        [NotMapped]
        public IFormFile? AttachmentFile { get; set; }
    }
}
