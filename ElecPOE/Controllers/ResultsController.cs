using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.DTO;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RestSharp;
using System.Dynamic;

namespace ElecPOE.Controllers
{
    public class ResultsController : Controller
    {
        private readonly IUnitOfWork<ProgressReport> _context;
      
        private IWebHostEnvironment _hostEnvironment;
        public INotyfService _notify { get; }
        public ResultsController(IUnitOfWork<ProgressReport> context,
                                IWebHostEnvironment hostEnvironment,
                               INotyfService notify) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

            _notify = notify ?? throw new ArgumentNullException(nameof(notify));

        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> StudentResults(string StudentNumber)
        {
            dynamic res = new ExpandoObject();

            List<ProgressReport> report = await _context.OnLoadItemsAsync();

            var repList = from n in report

                          where n.StudentNumber== StudentNumber

                          select n; 

            List<ProgressReport> filterReport = repList.ToList();

            res.Repo = filterReport;

            Student stud = await GetStudent(StudentNumber);

            ViewData["St"] = $"{stud.FirstName} {stud.LastName}";

            ViewData["StudentNumber"] = StudentNumber;

            ViewData["StudentId"] = stud.StudentId;

            return View(res);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudentResults(ProgressReport report)
        {
            report.IsActive = true;

            report.CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}";

            report.CreatedOn = Helper.OnGetCurrentDateTime();

            if (ModelState.IsValid)
            {
               AttachmentUploader(report);

                var resModel = await _context.OnItemCreationAsync(report);

                if(resModel != null)
                {
                    int rc = await _context.ItemSaveAsync();

                    if(rc > 0) {

                        TempData["success"] = "Learner results successfully uploaded";

                        return RedirectToAction(nameof(StudentResults),new { StudentNumber = report.StudentNumber});
                    }
                    else
                    {
                        TempData["error"] = "Error: Learner results NOT uploaded";
                    }
                }
            }

            return View();
        }
        public async Task<Student> GetStudent(string StudentNumber)
        {
            string res = string.Empty;

            Student student = new Student();

            var client = new RestClient($"http://forekapi.dreamline-ict.co.za/api/Student?StudentNumber={StudentNumber}");

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
        public async void AttachmentUploader(ProgressReport attachment)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;

            string fileName = Path.GetFileNameWithoutExtension(attachment.ReportFile.FileName);

            string extension = Path.GetExtension(attachment.ReportFile.FileName);

            attachment.AttachReport = fileName = fileName + Helper.GenerateGuid() + extension;

            string path = Path.Combine(wwwRootPath + "/Results/", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await attachment.ReportFile.CopyToAsync(fileStream);
            }
        }
        public async Task<IActionResult> AttachmentDownload(string filename)
        {
            if (filename == null)

                return Content("Sorry NO Attachment found!!!");


            var path1 = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot");

            string folder = path1 + @"\Results\" + filename;

            var memory = new MemoryStream();

            using (var stream = new FileStream(folder, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, Helper.GetContentType(folder), Path.GetFileName(folder));
        }

        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        public async Task<IActionResult> RemoveProgressReport(Guid ReportId)
        {
            if (ReportId == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            ProgressReport report = await _context.OnLoadItemAsync(ReportId);

            if (report is null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            int del = await _context.OnRemoveItemAsync(ReportId);

            if (del > 0)
            {
                TempData["success"] = "Report removed successfully";

                return RedirectToAction(nameof(StudentResults), new { StudentNumber = report.StudentNumber });

            }
            else
            {
                TempData["error"] = "Report NOT removed";
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
    }


}
