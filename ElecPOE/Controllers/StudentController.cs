using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.DTO;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Dynamic;

namespace ElecPOE.Controllers
{
    public class StudentController : Controller
    {
        private readonly IUnitOfWork<StudentAttachment> _context;

        private IWebHostEnvironment _hostEnvironment;
        public INotyfService _notify { get; }
        public StudentController(IUnitOfWork<StudentAttachment> context,
                                IWebHostEnvironment hostEnvironment,
                                INotyfService notify)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

            _notify = notify ?? throw new ArgumentNullException(nameof(notify));

        }

        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        public async Task<IActionResult> StudentList()
        {
            string token = Helper.GenerateJWTToken();

            List<Student> students = new();

            HttpClient client = Helper.Initialize("http://forekapi.dreamline-ict.co.za/api/Students/");

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            HttpResponseMessage res = await client.GetAsync("http://forekapi.dreamline-ict.co.za/api/Students/");

            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;

                students = JsonConvert.DeserializeObject<List<Student>>(results);
            }

            ViewData["CurrentUser"] = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}";

            ViewData["role"] = OnGetCurrentUser().Role.ToString();

            return View(students);
        }
        public async Task<IActionResult> StudentDetail(string StudentNumber)
        {
            if (string.IsNullOrEmpty(StudentNumber))
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            Student student = await GetStudent(StudentNumber);

            if (student.EnrollmentHistory.Count == 0 || student.EnrollmentHistory == null)
            {
                return View(nameof(Unassigned));
            }

            if (student.StudentNumber is null)
            {
                return RedirectToAction("RouteNotFound", "Global");

            }

            ViewData["CurrentUser"] = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}";

            ViewData["role"] = OnGetCurrentUser().Role.ToString();

            return View(student);
        }
        [HttpGet]
        public async Task<IActionResult> StudentDocuments(string StudentNumber)
        {
            Student student = await GetStudent(StudentNumber);

            if (String.IsNullOrEmpty(StudentNumber) || student == null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            var attachments = await _context.OnLoadItemsAsync();

            var filterAttachments = from n in attachments

                                    where n.StudentNumber == StudentNumber

                                    select n;

            List<StudentAttachment> list = filterAttachments.ToList();

            dynamic refObj = new ExpandoObject();

            refObj.FileModel = list;

            ViewData["StudentNumber"] = StudentNumber;

            ViewData["name"] = $"{student.FirstName} {student.LastName}";

            ViewData["StudentId"] = $"{student.StudentId}";

            return View(refObj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudentDocuments(StudentAttachment attachment)
        {
            Student student = await GetStudent(attachment.StudentNumber);

            attachment.AttachmentId = Helper.GenerateGuid();

            attachment.CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}";

            attachment.CreatedOn = Helper.OnGetCurrentDateTime();

            attachment.IsActive = true;

            attachment.StudentNumber = student.StudentNumber;

            string fileName = attachment.AttachmentFile.FileName;

            attachment.Document = $"{fileName}";

            attachment.StudentId = student.StudentId;

            if (ModelState.IsValid)
            {
                AttachmentUploader(attachment);

                StudentAttachment file = await _context.OnItemCreationAsync(attachment);

                if (file != null)
                {
                    int rc = await _context.ItemSaveAsync();

                    if (rc > 0)
                    {
                        TempData["success"] = $"{file.DocumentName} successfully saved and uploaded";

                        return RedirectToAction("StudentDocuments", new { StudentNumber = attachment.StudentNumber, StudentId = attachment.StudentId });
                    }
                    else
                    {
                        TempData["error"] = $"Error: UNABLE to saved and uploaded file!!!";
                    }
                }
                else
                {
                    TempData["error"] = $"Error: something went wrong!!!";
                }
            }
            else
            {
                TempData["error"] = $"Error: File upload required!!!";
            }

            return View();

        }
        public async Task<Student> GetStudent(string StudentNumber)
        {
            string res = string.Empty;

            Student student = new();

            var client = new RestClient($"http://forekapi.dreamline-ict.co.za/api/Student?StudentNumber={StudentNumber}");

            var request = new RestRequest();

            request.AddParameter("StudentNumber", StudentNumber);

            request.AddHeader("Authorization", $"Bearer {Helper.GenerateJWTToken()}");

            var response = await client.ExecuteGetAsync(request);

            if (response.IsSuccessful)
            {
                res = response.Content.ToString();

                student = JsonConvert.DeserializeObject<Student>(res);

                if (student.EnrollmentHistory.Count > 0)
                {
                    EnrollmentHistoryViewModel enrollment = new EnrollmentHistoryViewModel
                    {
                        CourseId = student.EnrollmentHistory[0].CourseId,

                        StartDate = student.EnrollmentHistory[0].StartDate,

                        CourseTitle = student.EnrollmentHistory[0].CourseTitle,

                        StudentId = student.EnrollmentHistory[0].StudentId,

                        CourseType = student.EnrollmentHistory[0].CourseType,

                        EnrollmentId = student.EnrollmentHistory[0].EnrollmentId,

                        EnrollmentStatus = student.EnrollmentHistory[0].EnrollmentStatus,

                        IsActive = student.EnrollmentHistory[0].IsActive

                    };
                }
                else
                {
                    _notify.Error("Error: Student not registered to ANY course!!!", 5);
                }
            }

            return student;
        }
        public async void AttachmentUploader(StudentAttachment attachment)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;

            string fileName = Path.GetFileNameWithoutExtension(attachment.AttachmentFile.FileName);

            string extension = Path.GetExtension(attachment.AttachmentFile.FileName);

            attachment.Document = fileName = fileName + Helper.GenerateGuid() + extension;

            string path = Path.Combine(wwwRootPath + "/Docs/", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await attachment.AttachmentFile.CopyToAsync(fileStream);
            }
        }
        public async Task<IActionResult> AttachmentDownload(string filename)
        {
            if (filename == null)

                return Content("Sorry NO Attachment found!!!");


            var path1 = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot");

            string folder = path1 + @"\Docs\" + filename;

            var memory = new MemoryStream();

            using (var stream = new FileStream(folder, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, Helper.GetContentType(folder), Path.GetFileName(folder));
        }
        public async Task<IActionResult> RemoveDocument(Guid AttachmentId)
        {
            if (AttachmentId == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            StudentAttachment attachment = await _context.OnLoadItemAsync(AttachmentId);

            if (attachment is null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            int del = await _context.OnRemoveItemAsync(AttachmentId);

            if (del > 0)
            {
                return RedirectToAction("StudentDocuments", new { StudentNumber = attachment.StudentNumber, StudentId = attachment.StudentId });
            }

            return View();

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
        public IActionResult Unassigned()
        {
            return View();
        }

    }
}
