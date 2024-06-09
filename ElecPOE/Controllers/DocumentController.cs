using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.DTO;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace ElecPOE.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IUnitOfWork<Document> _context;
        private readonly IUnitOfWork<Module> _modContext;
        private readonly IUnitOfWork<Course> _courseContext;

        private readonly IWebHostEnvironment _hostEnvironment;

        private readonly INotyfService _notify;

        public DocumentController(

            IUnitOfWork<Document> context,

            IUnitOfWork<Module> modContext,

            IUnitOfWork<Course> courseContext,

            IWebHostEnvironment hostEnvironment,

            INotyfService notify)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _modContext = modContext ?? throw new ArgumentNullException(nameof(modContext));

            _courseContext = courseContext ?? throw new ArgumentNullException(nameof(courseContext));

            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

            _notify = notify ?? throw new ArgumentNullException(nameof(notify));
        }

        [HttpGet]
        public async Task<IActionResult> RequestDocument()
        {
            var students = await StudentList();

            List<Course> courses = await _courseContext.OnLoadItemsAsync();

            List<Module> modules = await _modContext.OnLoadItemsAsync();

            var regStudents = students.Where(n => !string.IsNullOrEmpty(n.StudentNumber));

            IEnumerable<SelectListItem> studObj = regStudents.Select(m => new SelectListItem
            {
                Value = m.StudentId.ToString(),

                Text = $"{m.FirstName} {m.LastName} ({m.StudentNumber})"
            });

            IEnumerable<SelectListItem> getCourses = from n in courses

                                                     select new SelectListItem
                                                     {
                                                         Value = n.CourseId.ToString(),

                                                         Text = $"{n.CourseName} ({Helper.GetDisplayName(n.Type)} {n.NType})"
                                                     };

            IEnumerable<SelectListItem> getModules = from n in modules

                                                     where n.IsActive == true

                                                     && n.ModuleName != null && n.ModuleName!= string.Empty

                                                     select new SelectListItem
                                                     {
                                                         Value = n.ModuleId.ToString(),

                                                         Text = $"{n.ModuleName}"
                                                     };

            ViewBag.StudentId = new SelectList(studObj, "Value", "Text");

            ViewBag.CourseId = new SelectList(getCourses, "Value", "Text");

            ViewBag.ModuleId = new SelectList(getModules, "Value", "Text");

            return View();  
        }
        public async Task<IActionResult> DocumentsRequested()
        {
            var onLoadDocs = await _context.OnLoadItemsAsync();

            List<DocumentDTO> documents = new();

            foreach (var item in onLoadDocs)
            {
                DocumentDTO document = new()
                {
                    DocumentId = item.DocumentId,

                    Department = item.Department,

                    Designation = item.Designation,

                    ApprovedBy = item.ApprovedBy,

                    CourseId = item.CourseId,

                    IsHardCopyIssued= item.IsHardCopyIssued,

                    DocumentFile = item.DocumentFile,

                    DocumentType = item.DocumentType,

                    StudentId = item.Student,

                    SelectedIDArray= item.SelectedIDArray,

                    SelectedStudentIDs = item.SelectedStudentIDs,

                    DocumentUpload = item.DocumentUpload,

                    IsEmailIssued = item.IsEmailIssued, 
                    
                    IsReturned = item.IsReturned,

                    ModuleId = item.ModuleId,

                    RequestDate = item.RequestDate,

                    Quantity = item.Quantity,

                    Reference = item.Reference,

                    RequestedBy = item.RequestedBy,

                    RequestPurpose = item.RequestPurpose,

                    ReturnDate = item.ReturnDate, 
                };

                documents.Add(document);
            }

            return View(documents);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestDocument(Document document)
        {
            document.DocumentId = Helper.GenerateGuid();

            string reference = GetLastReferenceFromDB();

            document.Reference = $"{Helper.IncrementReference(reference,1)}";

            document.CreatedOn = Helper.OnGetCurrentDateTime();

            document.CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}";

            document.RequestedBy = OnGetCurrentUser().Id;

            document.IsActive = true;

            document.IsReturned = false;

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Error: Something went wrong!!!";
            }

            var createDoc = await _context.OnItemCreationAsync(document);

            if(createDoc != null)
            {
                //watch

                document.SelectedStudentIDs = string.Join(",", document.SelectedIDArray);

                int rc = await _context.ItemSaveAsync();
                 
                if(rc > 0)
                {
                    TempData["success"] = $"{Helper.GetDisplayName(document.DocumentType)} successfully saved";
                }
                else
                {
                    TempData["error"] = $"Error: document NOT saved!!!";
                }
            }
            else
            {
                TempData["error"] = $"Error: unable to save document!!!";
            }


            return View();
        }

        [HttpGet]
        public async Task<IActionResult> OnRequestDocument(Guid DocumentId)
        {
            Document document = await _context.OnLoadItemAsync(DocumentId);

            if (document is null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            DocumentDTO documentDTO = new()
            {
                DocumentId = document.DocumentId,

                Department = document.Department,

                Designation = document.Designation,

                ApprovedBy = document.ApprovedBy,

                SelectedIDArray = document.SelectedStudentIDs.Split(',').ToArray(),

                CourseId = document.CourseId,

                IsHardCopyIssued = document.IsHardCopyIssued,

                DocumentFile = document.DocumentFile,

                DocumentType = document.DocumentType,

                StudentId = document.Student,

                DocumentUpload = document.DocumentUpload,

                IsEmailIssued = document.IsEmailIssued,

                IsReturned = document.IsReturned,

                ModuleId = document.ModuleId,

                RequestDate = document.RequestDate,

                Quantity = document.Quantity,

                Reference = document.Reference,

                RequestedBy = document.RequestedBy,

                RequestPurpose = document.RequestPurpose,

                ReturnDate = document.ReturnDate,
            };

            var students = await StudentList();

            List<Course> courses = await _courseContext.OnLoadItemsAsync();

            List<Module> modules = await _modContext.OnLoadItemsAsync();

            var regStudents = students.Where(n => !string.IsNullOrEmpty(n.StudentNumber));

            IEnumerable<SelectListItem> studObj = regStudents.Select(m => new SelectListItem
            {
                Value = m.StudentId.ToString(),

                Text = $"{m.FirstName} {m.LastName} ({m.StudentNumber})"
            });

            IEnumerable<SelectListItem> getCourses = from n in courses

                                                     select new SelectListItem
                                                     {
                                                         Value = n.CourseId.ToString(),

                                                         Text = $"{n.CourseName} ({Helper.GetDisplayName(n.Type)} {n.NType})"
                                                     };

            IEnumerable<SelectListItem> getModules = from n in modules

                                                     where n.IsActive == true

                                                     && n.ModuleName != null && n.ModuleName != string.Empty

                                                     select new SelectListItem
                                                     {
                                                         Value = n.ModuleId.ToString(),

                                                         Text = $"{n.ModuleName}"
                                                     };


            ViewBag.StudentId = new SelectList(studObj, "Value", "Text");

            ViewBag.CourseId = new SelectList(getCourses, "Value", "Text");

            ViewBag.ModuleId = new SelectList(getModules, "Value", "Text");

            return View(documentDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnRequestDocument(DocumentDTO document)
        {
            document.ModifiedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}";

            document.ModifiedOn = Helper.OnGetCurrentDateTime();

            if (ModelState.IsValid)
            {
                Document doc = Helper.MapProperties<DocumentDTO, Document>(document);

                var docObj = await _context.OnModifyItemAsync(doc);

                if(docObj != null)
                {
                    TempData["success"] = $"Record successfully saved";
                }
                else
                {
                    TempData["error"] = $"Error: Unable to save record!!!";
                }
            }

            return View();
        }
        private async Task<List<Student>> StudentList()
        {
            return await Helper.GetStudentListAsync();
        }
        public async Task<JsonResult> GetCourseModuleId(Guid CourseId)
        {
            var mods = await _modContext.OnLoadItemsAsync();

            var filterMods = from n in mods

                             where n.CourseIdFK == CourseId

                             select n;

            return Json(filterMods.ToList());
        }
        private User OnGetCurrentUser()
        {
            string sessionUserJson = HttpContext.Session.GetString("SessionUser");

            if (string.IsNullOrEmpty(sessionUserJson))
            {
                return null;
            }

            try
            {
                User user = JsonConvert.DeserializeObject<User>(sessionUserJson);

                return user;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        public string GetLastReferenceFromDB()
        {
            var lastRecord = _context.OnLoadItemsAsync().Result.OrderByDescending(x => x.Reference).FirstOrDefault();

            return lastRecord.Reference;
        }


    }
}
