using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.DTO;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Dynamic;

namespace ElecPOE.Controllers
{

	[Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
	public class LessonPlanConfig : Controller
	{

		private readonly IUnitOfWork<LessonPlan> _context;
		private readonly IUnitOfWork<Course> _courseContext;
		private readonly IUnitOfWork<Module> _moduleContext;
		private readonly IUnitOfWork<User> _userContext;
		private readonly IUnitOfWork<Report> _reportContext;
		private readonly IUnitOfWork<AssessmentAttachment> _assessmentContext;

		private IWebHostEnvironment _hostEnvironment;
		public INotyfService _notify { get; }
		public LessonPlanConfig(IUnitOfWork<LessonPlan> context,
									IUnitOfWork<Module> moduleContext,
									IUnitOfWork<Course> courseContext,
									IUnitOfWork<Report> reportContext,
									IUnitOfWork<AssessmentAttachment> assessmentContext,
									IWebHostEnvironment hostEnvironment,
									IUnitOfWork<User> userContext,
									INotyfService notify)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context)); ;

			_moduleContext = moduleContext ?? throw new ArgumentNullException(nameof(moduleContext)); ;

			_courseContext = courseContext ?? throw new ArgumentNullException(nameof(courseContext));

			_assessmentContext = assessmentContext ?? throw new ArgumentNullException(nameof(assessmentContext));

			_reportContext = reportContext ?? throw new ArgumentNullException(nameof(reportContext));

