using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.DTO;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ElecPOE.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
    public class VisitController : Controller
    {
        private readonly IUnitOfWork<Visit> _context;
        private readonly IUnitOfWork<Placement> _placementContext;
        private readonly IUnitOfWork<Company> _companyContext;
        private readonly IUnitOfWork<ContactPerson> _contactContext;
        private IWebHostEnvironment _hostEnvironment;
        public INotyfService _notify { get; }
        public VisitController(IUnitOfWork<Visit> context,
                                    IUnitOfWork<Placement> placementContext,
                                    IUnitOfWork<Company> companyContext,
                                    IUnitOfWork<ContactPerson> contactContext,
                                    IWebHostEnvironment hostEnvironment,
                                    INotyfService notify)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _companyContext = companyContext ?? throw new ArgumentNullException(nameof(companyContext));

            _placementContext = placementContext ?? throw new ArgumentNullException(nameof(placementContext));

            _contactContext = contactContext ?? throw new ArgumentNullException(nameof(contactContext));

            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

            _notify = notify ?? throw new ArgumentNullException(nameof(notify));

        }

        public async Task<ActionResult<VisitDTO>> Visitation(Guid CompanyId)
        {
            if (CompanyId == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            Company company = await _companyContext.OnLoadItemAsync(CompanyId);

            if (company == null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            var contact = await _contactContext.OnLoadItemsAsync();

            var contactInfo = contact.FirstOrDefault(m => m.AssociativeId == company.CompanyId);

            if (contactInfo == null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            VisitDTO visitDTO = null;

            visitDTO = new VisitDTO
            {
                Company = company.CompanyName,

                HasReport = false,

                Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),

                LearnerFeedback = "",

                Mentor = $"{contactInfo.Name} {contactInfo.LastName}",

                CompanyId = CompanyId,

            };

            ViewData["CompId"] = CompanyId;

            return View(visitDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Visitation(VisitDTO model)
        {
            Visit visit = new Visit
            {
                VisitId = Helper.GenerateGuid(),

                CreatedOn = Helper.OnGetCurrentDateTime(),

                CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}",

                Date = ParseDateTime(model.Date),

                CompanyId = model.CompanyId,

                HasReport = model.HasReport,

                IsActive = true,

                LearnerFeedback = model.LearnerFeedback, 

                Mentor = model.Mentor,

                Report = model.Report ,

                ReportFile = model.ReportFile,  

            };

            Company company = await _companyContext.OnLoadItemAsync(visit.CompanyId);

            if (visit != null)
            {
                if (ModelState.IsValid)
                {
                    var visitObj = await _context.OnItemCreationAsync(visit);

                    if (visitObj != null)
                    {
                        if (visit.HasReport)
                        {
                            await AttachmentUploader(visit);
                        }

                        int rc = await _context.ItemSaveAsync();

                        if (rc > 0)
                        {
                            TempData["success"] = "Visitation successfully captured";

                            return RedirectToAction(nameof(VisitationList));
                        }
                        else
                        {
                            TempData["error"] = "Error: Unable to save visitation details!!!";
                        }
                    }
                    else
                    {
                        TempData["error"] = "Error: Something went wrong!!!";
                    }
                }
                else
                {
                    _notify.Error("Error: Something went wrong!");
                }
            }
               

            return View();
        }

        public async Task<IActionResult> VisitationList()
        {
            var visitList = await _context.OnLoadItemsAsync();

            VisitationDTO visit = null;

            List<VisitationDTO> visitations = new List<VisitationDTO>();

            foreach(var item in visitList)
            {
                visit = new VisitationDTO
                {
                    Date = item.Date,

                    Company = OnGetCompany(item.CompanyId).Result.CompanyName,

                    HasReport = item.HasReport,

                    LearnerFeedback = item.LearnerFeedback,  

                    Mentor = item.Mentor,   

                    Report = item.Report,

                    ReportFile = item.ReportFile,   

                    VisitId = item.VisitId, 

                    VisitPurpose = item.VisitPurpose,   
                };

                visitations.Add(visit);
            }

            return View(visitations);
        }

        public async Task<IActionResult> OnModifyVisitation(Guid VisitId)
        {
            Visit visit = await _context.OnLoadItemAsync(VisitId);

            if(visit == null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            return View(visit);
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
        public async Task AttachmentUploader(Visit visit)
        {
            try
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                string fileName = Path.GetFileNameWithoutExtension(visit.ReportFile.FileName);

                string extension = Path.GetExtension(visit.ReportFile.FileName);

                visit.Report = fileName = fileName + Helper.GenerateGuid() + extension;

                string path = Path.Combine(wwwRootPath + "/Visitation/", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await visit.ReportFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";

            }
        }
        public static DateTime ParseDateTime(string input)
        {
            DateTime result;

            if (DateTime.TryParse(input, out result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException("Failed to parse the input string as DateTime.");
            }
        }

        public async Task<IActionResult> AttachmentDownload(string filename)
        {
            try
            {
                if (filename == null)

                    return Content("Sorry, no attachment found!!!");

                var path1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                string folder = Path.Combine(path1, @"Visitation", filename);

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

        public async Task<Company> OnGetCompany(Guid CompanyId)
        {
            return await _companyContext.OnLoadItemAsync(CompanyId);
        }


    }
}
