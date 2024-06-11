
#region Using Directives

using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.DTO;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.Reflection;

#endregion

namespace ElecPOE.Controllers
{
    public class ApplicationsController : Controller
    {
        #region Private DbContext

        private readonly IUnitOfWork<Application> _context;
        private readonly IUnitOfWork<Course> _courseContext;
        private readonly IUnitOfWork<Guardian> _guardianContext;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly INotyfService _notify;
        private readonly ILogger<ApplicationsController> _logger;


        #endregion

        public ApplicationsController(

            IUnitOfWork<Application> context,

            IUnitOfWork<Course> courseContext,

            IUnitOfWork<Guardian> guardianContext,

            IWebHostEnvironment hostEnvironment,

            ILogger<ApplicationsController> logger,

            INotyfService notify)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _courseContext = courseContext ?? throw new ArgumentNullException(nameof(courseContext));

            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

            _guardianContext = guardianContext ?? throw new ArgumentNullException(nameof(guardianContext));

            _notify = notify ?? throw new ArgumentNullException(nameof(notify));

            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Apply()
        {
            List<Course> courses = await _courseContext.OnLoadItemsAsync();

            courses = courses.Where(m => m.IsActive).ToList();

            var courseList = courses.Select(s => new SelectListItem
            {
                Value = s.CourseId.ToString(),

                Text = $"{s.CourseName} ({Helper.GetDisplayName(s.Type)}) {s.NType}"
            });

            ViewBag.CourseId = new SelectList(courseList, "Value", "Text");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(ApplyDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Error: Validation error!!!";

                return View(model);
            }

            Application application = CreateApplication(model);

            var response = await SaveApplicationAsync(application);

            if (response > 0)
            {
                await AttachmentUploader(application);

                await QualificationUploader(application);

                await ResidenceUploader(application);

                await UploadFileIfNotNullAsync(application, model.GuardianIDFile, "/GuardianID/", "GuardianIDDoc");

                OnSendEmail(application);

                //OnSendSMS(application);

                return RedirectToAction("Acknowledgement", "Global");

            }

            TempData["success"] = response > 0 ? "Application successfully saved and submitted" : "Error: Application NOT saved!!!";

            return View(model);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Applications()
        {
            var list = await _context.OnLoadItemsAsync();

            List<ApplicationsDTO> applications = new();

            foreach (var item in list)
            {

                var dto = new ApplicationsDTO
                {
                    ApplicationId = item.ApplicationId,

                    IDNumber = item.IDNumber,

                    Email = item.Email,

                    Status = item.Status.ToString(),

                    SubmittedDate = ConvertToDateTime(Helper.ExtractDateFromReference(item.ReferenceNumber)),
                
                    Cellphone = item.Cellphone,

                    Course = await ConvertCourseIdToStringAsync(item.CourseId),

                    Names = $"{item.ApplicantName} {item.ApplicantSurname}",

                    Reference = item.ReferenceNumber,

                    IDPassDoc = item.IDPassDoc ?? "Id Error",

                    QualificationDoc = item.HighestQualDoc ?? "Qualification Error", 

                   
                };

                applications.Add(dto);
            }

            return View(applications);
        }
        public async Task<IActionResult> OnApply(Guid ApplicationId)
        {
            var applications = await _context.OnLoadItemAsync(ApplicationId);

            if(applications is null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            ApplyDTO application = new()
            {
                ApplicantId = ApplicationId,

                ApplicantIDPassFile = applications.IDPassFile,

                ApplicantName = applications.ApplicantName,

                ApplicantSurname = applications.ApplicantSurname,

                ApplicantTitle = applications.ApplicantTitle,

                Cellphone= applications.Cellphone,  

                Email= applications.Email,

                Gender= applications.Gender,

                CourseId = applications.CourseId,

                IDNumber= applications.IDNumber,

                IDPassDoc= applications.IDPassDoc,

                Status= applications.Status,

                Selection= applications.Selection,

                ReferenceNumber= applications.ReferenceNumber,
           
                StudyPermitCategory = applications.StudyPermitCategory,

                HighestQualification = applications.HighestQualification,

                HighestQualDoc = applications.HighestQualDoc,

                ResidenceDoc = applications.ResidenceDoc, 
                
            };

            ViewData["Course"] = await ConvertCourseIdToStringAsync(applications.CourseId);

            ViewBag.Application = applications;

            return View(application);
        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnApply(ApplyDTO model)
        {
            var application = await _context.OnLoadItemAsync(model.ApplicantId);

            var guardians = await _guardianContext.OnLoadItemsAsync();

            var guardian = guardians.FirstOrDefault(m => m.ApplicationId == application.ApplicationId);

            model.GuardianFirstName = guardian.FirstName;

            model.GuardianLastName = guardian.LastName;

            model.GuardianCellphone = guardian.Cellphone;

            model.GuardianRelationship = guardian.Relationship;

            Application app = new()
            {
                ApplicationId = application.ApplicationId,

                ReferenceNumber = model.ReferenceNumber,

                CourseId = application.CourseId,

                HighestQualDoc = model.HighestQualDoc,

                HighestQualification= model.HighestQualification, 
                
                ApplicantTitle = model.ApplicantTitle,

                ApplicantName = model.ApplicantName,

                ApplicantSurname = model.ApplicantSurname,

                Cellphone = model.Cellphone,

                Email = model.Email,

                Gender =model.Gender,

                IDNumber = model.IDNumber,

                PassportNumber = model.PassportNumber,

                Selection = model.Selection,

                Status = model.Status,

                StudyPermitCategory = model.StudyPermitCategory,

                IDPassDoc= model.IDPassDoc,
            };

            var modifyApp = await _context.OnModifyItemAsync(app);

            if (modifyApp != null) {

                TempData["success"] = "Application saved successfully";

                return RedirectToAction(nameof(Applications));
            }
            else
            {
                TempData["error"] = "Error: Unable to save application!!!";
            }


            return View(model);
        }
        private Application CreateApplication(ApplyDTO model)
        {
            Guid key = Helper.GenerateGuid();

            string getID = model.ApplicantIDPassFile.FileName;

            string getQualification = model.AplicantHighestQualFile.FileName;

            string getResidence = model.ApplicantResidenceFile.FileName;

            Application application = new Application
            {
                ApplicationId = key,

                Email = model.Email,

                Cellphone = model.Cellphone,

                ResidenceDoc = getResidence,

                HighestQualDoc = getQualification,

                IDNumber = model.IDNumber,

                IDPassDoc = getID,

                ApplicantName = model.ApplicantName,

                ApplicantSurname = model.ApplicantSurname,

                Selection = model.Selection,

                Status = Enums.ApplicationStatus.Submitted,

                StudyPermitCategory = model.StudyPermitCategory,

                ApplicantTitle = model.ApplicantTitle,

                ReferenceNumber = $"FOR{DateTime.Now.ToShortDateString()}{Helper.RandomStringGenerator(3)}",
                //FOR6/20/2023iYs

                CourseId = model.CourseId,

                Gender = model.Gender,

                PassportNumber = model.PassportNumber,

                HighestQualification = model.HighestQualification,

                HighestQualFile = model.AplicantHighestQualFile,

                IDPassFile = model.ApplicantIDPassFile,

                ResidenceFile = model.ApplicantResidenceFile,

                ApplicantAddress = CreateApplicantAddress(model, key),

                ApplicantGuardian = CreateApplicantGuardian(model, key),
            };

            return application;
        }
        private Address CreateApplicantAddress(ApplyDTO model, Guid applicationId)
        {
            return new Address
            {
                AddressId = Helper.GenerateGuid(),

                StreetName = model.StreetName,

                AssociativeId = applicationId,

                City = model.City,

                Line1 = model.Line1,

                PostalCode = model.PostalCode,

                Province = model.Province
            };
        }
        private Guardian CreateApplicantGuardian(ApplyDTO model, Guid applicationId)
        {
            return new Guardian
            {
                IDDoc = model.IDPassDoc,

                IDFile = model.GuardianIDFile,

                ApplicationId = applicationId,

                Cellphone = model.GuardianCellphone,

                GuardianId = Helper.GenerateGuid(),

                FirstName = model.GuardianFirstName,

                LastName = model.GuardianLastName,

                Relationship = model.GuardianRelationship,
            };
        }
        private async Task<int> SaveApplicationAsync(Application application)
        {
            var response = await _context.OnItemCreationAsync(application);

            if (response != null)
            {
                return await _context.ItemSaveAsync();
            }
            return 0;
        }
        private async Task FileUploader(Application application, IFormFile file, string destinationFolder, string propertyName)
        {
            try
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                string folderPath = Path.Combine(wwwRootPath, destinationFolder);

                string absolutePath = wwwRootPath + folderPath;

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fileName = Path.GetFileNameWithoutExtension(file.FileName);

                string extension = Path.GetExtension(file.FileName);

                PropertyInfo property = typeof(Application).GetProperty(propertyName + "Doc");

                if (property != null)
                {
                    property.SetValue(application, fileName + Helper.GenerateGuid() + extension);
                }

                string path = wwwRootPath + Path.Combine(folderPath, fileName + extension);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
            }
        }
        private async Task UploadFileIfNotNullAsync(Application application, IFormFile file, string directory, string fileName)
        {
            if (file != null)
            {
                await FileUploader(application, file, directory, fileName);
            }
        }
        private void OnSendSMS(Application model)
        {

            Helper.SendSMS("Dear Sir - a new application needing your attention" +
                            $" was just recieved - ref: {model.ReferenceNumber}", "0661401781");
        }
        private async void OnSendEmail(Application model)
        {
            string programme = await ConvertCourseIdToStringAsync(model.CourseId);

            string message = MessageBuilder(model, programme);

            Helper.OnSendMailNotification(model.Email, $"Online Application - Ref: {model.ReferenceNumber}", message, "Forek Online Applications");

        }
        private async Task<string> ConvertCourseIdToStringAsync(Guid courseId)
        {
            if (courseId == Guid.Empty)
            {
                throw new ArgumentException("Invalid course ID provided.");
            }

            Course course = await _courseContext.OnLoadItemAsync(courseId);

            if (course == null)
            {
                throw new InvalidOperationException("Course not found.");
            }

            return $"{course.CourseName} ({Helper.GetDisplayName(course.Type)}) {course.NType}";
        }
        public IActionResult DownloadFile(string fileName, string destinationFolder)
        {
            try
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                string folderPath = Path.Combine(wwwRootPath, destinationFolder);

                string filePath = Path.Combine(folderPath, fileName);

                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                return File(fileStream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";

                return RedirectToAction(nameof(Applications));
            }
        }
        public string MessageBuilder(Application model, string programme)
        {
            string message = $"Dear {model.ApplicantName} {model.ApplicantSurname},\n\n" +

           $"This email is to confirm that we have received your application for the {programme} program.\n\n\n" +
           "Your application is currently being reviewed by our admissions committee.\n\n" +
           "\nContact Information: If you have any questions or need to provide additional information, please do not hesitate to contact our admissions office. You can reach us at 060 728 6757 or via email at info@forek.co.za\n\n" +
           "\nWe appreciate your patience and understanding throughout the admissions process. We will make every effort to keep you informed about your application status and provide a timely response.\n\n" +
           "\nThank you once again for choosing Forek Institute of Technology for your educational aspirations. We wish you the best of luck and look forward to reviewing your application.\n\n" +
           "\nKind regards,\n\n" +
           "\nT Manyimo\n" +
           "\nCampus Manager\n" +
           "\nForek Institute of Technology";

            return message;
        }
        public async Task<IActionResult> AttachmentDownload(string filename)
        {
            try
            {
                if (filename == null)

                    return Content("Sorry, no attachment found!!!");

                var path1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                string folder = Path.Combine(path1, @"AppIDPass", filename);

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
        public async Task QualificationUploader(Application attachment)
        {
            try
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                string fileName = Path.GetFileNameWithoutExtension(attachment.HighestQualFile.FileName);

                string extension = Path.GetExtension(attachment.HighestQualFile.FileName);

                attachment.HighestQualDoc = fileName = fileName + extension;

                string path = Path.Combine(wwwRootPath + "/HighestQualification/", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await attachment.HighestQualFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";

            }
        }
        public async Task AttachmentUploader(Application attachment)
        {
            try
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                string fileName = Path.GetFileNameWithoutExtension(attachment.IDPassFile.FileName);

                string extension = Path.GetExtension(attachment.IDPassFile.FileName);

                attachment.IDPassDoc = fileName = fileName + extension;

                string path = Path.Combine(wwwRootPath + "/AppIDPass/", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await attachment.IDPassFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";

            }
        }
        public async Task<IActionResult> QualificationDownload(string filename)
        {
            try
            {
                if (filename == null)

                    return Content("Sorry, no attachment found!!!");

                var path1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                string folder = Path.Combine(path1, @"HighestQualification", filename);

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
        public async Task ResidenceUploader(Application attachment)
        {
            try
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                string fileName = Path.GetFileNameWithoutExtension(attachment.ResidenceFile.FileName);

                string extension = Path.GetExtension(attachment.ResidenceFile.FileName);

                attachment.ResidenceDoc = fileName = fileName + extension;

                string path = Path.Combine(wwwRootPath + "/Residence/", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await attachment.ResidenceFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";

            }
        }
        public async Task<IActionResult> ResidenceDownload(string filename)
        {
            try
            {
                if (filename == null)

                    return Content("Sorry, no attachment found!!!");

                var path1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                string folder = Path.Combine(path1, @"Residence", filename);

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
        public IActionResult FileDownload(string filename,string folder)
        {
            try
            {
                if (filename == null)

                    return Content("Sorry, no attachment found!!!");

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder, filename);

                if (!System.IO.File.Exists(path))

                    return Content("Sorry, the file does not exist!!!");

                var fileBytes = System.IO.File.ReadAllBytes(path);

                return File(fileBytes, Helper.GetContentType(path), Path.GetFileName(path));
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                return View();
            }
        }
        public void OnNotifyApplicant(string UserEmail, string UserPhone, string Message, bool IsSMS, bool IsEmail)
        {
            try
            {
                Helper.SendSMS(Message, UserPhone);

            } catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }
           
        }
        private DateTime ConvertToDateTime(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                throw new ArgumentException("Input string cannot be null or empty.", nameof(dateString));
            }

            string dateFormat = "dd/MM/yyyy";


            if (DateTime.TryParseExact(dateString, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            else
            {
                throw new FormatException("The input string is not in the correct format.");
            }
        }


        /// <summary>
        /// Inspects a list of applications and checks if there are any with a status of "Pending" for more than 24 hours.
        /// If such applications are found, notifications are sent.
        /// </summary>
        /// <param name="applications">The list of applications to inspect.</param>
        private void InspectPendingApplications(List<ApplicationsDTO> applications)
        {
            if (applications == null || !applications.Any())
            {
                _logger.LogInformation("No applications to inspect.");

                return;
            }

            DateTime twentyFourHoursAgo = DateTime.UtcNow.AddHours(-24);

            var staleApplications = applications.Where(x => x.Status.ToString() == Enums.ApplicationStatus.Pending.ToString() && x.SubmittedDate <= twentyFourHoursAgo).ToList();

            if (staleApplications.Any())
            {
                _logger.LogInformation($"{staleApplications.Count} pending applications are older than 24 hours.");

                foreach (var application in staleApplications)
                {
                    SendNotification(application);
                }
            }
            else
            {
                _logger.LogInformation("No pending applications older than 24 hours.");
            }
        }

        /// <summary>
        /// Sends a notification for a given application.
        /// </summary>
        /// <param name="application">The application for which to send the notification.</param>
        private void SendNotification(ApplicationsDTO application)
        {
            Helper.SendSMS("", "");
        }


    }
}
