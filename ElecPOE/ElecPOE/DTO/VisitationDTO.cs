﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ElecPOE.DTO
{
    public class VisitationDTO
    {
        public Guid VisitId { get; set; }
        public string Company { get; set; }
        public bool HasReport { get; set; }
        public string? Report { get; set; }
        public DateTime Date { get; set; }
        public string? LearnerFeedback { get; set; }
        public string? VisitPurpose { get; set; }
        public string? Mentor { get; set; }

        [NotMapped]
        public IFormFile? ReportFile { get; set; }
    }
}
