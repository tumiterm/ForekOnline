using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Mvc;

namespace ElecPOE.Controllers
{
    public class ModuleController : Controller
    {
        private readonly IUnitOfWork<Module> _context;

        private IWebHostEnvironment _hostEnvironment;
        public INotyfService _notify { get; }
        public ModuleController(IUnitOfWork<Module> context,
                                    IWebHostEnvironment hostEnvironment,
                                    INotyfService notify)
        {
            _context = context;

            _hostEnvironment = hostEnvironment;

            _notify = notify;

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCourseModule(Module module)
         {
            module.IsActive = true; 

            module.ModuleId = Helper.GenerateGuid();

            if(ModelState.IsValid)
            {
                Module mod = await _context.OnItemCreationAsync(module);

                if (mod != null) 
                {
                    int rc = await _context.ItemSaveAsync();

                    if(rc > 0)
                    {
                        TempData["success"] = "Module Added successfully";

                        return RedirectToAction("OnCourse", "Course", new { CourseId = mod.CourseIdFK });
                    }
                }
            }
            return View();
        }
    }
}
