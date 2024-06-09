using ElecPOE.Enums;
using ElecPOE.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.DTO
{
    public class DocumentDTO : Base
    {
        public Guid DocumentId { get; set; }
        public string? Reference { get; set; }
        public eDocumentType DocumentType { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public Guid? StudentId { get; set; }
        public Guid RequestedBy { get; set; }

        public string? SelectedStudentIDs { get; set; }

        public string[] SelectedIDArray { get; set; }

        public Guid? ModuleId { get; set; }
        public Guid? CourseId { get; set; }
        public string Department { get; set; }
        public eSysRole Designation { get; set; }
        public int Quantity { get; set; }
        public string RequestPurpose { get; set; }
        public Guid? ApprovedBy { get; set; }
        public bool IsEmailIssued { get; set; }
        public bool IsHardCopyIssued { get; set; }
        public string? DocumentUpload { get; set; }

        public bool IsReturned = false;
        public IFormFile? DocumentFile { get; set; }
    }
}
