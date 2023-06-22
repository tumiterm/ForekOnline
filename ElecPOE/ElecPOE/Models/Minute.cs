using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ElecPOE.Enums;

namespace ElecPOE.Models
{
    public class Minute : Base
    {
        public Guid MinuteId { get; set; }

        public string MinuteName { get; set;}

        public string Venue { get; set;}

        public DateTime Date { get; set;}

        public string RequestedBy { get; set;}

        [NotMapped]

        [Display(Name = "Upload Minutes")]
        public IFormFile? AttachmentFile { get; set; }

        public string? MinuteDoc { get; set; }

        public eMeetingType MeetingType { get; set; }
    }
}
