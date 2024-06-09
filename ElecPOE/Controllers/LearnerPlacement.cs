using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.DTO;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Composition;
using System.Text;

namespace ElecPOE.Controllers
{
    public class LearnerPlacement : Controller
    {
        private readonly IUnitOfWork<Models.Placement> _context;
        private readonly IUnitOfWork<Company> _companyContext;
        private readonly IUnitOfWork<User> _userContext;
        private readonly IUnitOfWork<Course> _courseContext;
        private readonly IUnitOfWork<Module> _modulesContext;
        private IWebHostEnvironment _hostEnvironment;
        public INotyfService _notify { get; }
        public LearnerPlacement(IUnitOfWork<Models.Placement> context,
                                IUnitOfWork<Course> courseContent,
                                IUnitOfWork<Module> moduleContext,
                                IUnitOfWork<Company> companyContext,
                                IUnitOfWork<User> userContext,
                                IWebHostEnvironment hostEnvironment,
                                INotyfService notify)
        {
            _context = context;

            _companyContext = companyContext;

            _courseContext = courseContent;

            _modulesContext = moduleContext;

            _userContext = userContext;

            _hostEnvironment = hostEnvironment;

            _notify = notify;

        }

        public async Task<IActionResult> PlacementDetails(string StudentNumber)
        {
            if (string.IsNullOrEmpty(StudentNumber))
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            Student student = await GetStudent(StudentNumber);

            string name = $"{student.FirstName} {student.LastName}";

            Placement placement =  _context.OnLoadItemsAsync().Result.FirstOrDefault(m => m.Student.Equals(name));

            if(placement is null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            var company = await OnGetCompany(placement.CompanyId);

            ViewData["company"] = company;

            ViewData["student"] = student;

            ViewData["PlacementId"] = placement.PlacementId;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PlaceLearner(string StudentNumber)
        {
            Student getStudent = await GetStudent(StudentNumber);

            if (getStudent == null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            var users = await _userContext.OnLoadItemsAsync();

            var company = await _companyContext.OnLoadItemsAsync();

            var filterUsers = from n in users

                              where n.Role == Enums.eSysRole.Facilitator ||

                              n.Role == Enums.eSysRole.Admin

                              select n;

            List<User> fUserList = filterUsers.ToList();


            IEnumerable<SelectListItem> userList = from s in fUserList

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



            ViewData["StudInfo"] = $"{getStudent.FirstName} {getStudent.LastName}";

            ViewData["StudentId"] = StudentNumber;

            ViewBag.UserId = new SelectList(userList, "Value", "Text");

            ViewBag.CompanyId = new SelectList(companyList, "Value", "Text");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceLearner(Placement placement, string StudentId)
        {
            Student getStudent = await GetStudent(StudentId);

            if (getStudent == null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            if (!IsStudentPlaced(placement.CompanyId, $"{getStudent.FirstName} {getStudent.LastName}"))
            {
                placement.IsActive = true;

                placement.CreatedOn = Helper.OnGetCurrentDateTime();

                placement.CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}";

                placement.PlacementId = Helper.GenerateGuid();

                var place = await _context.OnItemCreationAsync(placement);

                if (place != null)
                {
                    int rc = await _context.ItemSaveAsync();

                    if (rc > 0)
                    {
                        if (placement.SendNotification)
                        {

                            string company = await OnGetCompany(placement.CompanyId);

                            Helper.SendSMS($"Dear - {getStudent.FirstName} Congratulations on being selected for workplace at {company}.You will be expected to report there from {DateTimeHandler(placement.StartDate)} till {DateTimeHandler(placement.EndDate)}",
                                           getStudent.Cellphone);

                        }

                        TempData["success"] = "Learner Placement Successful";

                        return RedirectToAction(nameof(PlacedLearners));

                    }
                }
            }
            else
            {
                TempData["error"] = "Error: Student is already placed at selected company";
            }

            return View();
        }
        public async Task<IActionResult> PlacedLearners()
        {
            var list = await _context.OnLoadItemsAsync();

            List<PlacementDTO> placementList = new List<PlacementDTO>();

            List<string> statusList = new List<string>();

            var placedLearner = from n in list

                                where n.IsActive == true

                                select n;

            foreach (var item in placedLearner)
            {
                PlacementDTO placementDTO = new PlacementDTO
                {
                    CompanyId = await OnGetCompany(item.CompanyId),

                    PlacedBy = await OnGetUsers(item.PlacedBy),

                    StartDate = DateTimeHandler(item.StartDate),

                    EndDate = DateTimeHandler(item.EndDate),

                    Status = item.Status,

                    Student = item.Student,

                    PlacementId = item.PlacementId,

                };

                statusList.Add(item.Status.ToString());

                placementList.Add(placementDTO);
            }

            ViewBag.StatusList = statusList;

            return View(placementList);
        }
        [HttpGet]
        public async Task<IActionResult> OnPlaceLearner(Guid PlacementId, string StudentNumber)
        {
            if (PlacementId == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            var users = await _userContext.OnLoadItemsAsync();

            var courses = await _courseContext.OnLoadItemsAsync();
            var modules = await _modulesContext.OnLoadItemsAsync();

            var company = await _companyContext.OnLoadItemsAsync();

            var filterUsers = from n in users

                              where n.Role == Enums.eSysRole.Facilitator ||

                              n.Role == Enums.eSysRole.Admin

                              select n;

            List<User> fUserList = filterUsers.ToList();

            IEnumerable<SelectListItem> userList = from s in fUserList

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

            IEnumerable<SelectListItem> courseList = courses.Select(s => new SelectListItem
            {
                Value = s.CourseId.ToString(),
                Text = $"{s.CourseName} ({s.Type})"
            });

            IEnumerable<SelectListItem> moduleList = modules.Select(s => new SelectListItem
            {
                Value = s.ModuleId.ToString(),
                Text = s.ModuleName
            });

            var placement = await _context.OnLoadItemAsync(PlacementId);

            PlacementDTO dto = new PlacementDTO
            {
                CompletionDate = placement.StartDate,

                PlacedBy = await OnGetUsers(placement.PlacedBy),

                CompanyId = await OnGetCompany(placement.CompanyId),

                Status = placement.Status,

                Student = placement.Student,

                StartDate = placement.StartDate.Value.ToShortDateString(),

                EndDate = placement.EndDate.Value.ToShortDateString(),

                IsActive = placement.IsActive,

                PlacementId = placement.PlacementId

            };

            ViewData["PlacementId"] = dto.PlacementId;

            ViewBag.UserId = new SelectList(userList, "Value", "Text", placement.PlacedBy);

            ViewBag.CompanyId = new SelectList(companyList, "Value", "Text");

            ViewBag.CourseId = new SelectList(courseList, "Value", "Text");

            ViewBag.ModuleId = new SelectList(moduleList, "Value", "Text");

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPlaceLearner(Models.Placement placement)
        {
            if (ModelState.IsValid)
            {
                placement.ModifiedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}";

                placement.ModifiedOn = Helper.OnGetCurrentDateTime();

                var placObj = await _context.OnModifyItemAsync(placement);

                if (placObj != null)
                {

                    ViewData["PlacementId"] = placement.PlacementId;

                    ViewData["CompanyId"] = placement.CompanyId;

                    TempData["success"] = "Learner changes saved successfully";

                    return RedirectToAction(nameof(PlacedLearners));
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
        private async Task<string> OnGetCompany(Guid CompanyId)
        {
            var company = await _companyContext.OnLoadItemAsync(CompanyId);

            return company.CompanyName;
        }
        private async Task<string> OnGetUsers(Guid UserId)
        {
            if (UserId == Guid.Empty)
            {
                throw new ArgumentException("Invalid UserId");
            }

            var users = await _userContext.OnLoadItemAsync(UserId);

            if (users == null)
            {
                throw new Exception("User not found");
            }

            return $"{users.Name} {users.LastName}";
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
                    _notify.Error("Error: StudentId not registered to ANY course!!!", 5);
                }
            }

            return student;
        }

        [HttpGet]
        public async Task<IActionResult> LearnerPlacementModules(Guid StudentId)
        {
            if (StudentId == Guid.Empty)
            {
                return NotFound();
            }

            return View();

            //Course
            //Modules
            //Placement
            //StudentId
        }
        private string DateTimeHandler(DateTime? input)
        {
            return input.Value.ToString("yyyy-MM-dd");
        }
        public async Task<JsonResult> GetCourseModuleId(Guid CourseId)
        {
            var mods = await _modulesContext.OnLoadItemsAsync();

            var filterMods = from n in mods

                             where n.CourseIdFK == CourseId

                             select n;

            return Json(filterMods.ToList());
        }
        private bool IsStudentPlaced(Guid CompanyId, string Student)
        {
            return _context.DoesEntityExist<Placement>(m => m.CompanyId == CompanyId && m.Student.Equals(Student));
        }
    }
}
