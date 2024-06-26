﻿
#region Using Directives

using AspNetCoreHero.ToastNotification.Abstractions;
using Azure;
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
        private readonly IUnitOfWork<ApplicationRejection> _rejContext;
        private readonly IUnitOfWork<Course> _courseContext;
        private readonly IUnitOfWork<Guardian> _guardianContext;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly INotyfService _notify;
        private readonly ILogger<ApplicationsController> _logger;
        private readonly IHelperService _helperService;


        #endregion

        public ApplicationsController(

            IUnitOfWork<Application> context,

            IUnitOfWork<ApplicationRejection> rejContext,

            IUnitOfWork<Course> courseContext,

            IUnitOfWork<Guardian> guardianContext,

            IWebHostEnvironment hostEnvironment,

            ILogger<ApplicationsController> logger,

            IHelperService helperService,

            INotyfService notify)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _rejContext = rejContext ?? throw new ArgumentNullException(nameof(rejContext));

            _courseContext = courseContext ?? throw new ArgumentNullException(nameof(courseContext));

            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

            _guardianContext = guardianContext ?? throw new ArgumentNullException(nameof(guardianContext));

            _notify = notify ?? throw new ArgumentNullException(nameof(notify));

            _logger = logger;

            _helperService = helperService;
        }

        /// <summary>
        /// Processes the application submission.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>

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

        /// <summary>
        /// Processes the application submission.
        /// </summary>
        /// <param name="model">The application data transfer object.</param>
        /// <returns>A task representing the asynchronous operation.</returns>

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


        /// <summary>
        /// Handles the retrieval and presentation of application data.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing an <see cref="IActionResult"/> with the application data.</returns>

        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Applications()
        {
            try
            {
                var list = await _context.OnLoadItemsAsync();

                List<ApplicationsDTO> applications = new();

                foreach (var item in list)
                {
                    var dto = await ConvertToApplicationsDTOAsync(item);

                    applications.Add(dto);
                }

                ViewData["New"] = await NewApplicationCountAsync();

                ViewData["Pending"] = await PendingApplicationCountAsync();

                return View(applications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing applications.");

                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Handles the application process for a specific application ID.
        /// </summary>
        /// <param name="ApplicationId">The ID of the application to process.</param>
        /// <returns>A task representing the asynchronous operation, containing an <see cref="IActionResult"/>.</returns>
        public async Task<IActionResult> OnApply(Guid ApplicationId)
        {
            try
            {
                var application = await _context.OnLoadItemAsync(ApplicationId);

                if (application is null)
                {
                    _logger.LogWarning("Application with ID {ApplicationId} not found.", ApplicationId);

                    return RedirectToAction("RouteNotFound", "Global");
                }

                var applyDto = ConvertToApplyDTO(application);

                ViewData["Course"] = await ConvertCourseIdToStringAsync(application.CourseId);

                ViewBag.Application = application;

                return View(applyDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing the application with ID {ApplicationId}.", ApplicationId);

                return StatusCode(500, "Internal server error");
            }
        }



        /// <summary>
        /// Updates an existing application with the provided application data transfer object.
        /// </summary>
        /// <param name="model">The application data transfer object containing the updated application details.</param>
        /// <returns>A task representing the asynchronous operation.</returns>

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnApply(ApplyDTO model)
        {
            var application = await _context.OnLoadItemAsync(model.ApplicantId);

            if (application == null)
            {
                TempData["error"] = "Application not found.";

                return View(model);
            }

            var guardian = await LoadGuardianAsync(application.ApplicationId);

            if (guardian == null)
            {
                TempData["error"] = "Guardian not found.";

                return View(model);
            }

            UpdateModelWithGuardianDetails(model, guardian);

            var updatedApplication = CreateUpdatedApplication(application, model);

            if (model.Status == Enums.ApplicationStatus.Rejected)
            {
                if (!await IsRejectionFormSubmittedAsync(model.ApplicantId))
                {
                    TempData["error"] = "Rejection form not submitted. Please provide reasons for rejection.";

                    return View(model);
                }
            }

            var modifyApp = await _context.OnModifyItemAsync(updatedApplication);

            if (modifyApp != null)
            {
                TempData["success"] = "Application saved successfully.";

                return RedirectToAction(nameof(Applications));
            }

            TempData["error"] = "Error: Unable to save application.";

            return View(model);
        }

        /// <summary>
        /// Rejects an application based on the provided rejection data transfer object.
        /// </summary>
        /// <param name="model">The rejection data transfer object containing the details of the rejection.</param>
        /// <returns>A task representing the asynchronous operation.</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnRejectApplication(RejectionDTO model)
        {
            if (!_context.DoesEntityExist<Application>(a => a.ApplicationId == model.ApplicationId))
            {
                return NotFound();
            }


            if (_rejContext.DoesEntityExist<ApplicationRejection>(a => a.ApplicationId == model.ApplicationId))
            {
                TempData["error"] = "Application Already Rejected!!";

                return RedirectToAction(nameof(OnApply), new { ApplicationId = model.ApplicationId });


            }
            var application = await _context.OnLoadItemAsync(model.ApplicationId);

            if (application == null)
            {
                return NotFound();
            }

            var applicationRejection = CreateApplicationRejection(model);

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Validation error.";

                return RedirectToAction(nameof(OnApply), new { ApplicationId = model.ApplicationId });
            }

            var response = await SaveApplicationRejectionAsync(applicationRejection);

            TempData["success"] = response > 0 ? "Application successfully rejected." : "Error: Application rejection failed.";

            return RedirectToAction(nameof(OnApply), new { ApplicationId = model.ApplicationId});
        }



        /// <summary>
        /// Downloads a file from the specified destination folder.
        /// </summary>
        /// <param name="fileName">The name of the file to download.</param>
        /// <param name="destinationFolder">The folder where the file is located.</param>
        /// <returns>A file result for downloading the file.</returns>

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


        /// <summary>
        /// Builds a notification message for the application.
        /// </summary>
        /// <param name="model">The application model.</param>
        /// <param name="programme">The name of the program applied for.</param>
        /// <returns>The notification message.</returns>
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


        /// <summary>
        /// Downloads an attachment file asynchronously.
        /// </summary>
        /// <param name="filename">The name of the file to download.</param>
        /// <returns>A file result for downloading the attachment.</returns>
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


        /// <summary>
        /// Uploads the qualification document for an application asynchronously.
        /// </summary>
        /// <param name="attachment">The application containing the qualification document.</param>
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


        /// <summary>
        /// Uploads the identification document for an application asynchronously.
        /// </summary>
        /// <param name="attachment">The application containing the identification document.</param>
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


        /// <summary>
        /// Downloads a qualification document file asynchronously.
        /// </summary>
        /// <param name="filename">The name of the file to download.</param>
        /// <returns>A file result for downloading the qualification document.</returns>
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


        /// <summary>
        /// Uploads the residence document for an application asynchronously.
        /// </summary>
        /// <param name="attachment">The application containing the residence document.</param>
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

        /// <summary>
        /// Downloads a residence document file asynchronously.
        /// </summary>
        /// <param name="filename">The name of the file to download.</param>
        /// <returns>A file result for downloading the residence document.</returns>
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

        /// <summary>
        /// Downloads a file from the specified folder.
        /// </summary>
        /// <param name="filename">The name of the file to download.</param>
        /// <param name="folder">The folder where the file is located.</param>
        /// <returns>A file result for downloading the file.</returns>
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

        /// <summary>
        /// Notifies the applicant via SMS or email.
        /// </summary>
        /// <param name="UserEmail">The email of the user.</param>
        /// <param name="UserPhone">The phone number of the user.</param>
        /// <param name="Message">The message to be sent.</param>
        /// <param name="IsSMS">Whether to send an SMS.</param>
        /// <param name="IsEmail">Whether to send an email.</param>
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

        #region Private Helpers

        /// <summary>
        /// Checks if a rejection form has been submitted for the specified applicantID asynchronously.
        /// </summary>
        /// <param name="applicantId">The applicant ID.</param>
        /// <returns>True if a rejection form exists; otherwise, false.</returns>
        private async Task<bool> IsRejectionFormSubmittedAsync(Guid applicantId)
        {
            return await Task.Run(() => _rejContext.DoesEntityExist<ApplicationRejection>(r => r.ApplicationId == applicantId));
        }


        /// <summary>
        /// Creates an updated application entity from the existing application and the application data transfer object.
        /// </summary>
        /// <param name="application">The existing application entity.</param>
        /// <param name="model">The application data transfer object.</param>
        /// <returns>The updated application entity.</returns>
        private Application CreateUpdatedApplication(Application application, ApplyDTO model)
        {
            return new Application
            {
                ApplicationId = application.ApplicationId,

                ReferenceNumber = model.ReferenceNumber,

                CourseId = application.CourseId,

                HighestQualDoc = model.HighestQualDoc,

                HighestQualification = model.HighestQualification,

                ApplicantTitle = model.ApplicantTitle,

                ApplicantName = model.ApplicantName,

                ApplicantSurname = model.ApplicantSurname,

                Cellphone = model.Cellphone,

                Email = model.Email,

                Gender = model.Gender,

                IDNumber = model.IDNumber,

                PassportNumber = model.PassportNumber,

                Selection = model.Selection,

                Status = model.Status,

                StudyPermitCategory = model.StudyPermitCategory,

                IDPassDoc = model.IDPassDoc
            };
        }


        /// <summary>
        /// Updates the application data transfer object with the guardian details.
        /// </summary>
        /// <param name="model">The application data transfer object.</param>
        /// <param name="guardian">The guardian details.</param>
        private void UpdateModelWithGuardianDetails(ApplyDTO model, Guardian guardian)
        {
            model.GuardianFirstName = guardian.FirstName;

            model.GuardianLastName = guardian.LastName;

            model.GuardianCellphone = guardian.Cellphone;

            model.GuardianRelationship = guardian.Relationship;
        }


        /// <summary>
        /// Loads the guardian details associated with the specified applicationID asynchronously.
        /// </summary>
        /// <param name="applicationId">The application ID.</param>
        /// <returns>The guardian details.</returns>
        private async Task<Guardian> LoadGuardianAsync(Guid applicationId)
        {
            var guardians = await _guardianContext.OnLoadItemsAsync();

            return guardians.FirstOrDefault(m => m.ApplicationId == applicationId);
        }


        /// <summary>
        /// Saves the provided ApplicationRejection instance to the database asynchronously.
        /// </summary>
        /// <param name="applicationRejection">The application rejection instance to save.</param>
        /// <returns>The number of state entries written to the database.</returns>
        private async Task<int> SaveApplicationRejectionAsync(ApplicationRejection applicationRejection)
        {
            await _rejContext.OnItemCreationAsync(applicationRejection);

            return await _rejContext.ItemSaveAsync();
        }


        /// <summary>
        /// Creates an ApplicationRejection instance from the provided RejectionDTO model.
        /// </summary>
        /// <param name="model">The rejection data transfer object.</param>
        /// <returns>The created ApplicationRejection instance.</returns>
        private ApplicationRejection CreateApplicationRejection(RejectionDTO model)
        {
            return new ApplicationRejection
            {
                FollowUpDate = model.FollowUpDate,
                AdditionalComments = model.AdditionalComments,
                NextSteps = model.NextSteps,
                ApplicationId = model.ApplicationId,
                CreatedBy = "Itu",
                CreatedOn = _helperService.GetCurrentDateTime(),
                Id = _helperService.GenerateGuid(),
                IsFinal = model.IsFinal,
                IsActive = true,
                Reason = model.Reason
            };
        }

        /// <summary>
        /// Creates an <see cref="Application"/> entity from the given <see cref="ApplyDTO"/> model.
        /// </summary>
        /// <param name="model">The model containing the application data.</param>
        /// <returns>The created <see cref="Application"/> entity.</returns>
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

        /// <summary>
        /// Creates an <see cref="Address"/> entity from the given <see cref="ApplyDTO"/> model and application ID.
        /// </summary>
        /// <param name="model">The model containing the address data.</param>
        /// <param name="applicationId">The ID of the application.</param>
        /// <returns>The created <see cref="Address"/> entity.</returns>
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

        /// <summary>
        /// Creates a <see cref="Guardian"/> entity from the given <see cref="ApplyDTO"/> model and application ID.
        /// </summary>
        /// <param name="model">The model containing the guardian data.</param>
        /// <param name="applicationId">The ID of the application.</param>
        /// <returns>The created <see cref="Guardian"/> entity.</returns>
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


        /// <summary>
        /// Asynchronously saves the given <see cref="Application"/> entity to the database.
        /// </summary>
        /// <param name="application">The application entity to save.</param>
        /// <returns>A task representing the asynchronous operation, containing the number of state entries written to the database.</returns>
        private async Task<int> SaveApplicationAsync(Application application)
        {
            var response = await _context.OnItemCreationAsync(application);

            if (response != null)
            {
                return await _context.ItemSaveAsync();
            }
            return 0;
        }

        /// <summary>
        /// Asynchronously uploads a file to the specified destination folder and updates the corresponding property of the application entity.
        /// </summary>
        /// <param name="application">The application entity to update.</param>
        /// <param name="file">The file to upload.</param>
        /// <param name="destinationFolder">The folder to upload the file to.</param>
        /// <param name="propertyName">The name of the property to update with the file path.</param>
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


        /// <summary>
        /// Asynchronously uploads a file if it is not null.
        /// </summary>
        /// <param name="application">The application entity to update.</param>
        /// <param name="file">The file to upload.</param>
        /// <param name="directory">The directory to upload the file to.</param>
        /// <param name="fileName">The name of the property to update with the file path.</param>
        private async Task UploadFileIfNotNullAsync(Application application, IFormFile file, string directory, string fileName)
        {
            if (file != null)
            {
                await FileUploader(application, file, directory, fileName);
            }
        }

        /// <summary>
        /// Sends an SMS notification about a new application.
        /// </summary>
        /// <param name="model">The application model containing the data.</param>
        private void OnSendSMS(Application model)
        {

            Helper.SendSMS("Dear Sir - a new application needing your attention" +
                            $" was just recieved - ref: {model.ReferenceNumber}", "0661401781");
        }

        /// <summary>
        /// Asynchronously sends an email notification about a new application.
        /// </summary>
        /// <param name="model">The application model containing the data.</param>
        private async void OnSendEmail(Application model)
        {
            string programme = await ConvertCourseIdToStringAsync(model.CourseId);

            _helperService.SendMailNotification(model.Email, "Application Acknowledgement",
            _helperService.OnSendMessage(model.ApplicantName, programme, model.ReferenceNumber), "Online Applications");

            _helperService.SendMailNotification("tmanyimo@forekinstitute.co.za", "New Application Alert",
            _helperService.OnSendMailToAdmin(model.ApplicantName, programme, model.ReferenceNumber), "Online Applications");

        }

        /// <summary>
        /// Converts the date string from to a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="dateString">The date string to be converted.</param>
        /// <returns>The converted <see cref="DateTime"/> object.</returns>
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

        /// <summary>
        /// Asynchronously gets the count of new applications with a status of "Submitted".
        /// </summary>
        /// <returns>A task representing the asynchronous operation, with the count of new applications with a status of "Submitted".</returns>
        private async Task<int> NewApplicationCountAsync()
        {
            var applications = await _context.OnLoadItemsAsync();

            return applications.Count(a => a.Status == Enums.ApplicationStatus.Submitted);
        }

        /// <summary>
        /// Asynchronously gets the count of pending applications with a status of "Pending".
        /// </summary>
        /// <returns>A task representing the asynchronous operation, with the count of pending applications with a status of "Pending".</returns>
        private async Task<int> PendingApplicationCountAsync()
        {
            var applications = await _context.OnLoadItemsAsync();

            return applications.Count(a => a.Status == Enums.ApplicationStatus.Pending);
        }

        /// <summary>
        /// Converts an <see cref="Application"/> entity to an <see cref="ApplicationsDTO"/>.
        /// </summary>
        /// <param name="item">The application entity to convert.</param>
        /// <returns>A task representing the asynchronous operation, containing the converted <see cref="ApplicationsDTO"/>.</returns>
        private async Task<ApplicationsDTO> ConvertToApplicationsDTOAsync(Application item)
        {
            return new ApplicationsDTO
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

                IDPassDoc = item.IDPassDoc ?? "Id Error"
                ,
                QualificationDoc = item.HighestQualDoc ?? "Qualification Error"
            };
        }

        /// <summary>
        /// Converts an <see cref="Application"/> entity to an <see cref="ApplyDTO"/>.
        /// </summary>
        /// <param name="application">The application entity to convert.</param>
        /// <returns>The converted <see cref="ApplyDTO"/>.</returns>
        private ApplyDTO ConvertToApplyDTO(Application application)
        {
            return new ApplyDTO
            {
                ApplicantId = application.ApplicationId,

                ApplicantIDPassFile = application.IDPassFile,

                ApplicantName = application.ApplicantName,

                ApplicantSurname = application.ApplicantSurname,

                ApplicantTitle = application.ApplicantTitle,

                Cellphone = application.Cellphone,

                Email = application.Email,

                Gender = application.Gender,

                CourseId = application.CourseId,

                IDNumber = application.IDNumber,

                IDPassDoc = application.IDPassDoc,

                Status = application.Status,

                Selection = application.Selection,

                ReferenceNumber = application.ReferenceNumber,

                StudyPermitCategory = application.StudyPermitCategory,

                HighestQualification = application.HighestQualification,

                HighestQualDoc = application.HighestQualDoc,

                ResidenceDoc = application.ResidenceDoc
            };
        }

        /// <summary>
        /// Converts a courseID to a meaningful string representation asynchronously.
        /// </summary>
        /// <param name="courseId">The courseID to convert.</param>
        /// <returns>A task representing the asynchronous operation, containing the course name.</returns>
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

        #endregion


    }
}
