using Microsoft.AspNetCore.Mvc;

namespace ElecPOE.Controllers
{
    public class StudentReportController : Controller
    {
        [HttpGet]
        public IActionResult UploadReport(Guid StudentId)
        {
            return View();
        }
    }
}
