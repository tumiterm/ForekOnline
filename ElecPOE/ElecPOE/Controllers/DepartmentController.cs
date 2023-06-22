using Microsoft.AspNetCore.Mvc;

namespace ElecPOE.Controllers
{
    public class DepartmentController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }

        
    }
}
