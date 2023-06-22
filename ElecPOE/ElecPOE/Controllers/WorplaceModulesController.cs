using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.DTO;
using ElecPOE.Enums;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Reflection;
using Module = ElecPOE.Models.Module;

namespace ElecPOE.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]

    public class WorplaceModulesController : Controller
    {
        private readonly IUnitOfWork<LearnerWorkplaceModules> _context;
        private readonly IUnitOfWork<Course> _courseContext;
        private readonly IUnitOfWork<Module> _modContext;
        private readonly IUnitOfWork<Placement> _placementContext;
        private readonly IUnitOfWork<Company> _companyContext;


        private IWebHostEnvironment _hostEnvironment;
        public INotyfService _notify { get; }
        public WorplaceModulesController(IUnitOfWork<LearnerWorkplaceModules> context,
                                    IUnitOfWork<Course> courseContext,
                                    IUnitOfWork<Module> modContext,
                                    IUnitOfWork<Placement> placementContext,
                                    IWebHostEnvironment hostEnvironment,
                                    IUnitOfWork<Company> companyContext,
                                    INotyfService notify)
        {
            _context = context;

            _courseContext = courseContext ?? throw new ArgumentNullException(nameof(courseContext));

            _modContext = modContext ?? throw new ArgumentNullException(nameof(modContext));

            _placementContext = placementContext ?? throw new ArgumentNullException(nameof(placementContext));

            _companyContext = companyContext ?? throw new ArgumentNullException(nameof(companyContext));

            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

            _notify = notify ?? throw new ArgumentNullException(nameof(notify));

        }

        public async Task<IActionResult> WorkplaceModules(Guid PlacementId)
        {
            Placement placement = await _placementContext.OnLoadItemAsync(PlacementId);

            List<WorkplaceModulesDTO> moduleList = new List<WorkplaceModulesDTO>();

            WorkplaceModulesDTO dto = null;

            var lwm = await _context.OnLoadItemsAsync();

            var lwmList = lwm.Where(n => n.PlacementId == PlacementId && n.IsActive == true).ToList();

            foreach (var item in lwmList)
            {
                dto = new WorkplaceModulesDTO
                {
                    LearnerWorkplaceModulesId = item.LearnerWorkplaceModulesId,

                    Days = item.Days,

                    Progress = item.Progress,

                    Student = item.Student,

                    Course = GetItemName(item.CourseId, eOperationType.Course),

                    Module = GetItemName(item.ModuleId, eOperationType.Module),

                    RegisteredBy = item.CreatedBy,

                    IsActive = item.IsActive,

                    PlacementId = item.PlacementId,

                    StartDate = item?.StartDate.Value.ToShortDateString(),

                    EndDate= item?.EndDate.Value.ToShortDateString(),  

                    Company = GetItemName(placement.CompanyId, eOperationType.Company),
                };

                ViewData["Stud"] = item.Student;

                ViewData["Course"] = $"{dto.Course}";

                ViewData["PlacementId"] = PlacementId;

                ViewData["Comp"] = dto.Company;

                moduleList.Add(dto);
            }

            return View(moduleList);
        }

        [HttpGet]
        public async Task<IActionResult> RegisterModule(Guid PlacementId)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterModule(LearnerWorkplaceModules modules, Guid PlacementId)
        {
            if(PlacementId == Guid.Empty)
            {
                return NotFound();
            }

            Placement placement = await _placementContext.OnLoadItemAsync(PlacementId);

            modules.Student = placement.Student;

            modules.IsActive = true;

            modules.LearnerWorkplaceModulesId = Helper.GenerateGuid();

            modules.CreatedOn = Helper.OnGetCurrentDateTime();

            modules.CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}";

            if (ModelState.IsValid)
            {
                var wmObject = await _context.OnItemCreationAsync(modules);

                if(wmObject != null) 
                {
                    int rc = await _context.ItemSaveAsync();

                    if (rc > 0)
                    {
                        TempData["success"] = "Module added and saved successfully";

                        return RedirectToAction(nameof(WorkplaceModules), new { PlacementId = modules.PlacementId });
                    }
                    else
                    {
                        TempData["error"] = "Module NOT saved!!!";
                    }

                }
            }

            return View();
        }

        public async Task<IActionResult> OnViewModule(Guid LearnerWorkplaceModulesId, Guid PlacementId)
        {
            if(LearnerWorkplaceModulesId == Guid.Empty)
            {
                return NotFound();
            }

            var lwm = await _context.OnLoadItemAsync(LearnerWorkplaceModulesId);

            var placement = await _placementContext.OnLoadItemAsync(PlacementId);

            var courses = await _courseContext.OnLoadItemsAsync();

            var modules = await _modContext.OnLoadItemsAsync();

            WorkplaceModulesDTO dto = new WorkplaceModulesDTO
            {
                Course = GetItemName(lwm.CourseId, eOperationType.Course),

                Days = lwm.Days,

                IsActive = lwm.IsActive,

                Module = GetItemName(lwm.ModuleId, eOperationType.Module),

                PlacementId = PlacementId,

                Progress = lwm.Progress,

                RegisteredBy = lwm.CreatedBy,

                Student = lwm.Student,

                ModuleId = lwm.ModuleId,

                CourseId = lwm.CourseId, 

                LearnerWorkplaceModulesId = lwm.LearnerWorkplaceModulesId,

                Company = GetItemName(placement.CompanyId, eOperationType.Company),
            };

            IEnumerable<SelectListItem> courseList = courses.Select(s => new SelectListItem
            {
                Value = s.CourseId.ToString(),

                Text = s.CourseName
            });

            IEnumerable<SelectListItem> moduleList = modules.Select(s => new SelectListItem
            {
                Value = s.ModuleId.ToString(),

                Text = s.ModuleName
            });

            ViewBag.CourseId = new SelectList(courseList, "Value", "Text");

            ViewBag.ModuleId = new SelectList(moduleList, "Value", "Text");

            return View(dto); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnViewModule(WorkplaceModulesDTO mods)
        {

            LearnerWorkplaceModules lwm = new LearnerWorkplaceModules
            {
                LearnerWorkplaceModulesId = mods.LearnerWorkplaceModulesId,

                ModifiedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}",

                CourseId = ConvertStringToGuid(mods.Course),

                ModuleId = ConvertStringToGuid(mods.Module),

                Days = mods.Days,

                IsActive = mods.IsActive,

                ModifiedOn = Helper.OnGetCurrentDateTime(),

                Progress = mods.Progress,

                Student = mods.Student,

                PlacementId = mods.PlacementId,

                StartDate = ParseDateTime(mods.StartDate),

                EndDate = ParseDateTime(mods.EndDate),  

            };

            if (ModelState.IsValid)
            {
                var modObj = await _context.OnModifyItemAsync(lwm);

                if(modObj != null) 
                {
                    TempData["success"] = "Module changes successfully applied";

                    return RedirectToAction(nameof(WorkplaceModules), new { PlacementId = mods.PlacementId });
                }
                else
                {
                    TempData["error"] = "Error: Module changes NOT saved!!";
                }
            }
            else
            {
                TempData["error"] = "Error: Something went wrong!!";
            }
            return View();
        }
        public async Task<JsonResult> GetCourseModuleId(Guid CourseId)
        {

            var mods = await _modContext.OnLoadItemsAsync();

            var filterMods = from n in mods

                             where n.CourseIdFK == CourseId

                             select n;

            return Json(filterMods.ToList());
        }
        private string GetItemName(Guid itemId, eOperationType itemType)
        {
            string itemName = string.Empty;

            switch (itemType)
            {
                case eOperationType.Course:

                    itemName = $"{_courseContext.OnLoadItemAsync(itemId).Result.CourseName} {_courseContext.OnLoadItemAsync(itemId).Result.Type}";

                    break;

                case eOperationType.Module:

                    itemName = _modContext.OnLoadItemAsync(itemId).Result.ModuleName;

                    break;

                case eOperationType.Company:

                    itemName = _companyContext.OnLoadItemAsync(itemId).Result.CompanyName;

                    break;

                default:
                    break;
            }

            return itemName;
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
        private Guid ConvertStringToGuid(string inputString)
        {
            Guid guid;

            if (Guid.TryParse(inputString, out guid))
            {
                return guid;
            }
            else
            {
                throw new ArgumentException("Invalid GUID format.");
            }
        }
        public static DateTime ParseDateTime(string? input)
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
    }
}