			_userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));

			_hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

			_notify = notify ?? throw new ArgumentNullException(nameof(notify));

        }

		[HttpGet]
		public async Task<IActionResult> OnCreatePlan(string IdPass)
		{
			List<Course> courses = await _courseContext.OnLoadItemsAsync();
			List<Module> modules = await _moduleContext.OnLoadItemsAsync();
			List<LessonPlan> plans = await _context.OnLoadItemsAsync();
			List<User> users = await _userContext.OnLoadItemsAsync();

			var filterUser = from n in users

							 where n.IDPass == IdPass

							 select n;

			if (String.IsNullOrEmpty(IdPass) || filterUser.Count() == 0)
			{

				return RedirectToAction("IncorrectID", "Report");

			}

			var filterPlan = from n in plans

							 where n.IdPass == IdPass &&

							 n.IsActive == true

							 select n;


			IEnumerable<SelectListItem> getCourses = from n in courses

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

			ViewData["IdPass"] = OnGetCurrentUser().IDPass;

			ViewData["UserDetail"] = $"{filterUser.First().Name} {filterUser.First().LastName} ({filterUser.First().StudentNumber})";

			dynamic dynamicLesson = new ExpandoObject();

			dynamicLesson.LessonObj = filterPlan.ToList();

			return View(dynamicLesson);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> OnCreatePlan(LessonPlanDTO model)
		{
			LessonPlan plan = new LessonPlan
			{
				LessonPlanId = Helper.GenerateGuid(),

				Course = model.Course,

				Module = model.Module,

				Document = model.Document,

				IdPass = model.IdPass,

				DocumentFile = model.DocumentFile,

				Funder = model.Funder,

				IsActive = true,

				Phase = model.Phase,

				Approval = Enums.eSelection.Pending,

				Reason = model.Reason,

				Reference = $"REF-LP/{Helper.RandomStringGenerator(3)}",

				CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}",

				CreatedOn = Helper.OnGetCurrentDateTime(),

			};

			if (plan != null)
			{
				if (ModelState.IsValid)
				{
					AttachmentUploader(plan);

					LessonPlan lPlan = await _context.OnItemCreationAsync(plan);

					if (lPlan != null)
					{
						int rc = await _context.ItemSaveAsync();

						if (rc > 0)
						{
                            TempData["success"] = "Lesson Plan uploaded & saved successfully";

        //                    Helper.SendSMS($"Hi Isaac, Lesson plan with reference {lPlan.Reference} has been submitted by {lPlan.CreatedBy}" +
								//$"and requires your attention", "0661401781");

							return RedirectToAction("OnCreatePlan", "LessonPlanConfig", new { IdPass = lPlan.IdPass });
						}
						else
						{
                            TempData["error"] = "Error: Unable to save Lesson Plan!!!";
						}
					}
					else
					{
						_notify.Error("Error: Something went wrong!!!", 5);
					}
				}
				else
				{
					_notify.Error("Error: All fields are required!!!", 5);
				}
			}
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> OnModifyPlanConfig(Guid LessonPlanId)
		{
			if (LessonPlanId == Guid.Empty)
			{
				return RedirectToAction("RouteNotFound", "Global");
			}

			var plans = await _context.OnLoadItemAsync(LessonPlanId);

			List<User> users = await _userContext.OnLoadItemsAsync();

			List<Course> courses = await _courseContext.OnLoadItemsAsync();

			List<Module> modules = await _moduleContext.OnLoadItemsAsync();

			IEnumerable<SelectListItem> getCourses = from n in courses

													 select new SelectListItem
													 {
														 Value = n.CourseId.ToString(),

														 Text = $"{n.CourseName} ({n.Type})"
													 };

			IEnumerable<SelectListItem> getModules = from n in modules

													 where n.IsActive == true

													 && n.CourseIdFK == courses.First().CourseId

													 && n.ModuleName != null

													 select new SelectListItem
													 {
														 Value = n.ModuleId.ToString(),

														 Text = $"{n.ModuleName}"
													 };


			ViewBag.CourseId = new SelectList(getCourses, "Value", "Text");

			ViewBag.ModuleId = new SelectList(getModules, "Value", "Text");

			IEnumerable<SelectListItem> getAdmins = from n in users

													where n.IsActive == true &&

													n.Role == Enums.eSysRole.Admin

													select new SelectListItem
													{
														Value = n.IDPass.ToString(),

														Text = $"{n.Name} {n.LastName} ({n.StudentNumber})"
													}; 


			ViewBag.IsApprovedBy = new SelectList(getAdmins, "Value", "Text");

			LessonPlanDTO dto = new LessonPlanDTO();

			dto.LessonPlanId = LessonPlanId;
			dto.Course = plans.Course;
			dto.Module = plans.Module;
			dto.Document = plans.Document;
			dto.Funder = plans.Funder;
			dto.IdPass = plans.IdPass;
			dto.Approval = plans.Approval;
			dto.Phase = plans.Phase;
			dto.IsActive = plans.IsActive;
			dto.Reason = plans.Reason;
			dto.Reference = plans.Reference;


			return View(dto);
		}
		public async Task<IActionResult> OnModifyPlanConfig(LessonPlanDTO model)
		{
			LessonPlan plan = new();

			plan.IsApprovedBy = await OnConvertUserToString(model.IsApprovedBy);

			plan.Approval = model.Approval;

			plan.IsActive = model.IsActive;

			plan.LessonPlanId = model.LessonPlanId;

			plan.Course = model.Course;

			plan.Module = model.Module;

			plan.Document = model.Document;

			plan.Funder = model.Funder;

			plan.IdPass = model.IdPass;

			plan.Phase = model.Phase;

			plan.Reason = model.Reason;

			plan.Reference = model.Reference;


			if (plan != null)
			{
				var obj = await _context.OnModifyItemAsync(plan);

				string cell = await OnGetCellphone(plan.IdPass);

				if (obj != null)
				{
                    TempData["success"] = "Lesson Plan successfully saved";

                    switch (plan.Approval)
					{
						case Enums.eSelection.No:

							Helper.SendSMS($"Hi Your Lesson Plan with reference: {plan.Reference}" +
							$" is NOT approved. Try re-doing it and re-submit ", cell);

							break;

						case Enums.eSelection.Yes:

							//Helper.SendSMS($"Hi Your Lesson Plan with reference: {plan.Reference}" +
							//$" has been approved.", cell);

							break;

						case Enums.eSelection.Pending:

							Helper.SendSMS($"Hi Your Lesson Plan with reference: {plan.Reference}" +
							$" is still NOT approved - kindly enquire about it to make a follow up", cell);

							break;

						default: break;
					}

					return RedirectToAction("OnCreatePlan", "LessonPlanConfig", new { IdPass = plan.IdPass });

				}
				else
				{

                    TempData["error"] = "Error: Unable to save Lesson Plan!!!\"";

				}
			}
			else
			{
                TempData["error"] = "Error: Something went wrong";
            }

            return View();
		}
		public async Task<IActionResult> AttachmentDownload(string filename)
		{
			if (filename == null)

				return Content("Sorry NO Attachment found!!!");


			var path1 = Path.Combine(
						   Directory.GetCurrentDirectory(),
						   "wwwroot");

			string folder = path1 + @"\LP\" + filename;

			var memory = new MemoryStream();

			using (var stream = new FileStream(folder, FileMode.Open))
			{
				await stream.CopyToAsync(memory);
			}
			memory.Position = 0;

			return File(memory, Helper.GetContentType(folder), Path.GetFileName(folder));
		}
		public async void AttachmentUploader(LessonPlan attachment)
		{
			string wwwRootPath = _hostEnvironment.WebRootPath;

			string fileName = Path.GetFileNameWithoutExtension(attachment.DocumentFile.FileName);

			string extension = Path.GetExtension(attachment.DocumentFile.FileName);

			attachment.Document = fileName = fileName + Helper.GenerateGuid() + extension;

			string path = Path.Combine(wwwRootPath + "/LP/", fileName);

			using (var fileStream = new FileStream(path, FileMode.Create))
			{
				await attachment.DocumentFile.CopyToAsync(fileStream);
			}
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
        private async Task<Course> OnConvertCourseId(Guid CourseId)
		{
			var course = await _courseContext.OnLoadItemAsync(CourseId);

			return course;
		}
		private async Task<Module> OnConvertModuleId(Guid ModuleId)
		{
			return await _moduleContext.OnLoadItemAsync(ModuleId);
		}
		private async Task<string> OnConvertUserToString(string IdPass)
		{
			List<User> users = await _userContext.OnLoadItemsAsync();

			var userFilter = from n in users

							 where n.IsActive == true &&

							 n.IDPass == IdPass

							 select n;

			return $"{userFilter.First().Name} {userFilter.First().LastName}";
		}
		private async Task<string> OnGetCellphone(string IdPass)
		{
			List<User> users = await _userContext.OnLoadItemsAsync();

			var userFilter = from n in users

							 where n.IsActive == true &&

							 n.IDPass == IdPass

							 select n;

			return userFilter.First().Cellphone;
		}
		public async Task<IActionResult> RemoveDocument(Guid LessonPlanId)
		{
			if (LessonPlanId == Guid.Empty)
			{
				return RedirectToAction("RouteNotFound", "Global");
			}

			LessonPlan plan = await _context.OnLoadItemAsync(LessonPlanId);

			if (plan is null)
			{
				return RedirectToAction("RouteNotFound", "Global");
			}

			int del = await _context.OnRemoveItemAsync(LessonPlanId);

			if (del > 0)
			{
				return RedirectToAction("OnCreatePlan", "LessonPlanConfig", new { IdPass = plan.IdPass });
			}

			return View();
		}
		public async Task<IActionResult> StatisticalAnalysis(string IdPass)
		{
			List<Course> courses = await _courseContext.OnLoadItemsAsync();
			List<Module> modules = await _moduleContext.OnLoadItemsAsync();
			List<LessonPlan> plans = await _context.OnLoadItemsAsync();
			List<User> users = await _userContext.OnLoadItemsAsync();

			var filterUser = from n in users

							 where n.IDPass == IdPass

							 select n;


			if (String.IsNullOrEmpty(IdPass) || filterUser.Count() == 0)
			{

				return RedirectToAction("IncorrectID", "Report");

			}

			var lessons = await _context.OnLoadItemsAsync();

			var assessments = await _assessmentContext.OnLoadItemsAsync();

			int getReportCount = await ReportCount(IdPass);

			string getFullNames = await OnGetFacilitators(IdPass);

			string getLoggedInUser = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}";

			var filterLessons = from n in lessons

								where n.IdPass == IdPass

								select n;

			var assessmentList = from a in assessments

								 where a.CreatedBy == getFullNames &&

								 a.IsActive == true

								 select a;



			ViewData["fUserName"] = $"{filterUser.First().Name} {filterUser.First().LastName} ({filterUser.First().StudentNumber})";

			ViewData["LPCount"] = filterLessons.Count();

			ViewData["LPRepCount"] = getReportCount;

			ViewData["UploadsCount"] = assessmentList.Count();

			return View();
		}
		private async Task<int> ReportCount(string IdPass)
		{
			var reports = await _reportContext.OnLoadItemsAsync();

			return reports.Where(m => m.IdPass == IdPass).Count();
		}
		private async Task<string> OnGetFacilitators(string IdPass)
		{
			var users = await _userContext.OnLoadItemsAsync();

			var facilitators = from n in users

							   where n.IsActive == true &&

							   n.Role == Enums.eSysRole.Facilitator &&

							   n.IDPass == IdPass

							   select n;

			return $"{facilitators.First().Name} {facilitators.First().LastName}";

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
