using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.Models
{
    public class Medical
    {
        [Key]
        public Guid MedicalId { get; set; }
        public string? MedicalName { get; set; }
        public string? MemberNumber { get; set; }
        public string? Telephone { get; set;}
        public string? Disability { get; set; }

        [ForeignKey(nameof(Application))]
        public Guid ApplicationId { get; set; }
    }
}
