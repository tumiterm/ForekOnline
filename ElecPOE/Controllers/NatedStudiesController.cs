using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ElecPOE.Controllers
{
    public class NatedStudiesController : Controller
    {

        private readonly IUnitOfWork<NatedEngineering> _context;
        private readonly IUnitOfWork<Course> _courseContext;
        private readonly IUnitOfWork<Module> _moduleContext;

        private IWebHostEnvironment _hostEnvironment;
        public INotyfService _notify { get; }
        public NatedStudiesController(IUnitOfWork<NatedEngineering> context,
                                    IUnitOfWork<Module> moduleContext,
                                    IUnitOfWork<Course> courseContext,
                                    IWebHostEnvironment hostEnvironment,
                                    INotyfService notify)
        {
            _context = context;

            _moduleContext = moduleContext ?? throw new ArgumentNullException(nameof(moduleContext));

            _courseContext = courseContext ?? throw new ArgumentNullException(nameof(courseContext));

            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

            _notify = notify ?? throw new ArgumentNullException(nameof(notify));

        }
        public IActionResult HomePage()
        {
            return View();
        }

        public async Task<IActionResult> EngineeringStudies()
        {
            List<Course> courses = await _courseContext.OnLoadItemsAsync();
            List<Module> modules = await _moduleContext.OnLoadItemsAsync();

            var filteredCourses = courses.Where(m => m.Type == Enums.eCourseType.Nated).ToList();

            IEnumerable<SelectListItem> getCourses = from n in filteredCourses

                                                     select new SelectListItem
                                                     {
                                                         Value = n.CourseId.ToString(),

                                                         Text = $"{n.CourseName} ({n.Type} {n.NType})"
                                                     };

            IEnumerable<SelectListItem> getModules = from n in modules

                                                     where n.IsActive == true

                                                     //   && n.CourseIdFK == courses.First().CourseId

                                                     && n.ModuleName != null

                                                     select new SelectListItem
                                                     {
                                                         Value = n.ModuleId.ToString(),

                                                         Text = $"{n.ModuleName}"
                                                     };


            ViewBag.CourseId = new SelectList(getCourses, "Value", "Text");

            ViewBag.ModuleId = new SelectList(getModules, "Value", "Text");

            return View();
        }

        public async Task<JsonResult> GetCourseModuleId(Guid CourseId)
        {
            var mods = await _moduleContext.OnLoadItemsAsync();

            var filterMods = from n in mods

                             where n.CourseIdFK == CourseId

                             select n;

            return Json(filterMods.ToList());
        }

    }
}
