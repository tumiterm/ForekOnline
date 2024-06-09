using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.DTO;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ElecPOE.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUnitOfWork<User> _context;

        private IWebHostEnvironment _hostEnvironment;
        public INotyfService _notify { get; }
        public AuthController(IUnitOfWork<User> context,
                                IWebHostEnvironment hostEnvironment,
                                INotyfService notify)
        {
            _context = context;

            _hostEnvironment = hostEnvironment;

            _notify = notify;

        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<IActionResult> OnViewUserInfo(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                return RedirectToAction("PageNotFound", "Global");
            }

            User user = await _context.OnLoadItemAsync(Id);

            if (user is null)
            {
                return RedirectToAction("PageNotFound", "Global");
            }

            ViewData["Password"] = user.Password;

            ViewData["CPassword"] = user.ConfirmPassword;

            ViewData["user"] = $"{user.Name} {user.LastName}";

            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnViewUserInfo(User user)
        {
            user.IsActive = true;

            user.LastLoginDate = Helper.OnGetCurrentDateTime();

            user.ModifiedBy = Helper.loggedInUser;

            user.ModifiedOn = Helper.OnGetCurrentDateTime();

            if (ModelState.IsValid)
            {
                var userObj = await _context.OnModifyItemAsync(user);

                if (userObj != null)
                {
                    TempData["success"] = "User details successfully saved";
                }
                else
                {
                    TempData["error"] = "Unable to add user!";
                }
            }
            else
            {
                TempData["error"] = "Please fill in all the fields!";

            }

            return RedirectToAction(nameof(RetrieveUsers));
        }

        [Authorize]
        [Route("/Auth/RemoveUser/{Id}")]
        public async Task<IActionResult> RemoveUser(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            User user = await _context.OnLoadItemAsync(Id);

            if (user is null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            int del = await _context.OnRemoveItemAsync(Id);

            if (del > 0)
            {
                return CreatedAtAction("RemoveUser", new { Id = user.Id });
            }

            return View();  

        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }
        public async Task<IActionResult> SignUp(User user)
        {
            bool Status = false;

            string message = "";

            #region Generate Activation Code 

            string time = DateTime.Now.ToString("hh:mm tt");

            string date = DateTime.Now.ToString("dddd, dd MMMM yyyy") + " " + time;

            user.ActivationCode = Helper.GenerateGuid();

            user.LastLoginDate = date;

            user.ResetPasswordCode = "";

            user.Id = Helper.GenerateGuid();

            #endregion

            #region  Password Hashing 

            user.Password = Helper.ValueEncryption(user.Password);

            user.ConfirmPassword = Helper.ValueEncryption(user.ConfirmPassword);

            #endregion

            user.IsEmailVerified = false;

            user.IsActive = true;

            user.CreatedOn = Helper.OnGetCurrentDateTime();

            user.ResetPasswordCode = "";

            user.Role = Enums.eSysRole.Student;

            if (ModelState.IsValid)
            {
                #region Save to Database

                try
                {
                    if (!_context.DoesEntityExist<User>(m => m.StudentNumber == user.StudentNumber))
                    {
                        var userAddition = _context.OnItemCreationAsync(user);

                        if (userAddition != null)
                        {
                            int rc = await _context.ItemSaveAsync();

                            if (rc > 0)
                            {
                                ViewData["successMessage"] = $"User Successfully Registered";

                            }
                        }

                        //SendVerificationLinkEmail(user.Username, user.ActivationCode.ToString(), "", "VerifyAccount");

                        //message = " Registration successful. Account activation link " +

                        //    " has been sent to your email: " + user.Username + "\nKindly click on the link to activate your account.\n" +

                        //    " Regards";

                        Status = true;

                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                #endregion
            }
            else
            {
                message = "Registration Successful";
            }

            ViewBag.Message = message;

            ViewBag.Status = Status;

            return View(user);
        }
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(UserViewModel user, string? ReturnUrl = "")
        {
            string message = "";

            if (!String.IsNullOrEmpty(user.StudentNumber))
            {
                if (!String.IsNullOrEmpty(user.Password))
                {
                    if (ModelState.IsValid)
                    {
                        var getUsers = await _context.OnLoadItemsAsync();

                        var filterUsers = from n in getUsers

                                          where n.StudentNumber == user.StudentNumber &&

                                          n.Password == Helper.ValueEncryption(user.Password) &&

                                          n.IsActive == true

                                          select n;

                        var userInfo = filterUsers.FirstOrDefault();

                        if(userInfo == null) {

                            TempData["ErrorMessage"] = "Invalid email or password.";

                            return RedirectToAction(nameof(SignIn));
                        }

                        if (userInfo != null)
                        {
                            var claims = new List<Claim>()
                            {
                               new Claim(ClaimTypes.NameIdentifier,userInfo.ToString()),
                               new Claim(ClaimTypes.Name, userInfo.Name),
                               new Claim(ClaimTypes.Surname, userInfo.LastName),
                               new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString()),
                               new Claim(ClaimTypes.Role, userInfo.Role.ToString()),
                               new Claim("POEAppCookie","Code")
                            };

                            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            var principal = new ClaimsPrincipal(identity);

                            var currentUserName = identity.FindFirst(ClaimTypes.Name);

                            var currentUserSurname = identity.FindFirst(ClaimTypes.Surname);

                            var currentUserId = identity.FindFirst(ClaimTypes.NameIdentifier);

                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
                            {
                                IsPersistent = user.RememberMe

                            });

                            if (string.Compare(Helper.ValueEncryption(user.Password), userInfo.Password) == 0)
                            {
                                int timeout = user.RememberMe ? 525600 : 20;

                                if (Url.IsLocalUrl(ReturnUrl))
                                {
                                    return Redirect(ReturnUrl);
                                }
                                else
                                {
                                    Helper.loggedInUser = $"{currentUserName.Value} {currentUserSurname.Value}";

                                    HttpContext.Session.SetString("SessionUser", JsonConvert.SerializeObject(userInfo));

                                    userInfo.LastLoginDate = DateTime.Now.ToString();

                                    await _context.ItemSaveAsync();

                                    if (userInfo.Role == Enums.eSysRole.None)
                                    {
                                        return RedirectToAction("AccessDenied", "Global");

                                    }
                                    else if (userInfo.Role == Enums.eSysRole.Admin || userInfo.Role == Enums.eSysRole.SuperAdmin)
                                    {
                                        return RedirectToAction("AdminPanel", "Home");

                                    }
                                    else if (userInfo.Role == Enums.eSysRole.Facilitator)
                                    {
                                        return RedirectToAction("StudentList", "Student");

                                    }

                                    else if (userInfo.Role == Enums.eSysRole.Student)
                                    {
                                        return RedirectToAction("StudentDetail", "Student", new { StudentNumber  = user.StudentNumber});

                                    }
                                }

                            }
                            else
                            {
                                message = "Invalid credentials provided";

                                _notify.Error("Invalid credentials provided");
                            }
                        }
                        else
                        {
                            //message = "Error: An error occured!";

                            //_notify.Error("Error: An error occured!");

                        }
                    }
                }
                else
                {
                    message = "Error: Invalid or Empty Password!";

                    _notify.Error("Error: Invalid or Empty Password!");
                }
            }
            else
            {
                message = "Error: Invalid or Empty Email Provided!";

                _notify.Error("Error: Invalid or Empty Email Provided!");
            }

            ViewBag.Message = message;

            return View();
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> RetrieveUsers()
        {
            return View(await _context.OnLoadItemsAsync());
        }

        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        public async Task<IActionResult> Facilitator()
        {
            List<User> users = await _context.OnLoadItemsAsync();

            IEnumerable<User> facilitator = from n in users

                                            where n.IsActive == true &&

                                            n.Role == Enums.eSysRole.Facilitator

                                            select n;

            return View(facilitator);
        }
        public async Task<ActionResult> Logout()
        {
            Helper.loggedInUser = String.Empty;

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(SignIn));
        }
        private bool ContainsDomain(string username,string domain)
        {
            bool contains = false;

            if(!(string.IsNullOrEmpty(username) && string.IsNullOrEmpty(domain)))
            {
                if (username.Contains(domain))
                {
                    contains = true;
                }
            }
            return contains;
        }
        private async Task<Student> IsValidStudent(string StudentNumber)
        {
            Student student = null;

            if (StudentNumber.StartsWith("FIT202"))
            {
                if(StudentNumber.Length <= 12)
                {
                    List<Student> students = await StudentList();

                    var filterStud = from n in students

                                     where n.StudentNumber == StudentNumber

                                     select n;

                    foreach(Student stud in filterStud)
                    {
                        student = new Student
                        {
                            AdmissionCategory = stud.AdmissionCategory,

                            Cellphone = stud.Cellphone,

                            DateofBirth = stud.DateofBirth,

                            Deregistered = stud.Deregistered,

                            IDNumber = stud.IDNumber,

                            FirstName = stud.FirstName,

                            LastName = stud.LastName,

                            StudentId = stud.StudentId,

                            StudentNumber = stud.StudentNumber,

                        };
                    }
                }
            }
            return student;
        }
        private async Task<List<Student>> StudentList()
        {
            string token = Helper.GenerateJWTToken();

            List<Student> students = new List<Student>();

            HttpClient client = Helper.Initialize("http://forekapi.dreamline-ict.co.za/api/Students/");

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            HttpResponseMessage res = await client.GetAsync("http://forekapi.dreamline-ict.co.za/api/Students/");

            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;

                students = JsonConvert.DeserializeObject<List<Student>>(results);
            }

            return students;
        }
        public IActionResult Settings() 
        {
            return View();
        }

    }
}

