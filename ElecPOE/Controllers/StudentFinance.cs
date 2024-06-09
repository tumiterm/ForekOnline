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
    [Authorize]
    public class StudentFinanceController : Controller
    {
        private readonly IUnitOfWork<LearnerFinance> _context;

        private IWebHostEnvironment _hostEnvironment;
        public INotyfService _notify { get; }
        public StudentFinanceController(IUnitOfWork<LearnerFinance> context,
                                IWebHostEnvironment hostEnvironment,
                                INotyfService notify)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

            _notify = notify ?? throw new ArgumentNullException(nameof(notify));

        }

        public async Task<IActionResult> UploadStudentStatement(string StudentNumber, Guid StudentId)
        {
            if (string.IsNullOrEmpty(StudentNumber) && StudentId == Guid.Empty)
            {
                return NotFound();
            }

            var attachments = await _context.OnLoadItemsAsync();

            var filterAttachments = from n in attachments

                                    where n.StudentNumber == StudentNumber

                                    select n;

            List<LearnerFinance> list = filterAttachments.ToList();

            dynamic refObj = new ExpandoObject();

            refObj.StatementList = list;

            Student student = await GetStudent(StudentNumber);

            ViewData["Studenz"] = $"{student.FirstName} {student.LastName}";

            return View(refObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadStudentStatement(LearnerFinance finance)
        {
            finance.IsActive = true;

            finance.Id = Helper.GenerateGuid();

            finance.CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}";

            finance.CreatedOn = Helper.OnGetCurrentDateTime();

            if (ModelState.IsValid)
            {
                AttachmentUploader(finance);

                var statement = await _context.OnItemCreationAsync(finance);

                if(statement != null)
                {
                    int rc = await _context.ItemSaveAsync();

                    if(rc > 0 )
                    {
                        TempData["success"] = "Student statement saved successfully";

                        return RedirectToAction(nameof(UploadStudentStatement), new { StudentNumber = statement.StudentNumber, StudentId = statement.StudentId });
                    }
                    else
                    {
                        TempData["error"] = "Error: Unable to save student statement";
                    }
                }
                else
                {
                    TempData["error"] = "Error: Something went wrong, Please try again!!!";
                }
            }
            else
            {
                TempData["error"] = "Error: Please fill in all the information!!!";
            }

            return View();
        }
        public async Task<IActionResult> AttachmentDownload(string filename)
        {
            if (filename == null)

                return Content("Sorry NO Statements found!!!");


            var path1 = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot");

            string folder = path1 + @"\Statements\" + filename;

            var memory = new MemoryStream();

            using (var stream = new FileStream(folder, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, Helper.GetContentType(folder), Path.GetFileName(folder));
        }
        public async void AttachmentUploader(LearnerFinance attachment)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;

            string fileName = Path.GetFileNameWithoutExtension(attachment.FileAttach.FileName);

            string extension = Path.GetExtension(attachment.FileAttach.FileName);

            attachment.File = fileName = fileName + Helper.GenerateGuid() + extension;

            string path = Path.Combine(wwwRootPath + "/Statements/", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await attachment.FileAttach.CopyToAsync(fileStream);
            }
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> RemoveAttachment(Guid ReportId)
        {
            if (ReportId == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            LearnerFinance attachment = await _context.OnLoadItemAsync(ReportId);

            if (attachment is null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            int del = await _context.OnRemoveItemAsync(ReportId);

            if (del > 0)
            {
                return RedirectToAction("UploadStudentStatement", new { StudentNumber = attachment.StudentNumber, StudentId = attachment.StudentId });

            }

            return View();

        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<Student> GetStudent(string StudentNumber)
        {
            string res = string.Empty;

            Student student = new Student();

            var client = new RestClient($"http://forekapi.dreamline-ict.co.za/api/StudentId?StudentNumber={StudentNumber}");

            var request = new RestRequest();

            request.AddParameter("StudentNumber", StudentNumber);

            request.AddHeader("Authorization", $"Bearer {Helper.GenerateJWTToken()}");

            var response = await client.ExecuteGetAsync(request);

            if (response.IsSuccessful)
            {
                res = response.Content.ToString();

                student = JsonConvert.DeserializeObject<Student>(res);

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

            return student;
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

    }
}
