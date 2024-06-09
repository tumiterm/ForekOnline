using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using RestSharp;
using System.Dynamic;
using System.Numerics;

namespace ElecPOE.Controllers
{
    [Authorize]
    public class TrainingAssessmentController : Controller
    {

        private readonly IUnitOfWork<Training> _context;

        private IWebHostEnvironment _hostEnvironment;
        public INotyfService _notify { get; }
        public TrainingAssessmentController(IUnitOfWork<Training> context,
                                IWebHostEnvironment hostEnvironment,
                                INotyfService notify)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

            _notify = notify ?? throw new ArgumentNullException(nameof(notify));

        }
        [HttpGet]
        public async Task<IActionResult> TrainingMaterial(string StudentNumber)
        {

            if (string.IsNullOrEmpty(StudentNumber))
            {

                TempData["error"] = "Training Material Error:Student number is required.";
            }

            try
            {

                Student student = await GetStudent(StudentNumber);

                if (student is null)
                {
                    return NotFound();
                }

                ViewData["name"] = $"{student.FirstName} {student.LastName}";

                //for(int i=0; i<student.EnrollmentHistory.Count; i++)
                //{
                //    ViewData["course"] = $"{student.EnrollmentHistory[i].CourseTitle}";
                //}

                if (student.EnrollmentHistory.Count > 0)
                {
                    ViewData["course"] = student.EnrollmentHistory[0].CourseTitle;
                }

                dynamic trainingObj = new ExpandoObject();

                List<Training> trainings = await _context.OnLoadItemsAsync();

                var filterTraining = trainings.Where(n => n.StudentNumber == StudentNumber).ToList();

                trainingObj.MaterialList = filterTraining.ToList();

                return View(trainingObj);
            }
            catch
            {
                TempData["error"] = "An error occurred while retrieving training materials";

                return View();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TrainingMaterial(Training training)
        {
            training.IsActive = true;

            training.CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}";

            training.CreatedOn = Helper.OnGetCurrentDateTime();

            AttachmentUploader(training);

            var material = await _context.OnItemCreationAsync(training);

            if (material != null)
            {
                int rc = await _context.ItemSaveAsync();

                if (rc > 0)
                {

                    TempData["success"] = "File saved successfully";

                    return RedirectToAction(nameof(TrainingMaterial), new { StudentNumber = training.StudentNumber });
                }
                else
                {
                    TempData["error"] = "Error: File upload failed";
                }
            }


            return View();
        }
        public async Task<Student> GetStudent(string StudentNumber)
        {

            try
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
                }

                return student;

            }
            catch (Exception ex)
            {
               TempData["error"] ="An error occurred while retrieving the student information.";

                return null;
            }

        }
        public async void AttachmentUploader(Training attachment)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;

            string fileName = Path.GetFileNameWithoutExtension(attachment.DocumentFile.FileName);

            string extension = Path.GetExtension(attachment.DocumentFile.FileName);

            attachment.Document = fileName = fileName + Helper.GenerateGuid() + extension;

            string path = Path.Combine(wwwRootPath + "/Training/", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await attachment.DocumentFile.CopyToAsync(fileStream);
            }
        }
        public async Task<IActionResult> AttachmentDownload(string filename)
        {
            if (filename == null)

                return Content("Sorry NO Attachment found!!!");


            var path1 = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot");

            string folder = path1 + @"\Training\" + filename;

            var memory = new MemoryStream();

            using (var stream = new FileStream(folder, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, Helper.GetContentType(folder), Path.GetFileName(folder));
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> RemoveDocument(Guid AttachmentId)
        {
            if (AttachmentId == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            Training attachment = await _context.OnLoadItemAsync(AttachmentId);

            if (attachment is null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            int del = await _context.OnRemoveItemAsync(AttachmentId);

            if (del > 0)
            {
                return RedirectToAction("TrainingMaterial", new { StudentNumber = attachment.StudentNumber, StudentId = attachment.StudentId });
            }

            return View();

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
