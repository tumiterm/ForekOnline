using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.DTO;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RestSharp;

namespace ElecPOE.Controllers
{
    public class PlacementController : Controller
    {
        private readonly IUnitOfWork<Placement> _context;
        private readonly IUnitOfWork<User> _userContext;

        private readonly IUnitOfWork<Company> _companyContext;
        private readonly IUnitOfWork<Student> _studentContext;


        private IWebHostEnvironment _hostEnvironment;
        public INotyfService _notify { get; }
        public PlacementController(IUnitOfWork<Placement> context,
                                    IUnitOfWork<User> userContext,
                                    IUnitOfWork<Company> companyContext,
                                    IUnitOfWork<Student> studentContext,
                                    IWebHostEnvironment hostEnvironment,
                                    INotyfService notify)
        {
            _context = context;

            _hostEnvironment = hostEnvironment;

            _notify = notify;

            _userContext = userContext;

            _companyContext = companyContext;

            _studentContext = studentContext;

        }
        public async Task<IActionResult> LearnerPlacement(string StudentNumber)
        {
            Student getStudent = await GetStudent(StudentNumber);

            if (getStudent == null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            var users = await _userContext.OnLoadItemsAsync();
            var company = await _companyContext.OnLoadItemsAsync();
            var student = await _studentContext.OnLoadItemsAsync();

            IEnumerable<SelectListItem> userList = from s in users

                                                   select new SelectListItem
                                                   {
                                                       Value = s.Id.ToString(),

                                                       Text = $"{s.Name} {s.LastName}"
                                                   };

            IEnumerable<SelectListItem> companyList = from s in company

                                                      select new SelectListItem
                                                      {
                                                          Value = s.CompanyId.ToString(),

                                                          Text = $"{s.CompanyName}"
                                                      };

            ViewBag.UserId = new SelectList(userList, "Value", "Text");

            ViewBag.CompanyId = new SelectList(companyList, "Value", "Text");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LearnerPlacement(Placement placement)
        {
            return View();
        }
        public async Task<Student> GetStudent(string StudentNumber)
        {
            string res = string.Empty;

            Student student = new Student();

            var client = new RestClient($"http://forekapi.dreamline-ict.co.za/api/Student?StudentNumber={StudentNumber}");

            var request = new RestRequest();

            request.AddParameter("StudentNumber", StudentNumber);

            request.AddHeader("Authorization", $"Bearer {Helper.GenerateJWTToken()}");

            var response = await client.ExecuteGetAsync(request);

            if (response.IsSuccessful)
            {
                res = response.Content.ToString();

                student = JsonConvert.DeserializeObject<Student>(res);

                if (student.EnrollmentHistory.Count > 0)
                {
                    EnrollmentHistoryViewModel enrollment = new EnrollmentHistoryViewModel
                    {
                        CourseId = student.EnrollmentHistory[0].CourseId,

                        StartDate = student.EnrollmentHistory[0].StartDate,

                        CourseTitle = student.EnrollmentHistory[0].CourseTitle,

                        StudentId = student.EnrollmentHistory[0].StudentId,

                        CourseType = student.EnrollmentHistory[0].CourseType,

                        EnrollmentId = student.EnrollmentHistory[0].EnrollmentId,

                        EnrollmentStatus = student.EnrollmentHistory[0].EnrollmentStatus,

                        IsActive = student.EnrollmentHistory[0].IsActive

                    };
                }
                else
                {
                    _notify.Error("Error: Student not registered to ANY course!!!", 5);
                }
            }

            return student;
        }


    }
}
