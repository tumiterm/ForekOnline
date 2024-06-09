using ElecPOE.Enums;
using ElecPOE.Models;

namespace ElecPOE.DTO
{
    public class LearnerAssessmentDTO
    {
        public AssessmentDTO Assessment { get; set; }
        public StudentDTO Attachments { get; set; }
        public TrainingDTO Training { get; set; }

    }
}
