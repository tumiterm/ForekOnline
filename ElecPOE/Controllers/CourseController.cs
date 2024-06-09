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
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class CourseController : Controller
    {
        private readonly IUnitOfWork<Course> _context;

        private readonly IUnitOfWork<Models.Module> _modContext;

        private IWebHostEnvironment _hostEnvironment;
        public INotyfService _notify { get; }
        public CourseController(IUnitOfWork<Course> context,
                                IUnitOfWork<Models.Module> modContext,
                                IWebHostEnvironment hostEnvironment,
                                INotyfService notify)
        {
            _context = context;

            _modContext = modContext;

            _hostEnvironment = hostEnvironment;

            _notify = notify;

        }
        [HttpGet]
        public IActionResult Course()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Course(CourseModuleDTO course)
        {
            Guid globalGuid = Helper.GenerateGuid();    

            Course courseObj = new Course
            {
                IsActive = true,

                CourseId = globalGuid,

                NType = course.NType,

                CourseName = course.Name,

                NQFLevel = course.CourseNQFLevel,

                CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}",

                CreatedOn = Helper.OnGetCurrentDateTime(),

                Type = course.Type,

                Credit = course.CourseCredit,

                Module = new List<Models.Module>
                {
                    new Models.Module
                    {
                        CourseIdFK = globalGuid,

                        Credit = course.Credit,

                        IsActive = true,

                        ModuleId = Helper.GenerateGuid(),

                        ModuleName = course.ModuleName,

                        NQFLevel = course.NQFLevel,

                    }

                }.ToList()
            };

            if (courseObj != null)
            {
                if (ModelState.IsValid)
                {
                    Course courseAdd = await _context.OnItemCreationAsync(courseObj); 
                    
                    if(courseAdd != null) 
                    {
                        int rc = await _context.ItemSaveAsync();

                        if (rc > 0)
                        {
                            TempData["success"] = "Course Added successfully";

                            return RedirectToAction(nameof(OnCourse), new { CourseId = courseAdd.CourseId});
                        }
                        else
                        {
                            TempData["error"] = "Error: Unable to save course";
                        }
                    }
                }
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> OnCourse(Guid CourseId, Guid ModuleId)
        {
            if(CourseId == Guid.Empty) 
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            Course course = await _context.OnLoadItemAsync(CourseId);

            CourseModuleDTO courseModule = null;

            List<Models.Module> moduleList = new List<Models.Module>();    

            if(course != null)
            {
                if(course.Type == Enums.eCourseType.Nated)
                {
                    ViewData["CourseName"] = $"{course.Type}: ({course.CourseName} {course.NType})";
                }
                else
                {
                    ViewData["CourseName"] = $"{course.Type}: ({course.CourseName})";
                }

                ViewData["Credit"] = course.Credit;
                
                ViewData["Type"] = course.Type;

                ViewData["NQFLevel"] = course.NQFLevel;

                courseModule = new CourseModuleDTO
                {
                    CourseCredit = course.Credit,

                    CourseId = CourseId,

                    Type = course.Type,
                     
                    Name = course.CourseName,
                };

                ViewData["CourseIdFK"] = CourseId;

                moduleList = await OnGetCourseModules(CourseId);
            }
            return View(moduleList);
        }

        [HttpGet]
        public async Task<IActionResult> OnGetCourse()
        {
            var courseList = await _context.OnLoadItemsAsync();

            List<CourseDTO> courses = new List<CourseDTO>();

            CourseDTO course = null;

            foreach (var item in courseList)
            {
                course = new CourseDTO
                {
                    CourseId = item.CourseId,

                    CourseName = item.CourseName,

                    Credit = item.Credit,

                    CreatedBy = item.CreatedBy,

                    CreatedOn = item.CreatedOn,

                    IsActive = item.IsActive,

                    ModifiedBy = item.ModifiedBy,

                    ModifiedOn = item.ModifiedOn,

                    NQFLevel = item.NQFLevel != null ? Helper.GetDisplayName(item.NQFLevel) : null,

                    Type = item.Type != null ? Helper.GetDisplayName(item.Type) : null,

                    NType = item.NType != null ? Helper.GetDisplayName(item.NType) : null
                };

                courses.Add(course);
            }

            return View(courses.Where(m => m.IsActive).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnCourse(Course course)
        {
            return View();
        }
        private async Task<List<Models.Module>> OnGetCourseModules(Guid CourseId)
        {
            if(CourseId == Guid.Empty)
            {
                 RedirectToAction("RouteNotFound", "Global");
            }

            List<Models.Module> mods = new List<Models.Module>(); 

            var modules = await _modContext.OnLoadItemsAsync();  

            var filterMods = from n in modules

                             where n.CourseIdFK== CourseId

                             && n.ModuleName != null

                             && n.ModuleName != string.Empty

                             select n;

            mods = filterMods.ToList(); 

            return mods;
        }

        [HttpGet]
        public async Task<IActionResult> OnModifyCourse(Guid CourseId)
        {
            if(CourseId == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");

            }

            Course course = await _context.OnLoadItemAsync(CourseId);
          
            CourseModuleDTO courseModule = null;
           
            if (course != null)
            {
                courseModule = new CourseModuleDTO
                {
                    Name = course.CourseName,

                    Type = course.Type,

                    CourseNQFLevel = course.NQFLevel,

                    CourseCredit = course.Credit,

                    IsActive = course.IsActive,

                    ModifiedOn = course.ModifiedOn,

                    ModifiedBy = course.ModifiedBy,

                    
                };
            }
            return View(courseModule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnModifyCourse(CourseModuleDTO model)
        {
            Course course = null;   

            if(ModelState.IsValid)
            {
                course = new Course
                {
                    CourseId = model.CourseId,

                    CourseName = model.Name,

                    Type = model.Type,

                    Credit = model.CourseCredit,

                    NQFLevel = model.CourseNQFLevel,

                    IsActive = model.IsActive,

                    ModifiedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}",

                    ModifiedOn = Helper.OnGetCurrentDateTime(),

                };

                if(course!= null)
                {
                    Course courseObj = await _context.OnModifyItemAsync(course);

                    if(courseObj != null)
                    {
                        TempData["success"] = "Course saved successful";

                        return RedirectToAction("OnGetCourse","Course");
                    }
                    else
                    {
                        TempData["error"] = "Error: Unable to save course!";

                    }
                }
                else
                {
                    TempData["error"] = "Error: Something went wrong!!";
                }
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
        public async Task<IActionResult> RemoveModule(Guid ModuleId)
        {
            if (ModuleId == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            Module mod = await _modContext.OnLoadItemAsync(ModuleId);

            if (mod is null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            int del = await _modContext.OnRemoveItemAsync(ModuleId);

            if (del > 0)
            {
                return RedirectToAction("OnCourse", new { CourseId = mod.CourseIdFK });
            }

            return View();
        }

    }
}
