using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.Models
{
    public class Minute : Base
    {
        public Guid MinuteId { get; set; }

        [Display(Name ="Meeting Agenda")]
        public string MinuteName { get; set;}


        [Display(Name ="Where was It Held?")]
        public string Venue { get; set;}


        [Display(Name ="Meeting Held?")]
        public DateTime Date { get; set;}


        [Display(Name ="Requested By?")]
        public Guid FacilitatorId { get; set; }


        [Display(Name ="Minutes Type")]
        public eMinuteType MinuteType { get; set; }

        [NotMapped]
        [Display(Name ="Upload Meeting Minutes")]
        public IFormFile? AttachmentFile { get; set; }

        public string? MinuteDoc { get; set; }



    }
}
