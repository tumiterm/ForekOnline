using ElecPOE.Contract;
using ElecPOE.DTO;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NuGet.Packaging.Licenses;
using System.ComponentModel;

namespace ElecPOE.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LicenseController : Controller
    {

        private readonly IUnitOfWork<ElecPOE.Models.License> _context;
        private readonly IUnitOfWork<Course> _courseContext;
        private readonly IUnitOfWork<Company> _companyContext;
        private readonly ILogger<LicenseController> _logger;

        private IWebHostEnvironment _hostEnvironment;
        public LicenseController(IUnitOfWork<Models.License> context, IWebHostEnvironment hostEnvironment,
                                IUnitOfWork<Course> courseContext, IUnitOfWork<Company> companyContext,
                                ILogger<LicenseController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

            _courseContext = courseContext ?? throw new ArgumentNullException(nameof(courseContext));

            _companyContext = companyContext ?? throw new ArgumentNullException(nameof(companyContext));

            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> AddUserLicense()
        {
            var companyList = await _companyContext.OnLoadItemsAsync();

            var courseList = await _courseContext.OnLoadItemsAsync();

            IEnumerable<SelectListItem> courses = courseList.Select(l => new SelectListItem
            {
                Value = l.CourseId.ToString(),

                Text = $"({l.CourseName}) - ({l.Type})",

            });

            IEnumerable<SelectListItem> companies = companyList
                                       .OrderBy(l => l.CompanyName)
                                       .Select(l => new SelectListItem
                                       {
                                           Value = l.CompanyId.ToString(),

                                           Text = $"{l.CompanyName}",

                                       });

            ViewBag.CompanyId = new SelectList(companies, "Value", "Text");

            ViewBag.CourseId = new SelectList(courses, "Value", "Text");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserLicense(LicenseDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Models.License license = new()
            {
                DateOfExpiry = model.DateOfExpiry,

                ClientType = model.ClientType,

                Title = model.Title,

                CourseKey = model.CourseKey,

                CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}",

                DateOfIssue = model.DateOfIssue,

                CreatedOn = Helper.OnGetCurrentDateTime(),

                FileUpload = model.FileUpload,

                IDNumber = model.IDNumber,

                Frequency = model.Frequency,

                IsActive = true,

                LastName = model.LastName,

                LicenseId = Helper.GenerateGuid(),

                Name = model.Name,

                CompanyId = model.CompanyId,

                IFormFileUpload = model.IFormFileUpload,

            };

            bool isUploaded = await AttachmentUploader(license);

            if (isUploaded)
            {
                var licenseObj = await _context.OnItemCreationAsync(license);

                int rc = licenseObj != null ? await _context.ItemSaveAsync() : 0;

                if (rc > 0)
                {
                    TempData["success"] = "License saved successfully";

                    return RedirectToAction(nameof(AddUserLicense));
                }
                else
                {
                    TempData["error"] = "License NOT saved!!!";

                    return View(license);
                }
            }

            return View();

        }

        public async Task<IActionResult> Licenses()
        {
            var licenses = await _context.OnLoadItemsAsync();

            var licenseList = licenses.Select(async lic => new LicenseDTO
            {
                DateOfExpiry = lic.DateOfExpiry,

                DateOfIssue = lic.DateOfIssue,

                IDNumber = lic.IDNumber,

                ClientType = lic.ClientType,

                Company = await OnGetCompanyNameAsync(lic.CompanyId),

                LicenseName = await OnGetCourseNameAsync(lic.CourseKey),

                CompanyId = lic.CompanyId,

                CourseKey = lic.CourseKey,

                FileUpload = lic.FileUpload,

                Frequency = lic.Frequency,

                LastName = lic.LastName,

                LicenseId = lic.LicenseId,

                Name = lic.Name,

                Title = lic.Title,

            }).ToList();

            var liceList = await Task.WhenAll(licenseList);

            return View(liceList);
        }

        [HttpGet]
        public async Task<IActionResult> LicenseInfo(Guid licenseId)
        {
            if (licenseId == Guid.Empty)
            {
                return NotFound();
            }

            try
            {
                Models.License license = await _context.OnLoadItemAsync(licenseId);

                if (license == null)
                {
                    return NotFound();
                }

                var viewModel = new LicenseDTO
                {
                    LastName = license.LastName,

                    Name = license.Name,

                    ClientType = license.ClientType,

                    Company = await OnGetCompanyNameAsync(license.CompanyId),

                    CompanyId = license.CompanyId,

                    CourseKey = license.CourseKey,

                    DateOfExpiry = license.DateOfExpiry,

                    DateOfIssue = license.DateOfIssue,

                    LicenseId = license.LicenseId,

                    FileUpload = license.FileUpload,

                    Frequency = license.Frequency,

                    IDNumber = license.IDNumber,

                    LicenseName = await OnGetCourseNameAsync(license.CourseKey),

                    Title = license.Title
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching license information: {ex.Message}");

                return StatusCode(500, "Internal Server Error");
            }
        }


        //public async Task<IActionResult> LicenseInfo(Guid LicenseId)
        //{

        //}

        public IActionResult LicenseDownload(string filename, string folder)
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

        public async Task<string> OnGetCompanyNameAsync(Guid companyId)
        {
            try
            {
                var company = await _companyContext.OnLoadItemAsync(companyId);

                return !string.IsNullOrEmpty(company?.CompanyName) ? company.CompanyName : "Company Loading...";

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");

                return "Error retrieving company";
            }
        }

        public async Task<string> OnGetCourseNameAsync(Guid courseId)
        {
            try
            {
                var course = await _courseContext.OnLoadItemAsync(courseId);

                return course != null ? $"{course.CourseName} - ({course.Type.ToString()})" : "Course not found";

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");

                return "Error retrieving course";
            }

        }

        public async Task<bool> AttachmentUploader(Models.License license)
        {
            if (license.IFormFileUpload != null && license.IFormFileUpload.Length > 0)
            {
                try
                {

                    string wwwRootPath = _hostEnvironment.WebRootPath;

                    string fileName = Path.GetFileNameWithoutExtension(license.IFormFileUpload.FileName);

                    string extension = Path.GetExtension(license.IFormFileUpload.FileName);

                    string path = Path.Combine(wwwRootPath + "/Licenses/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await license.IFormFileUpload.CopyToAsync(fileStream);
                    }

                    license.FileUpload = fileName = fileName + Helper.GenerateGuid() + extension;

                    return true;

                }
                catch (Exception ex)
                {
                    return false;
                }

            }

            return false;
        }


    }
}
