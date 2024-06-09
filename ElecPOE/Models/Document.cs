using ElecPOE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecPOE.Models
{
    public class Document :  Base
    {
        [Key]
        public Guid DocumentId { get; set; }
        public string? Reference { get; set; }
        public eDocumentType DocumentType { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        [ForeignKey(nameof(Models.Student))]
        public Guid? Student { get; set; }

        [ForeignKey(nameof(User))]    
        public Guid RequestedBy { get; set;}

        [ForeignKey(nameof(Module))]  
        public Guid? ModuleId { get; set; }

        [ForeignKey(nameof(Course))]
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
        public string? SelectedStudentIDs { get; set; }

        [NotMapped]
        public string[] SelectedIDArray { get; set; }

        [NotMapped]
        public IFormFile? DocumentFile { get; set; }    


    }
}
