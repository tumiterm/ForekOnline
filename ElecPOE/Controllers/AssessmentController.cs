using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.DTO;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ElecPOE.Controllers
{
    [Authorize]
    public class AssessmentController : Controller
    {
        private readonly IUnitOfWork<AssessmentAttachment> _context;

        private readonly IUnitOfWork<User> _userContext;

        private readonly IUnitOfWork<Module> _modContext;

        private readonly IWebHostEnvironment _hostEnvironment;

        private readonly INotyfService _notify;

        public AssessmentController(
            IUnitOfWork<AssessmentAttachment> context,

            IUnitOfWork<User> userContext,

            IUnitOfWork<Module> modContext,

            IWebHostEnvironment hostEnvironment,

            INotyfService notify)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));

            _modContext = modContext ?? throw new ArgumentNullException(nameof(modContext));

            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

            _notify = notify ?? throw new ArgumentNullException(nameof(notify));
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AttachAssessment(string studentNumber)
        {
            if (string.IsNullOrEmpty(studentNumber))
            {
                return NotFound();
            }

            Student student = await GetStudent(studentNumber);

            if (student is null)
            {
                return NotFound();
            }

            EnrollmentHistory getStudActiveEnrollment = student.EnrollmentHistory.Where(m => m.IsActive).FirstOrDefault();

            var attachments = await _context.OnLoadItemsAsync();

            var filterAttachments = attachments.Where(n => n.StudentNumber == studentNumber);

            List<AssessmentAttachment> list = filterAttachments.ToList();

            dynamic refObj = new ExpandoObject();

            refObj.FileModel = list;

            ViewData["StudentNumber"] = studentNumber;

            ViewData["name"] = $"{student.FirstName} {student.LastName}";

            ViewData["StudentId"] = $"{student.StudentId}";

            var studentEnrollment = student.EnrollmentHistory;

            //for (int i = 0; i < studentEnrollment.Count; i++)
            //{
            //    ViewData["course"] = studentEnrollment[i].CourseTitle.FirstOrDefault();
            //}

            ViewData["course"] = getStudActiveEnrollment.CourseTitle;

            var modList = await _modContext.OnLoadItemsAsync();

            var modFilter = modList.Where(n => !string.IsNullOrEmpty(n.ModuleName));

            IEnumerable<SelectListItem> modules = modFilter.Select(m => new SelectListItem
            {
                Value = m.ModuleId.ToString(),

                Text = $"{m.ModuleName}"
            });

            ViewBag.ModuleId = new SelectList(modules, "Value", "Text");

            return View(refObj);
        }

        [HttpPost]
        public async Task<IActionResult> AttachAssessment(AssessmentAttachment attachment)
        {
            try
            {
                attachment.AttachmentId = Helper.GenerateGuid();

                attachment.IsActive = true;

                attachment.CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}";

                attachment.CreatedOn = Helper.OnGetCurrentDateTime();

                if (ModelState.IsValid)
                {
                    AttachmentUploader(attachment);

                    var attached = await _context.OnItemCreationAsync(attachment);

                    if (attached != null)
                    {
                        int rc = await _context.ItemSaveAsync();

                        if (rc > 0)
                        {
                            TempData["success"] = "Learner attachment successfully saved";

                            return RedirectToAction("AttachAssessment", new { StudentNumber = attachment.StudentNumber, StudentId = attachment.StudentId });
                        }
                        else
                        {
                            TempData["error"] = "Error: Unable to save assessment!";
                        }
                    }
                    else
                    {
                        TempData["error"] = "Error: Something went wrong, contact admin!";
                    }
                }
                else
                {
                    TempData["error"] = "Error: Please fill in all the fields!";

                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $" Error: An unexpected error occurred\n{ex.Message}";
            }

            return View();
        }
       

        public async Task<IActionResult> OnPreviewAssessment(Guid attachmentId)
        {
            var assessment = await _context.OnLoadItemAsync(attachmentId);

            return View(assessment);

        }

        public async Task<Student> GetStudent(string studentNumber)
        {
            try
            {
                Student student = new Student();

                var client = new RestClient($"http://forekapi.dreamline-ict.co.za/api/Student?StudentNumber={studentNumber}");

                var request = new RestRequest();

                request.AddParameter("StudentNumber", studentNumber);

                request.AddHeader("Authorization", $"Bearer {Helper.GenerateJWTToken()}");

                var response = await client.ExecuteGetAsync(request);

                if (response.IsSuccessful)
                {
                    var res = response.Content.ToString();

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
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";

                return null;
            }
        }

        public async Task AttachmentUploader(AssessmentAttachment attachment)
        {
            try
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                string fileName = Path.GetFileNameWithoutExtension(attachment.AttachmentFile.FileName);

                string extension = Path.GetExtension(attachment.AttachmentFile.FileName);

                attachment.Document = fileName = fileName + Helper.GenerateGuid() + extension;

                string path = Path.Combine(wwwRootPath + "/Assessments/", fileName);

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

                string folder = Path.Combine(path1, @"Assessments", filename);

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

        [Route("/Assessment/RemoveDocument/{AttachmentId}")]
        public async Task<IActionResult> RemoveDocument(Guid AttachmentId)
        {
            try
            {
                if (AttachmentId == Guid.Empty)
                {
                    return RedirectToAction("RouteNotFound", "Global");
                }

                AssessmentAttachment assessment = await _context.OnLoadItemAsync(AttachmentId);

                if (assessment is null)
                {
                    return RedirectToAction("RouteNotFound", "Global");
                }

                int del = await _context.OnRemoveItemAsync(AttachmentId);

                if (del > 0)
                {
                    return CreatedAtAction("AttachAssessment", new { StudentNumber = assessment.StudentNumber });
                }

                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";

                return View();
            }
        }
        private async Task<User> OnGetUser(string IdPass)
        {
            try
            {
                List<User> users = await _userContext.OnLoadItemsAsync();

                IEnumerable<User> userFilter = null;

                if (!string.IsNullOrEmpty(IdPass))
                {
                    userFilter = users.Where(n => n.IsActive == true && n.Role == Enums.eSysRole.Facilitator && n.IDPass == IdPass);
                }
                else
                {
                    return null;
                }

                return userFilter.FirstOrDefault(m => m.IDPass == IdPass);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";

                return null;
            }
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
