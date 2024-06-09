using ElecPOE.Data;
using ElecPOE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ElecPOE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, 
                              ApplicationDbContext context)
        {
            _logger = logger;

            _context = context;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> AdminPanel()
        {
            ViewData["ID"] = await _context.StudentAttachments.Where(m => m.DocumentName == Enums.eLearnerAdministration.LearnerID).CountAsync();
            ViewData["CV"] = await _context.StudentAttachments.Where(m => m.DocumentName == Enums.eLearnerAdministration.CV).CountAsync();
            ViewData["Matric"] = await _context.StudentAttachments.Where(m => m.DocumentName == Enums.eLearnerAdministration.Matric).CountAsync();
            ViewData["Guardian"] = await _context.StudentAttachments.Where(m => m.DocumentName == Enums.eLearnerAdministration.GuardianId).CountAsync();

            ViewData["Summative"] = await _context.AssessmentAttachments.Where(m => m.Type == Enums.eAssessmentAdministration.Summative).CountAsync();
            ViewData["Formative"] = await _context.AssessmentAttachments.Where(m => m.Type == Enums.eAssessmentAdministration.Formative).CountAsync();
            ViewData["Practical"] = await _context.AssessmentAttachments.Where(m => m.Type == Enums.eAssessmentAdministration.Practical).CountAsync();
            ViewData["IISA"] = await _context.AssessmentAttachments.Where(m => m.Type == Enums.eAssessmentAdministration.IISA).CountAsync();


            ViewData["Welder"] = await _context.Results.Where(m => m.Course == Enums.eTrade.Welder).CountAsync();
            ViewData["Plumber"] = await _context.Results.Where(m => m.Course == Enums.eTrade.Plumber).CountAsync();
            ViewData["Electrician"] = await _context.Results.Where(m => m.Course == Enums.eTrade.Electrician).CountAsync();
            ViewData["NATED"] = await _context.Results.Where(m => m.Course == Enums.eTrade.NATED).CountAsync();

            ViewData["EnrollmentForm"] = await _context.Training.Where(m => m.Type == Enums.eTrainingAdministration.EnrollmentForm).CountAsync();
            ViewData["InductionForm"] = await _context.Training.Where(m => m.Type == Enums.eTrainingAdministration.InductionForm).CountAsync();
            ViewData["Declaration"] = await _context.Training.Where(m => m.Type == Enums.eTrainingAdministration.Declaration).CountAsync();
            ViewData["Conduct"] = await _context.Training.Where(m => m.Type == Enums.eTrainingAdministration.Conduct).CountAsync();

            ViewData["CurrentUser"] = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}";
            ViewData["role"] = OnGetCurrentUser().Role.ToString();

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private User OnGetCurrentUser()
        {
            return JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("SessionUser"));
        }

    }
}