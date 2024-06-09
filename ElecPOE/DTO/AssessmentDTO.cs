using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ElecPOE.Models;

namespace ElecPOE.DTO
{
    public class AssessmentDTO : Base
    {
        public string Student{ get; set; }
        public eModule? Module { get; set; }
        public eAssessmentAdministration Type { get; set; }
    }
}
