using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.Models;
using ElecPOE.Enums;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ElecPOE.Controllers
{

    [Authorize(Roles = "Admin,Facilitator")]
    public class EmployeeController : Controller
    {

        private readonly IUnitOfWork<Models.File> _context;

        private readonly IUnitOfWork<User> _userContext;

        private readonly IUnitOfWork<LessonPlan> _lessonPlan;

        private readonly IUnitOfWork<Report> _reportContext;

        private IWebHostEnvironment _hostEnvironment;
        public INotyfService _notify { get; }
        public EmployeeController(IUnitOfWork<Models.File> context,

                                IUnitOfWork<User> userContext,

                                IUnitOfWork<Report> reportContext,

                                IUnitOfWork<LessonPlan> lessonPlan,

                                IWebHostEnvironment hostEnvironment,

                                INotyfService notify)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

           _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));

            _lessonPlan = lessonPlan ?? throw new ArgumentNullException(nameof(lessonPlan)); 

            _reportContext = reportContext ?? throw new ArgumentNullException(nameof(reportContext));

            _notify = notify;

        }

        public IActionResult Employees()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> EmployeeFiles(string employeeId)
        {
            IEnumerable<User> users = await _userContext.OnLoadItemsAsync();

            IEnumerable<Report> report = await _reportContext.OnLoadItemsAsync();

            IEnumerable<LessonPlan> plans = await _lessonPlan.OnLoadItemsAsync();

            User finUser = users.FirstOrDefault(n => n.IDPass == employeeId);

            if (finUser is null)
            {
                return NotFound();
            }

            ViewData["UserId"] = finUser.Id;

            ViewData["Id"] = employeeId;

            ViewData["Name"] = $"{finUser.Name} {finUser.LastName}";

            var files = await _context.OnLoadItemsAsync();

            var fileCounts = new Dictionary<string, int>
            {
              { "MonthlyReport", files.Count(f => f.Type == eFileType.MonthlyReport  && f.UserId == finUser.Id) },
              { "Timetable", files.Count(f => f.Type == eFileType.Timetable && f.UserId == finUser.Id) },
              { "TrainingSchedule", files.Count(f => f.Type == eFileType.TrainingSchedule && f.UserId == finUser.Id) },
              { "StudyMaterial", files.Count(f => f.Type == eFileType.StudyMaterial && f.UserId == finUser.Id) },
              { "AssessmentPlan", files.Count(f => f.Type == eFileType.AssessmentPlan && f.UserId == finUser.Id) },
              { "Attendance", files.Count(f => f.Type == eFileType.Attendance && f.UserId == finUser.Id) }
            };

            ViewData["FileCounts"] = fileCounts;

            var monthlyRepo = report.Where(m => m.ReportType == ReportType.Monthly && m.IdPass == finUser.IDPass).Count();

            var weeklyRepo = report.Where(m => m.ReportType == ReportType.Weekly && m.IdPass == finUser.IDPass).Count();

            var lessonPlan = plans.Where(m=> m.IdPass == finUser.IDPass).Count();

            ViewData["monthly"] = monthlyRepo;
             
            ViewData["weekly"] = weeklyRepo;

            ViewData["plans"] = lessonPlan;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmployeeFiles(Models.File file)
        {
            file.FileId = Helper.GenerateGuid();

            file.CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}";

            file.CreatedOn = Helper.OnGetCurrentDateTime();

            file.IsActive = true;

            if (ModelState.IsValid)
            {
                await AttachmentUploader(file);

                var uploadFile = await _context.OnItemCreationAsync(file);

                if(uploadFile != null)
                {
                    int rc = await _context.ItemSaveAsync();

                    if(rc > 0)
                    {
                        TempData["success"] = $"{Helper.GetDisplayName(file.Type)} successfully saved";

                        return RedirectToAction(nameof(EmployeeFiles), new { EmployeeId = OnConvertUserId(file.UserId) });
                    }
                    else
                    {
                        TempData["error"] = $"Error: Unable to save file!!!";
                    }
                }
                else
                {
                    TempData["error"] = $"Error: An error occured!!!";

                }

            }
            else
            {
                TempData["error"] = $"(Model) Error: An error occured!!!";
            }

            return View();
        }  
        public async Task<IActionResult> OnGetDeptEmployees(string Dept) 
        {
            var employees = await _userContext.OnLoadItemsAsync();

            var filterEmps = from n in employees

                             where n.Department.ToString() == Dept

                             orderby n.Username

                             ascending

                             select n;

            return View(filterEmps); 
        }
        public async Task<IActionResult> OnViewFiles(string docType, Guid UserId) 
        {
            var file = await _context.OnLoadItemsAsync();

            var filterEmps = from n in file 

                             where n.Type.ToString().Equals(docType) &&

                             n.UserId == UserId

                             select n;

            List<Models.File> doc = filterEmps.ToList();   

            return View(doc);
        }
        public async Task AttachmentUploader(Models.File attachment)
        {
            try
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                string fileName = Path.GetFileNameWithoutExtension(attachment.AttachmentFile.FileName);

                string extension = Path.GetExtension(attachment.AttachmentFile.FileName);

                attachment.Attachment = fileName = fileName + extension;

                string path = Path.Combine(wwwRootPath + "/EmpFiles/", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await attachment.AttachmentFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";

            }
        }
        public async Task<IActionResult> AttachmentDownload(string filename)
        {
            try
            {
                if (filename == null)

                    return Content("Sorry, no attachment found!!!");

                var path1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                string folder = Path.Combine(path1, @"EmpFiles", filename);

                var memory = new MemoryStream();

                using (var stream = new FileStream(folder, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                return File(memory, Helper.GetContentType(folder), Path.GetFileName(folder));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";

                return View();
            }
        }
        private string OnConvertUserId(Guid UserId)
        {
            return _userContext.OnLoadItemAsync(UserId).Result.IDPass;
        }
        private User OnGetCurrentUser()
        {
            try
            {
                string sessionUserJson = HttpContext.Session.GetString("SessionUser");

                if (string.IsNullOrEmpty(sessionUserJson))
                {
                    return null;
                }

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
