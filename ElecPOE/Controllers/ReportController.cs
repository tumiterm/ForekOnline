using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Text;

namespace ElecPOE.Controllers
{
    public class ReportController : Controller
    {
        private readonly IUnitOfWork<User> _userContext;
        private readonly IUnitOfWork<Report> _context;


        private IWebHostEnvironment _hostEnvironment;
        public INotyfService _notify { get; }
        public ReportController(IUnitOfWork<User> userContext,
                                IUnitOfWork<Report> context,
                                IWebHostEnvironment hostEnvironment,
                                INotyfService notify)
        {
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));

            _context = context ?? throw new ArgumentNullException(nameof(context));

            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

            _notify = notify ?? throw new ArgumentNullException(nameof(notify));

        }

        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        [HttpGet]
        public async Task<IActionResult> CreateReport(string IdPass)
        {
            var users = await _userContext.OnLoadItemsAsync();

            IEnumerable<SelectListItem> getUsers = from n in users

                                                   where n.Role == Enums.eSysRole.Facilitator || n.Role == Enums.eSysRole.Admin

                                                   select new SelectListItem
                                                   {
                                                       Value = n.Id.ToString(),

                                                       Text = $"({n.LastName} {n.Name} | {n.StudentNumber})"
                                                   };


            ViewBag.UserId = new SelectList(getUsers, "Value", "Text");

            User getUser = users.FirstOrDefault(m => m.IDPass == IdPass);

            ViewData["IName"] = $"{getUser.Name} {getUser.LastName}";

            ViewData["IDPass"] = IdPass;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReport(Report report)
        {
            User user = await OnGetUser(report.IdPass);

            string isDownloadable = string.Empty;

            report.IsActive = true;

            report.ReportId = Helper.GenerateGuid();

            report.Reference = $"REP-{Helper.RandomStringGenerator(4)}";

            report.CreatedBy = $"{user.Name} {user.LastName}";

            report.CreatedOn = Helper.OnGetCurrentDateTime();

            if (ModelState.IsValid)
            {
                if (report.Operation == Enums.eOperation.Upload)
                {
                    if (report.DocumentFile.FileName != null)
                    {
                        AttachmentUploader(report);

                        isDownloadable = "Yes";
                    }

                }

                var reportObj = await _context.OnItemCreationAsync(report);

                if (reportObj != null)
                {
                    int rc = await _context.ItemSaveAsync();

                    if (rc > 0)
                    {
                        TempData["success"] = "Report successfully saved";

                        Helper.OnSendMailNotification("tmanyimo@forekinstitute.co.za", "Campus Manager",
                        Helper.OnSendMessage(Helper.loggedInUser, report.ReportType.ToString(), report.CreatedOn, report.Module, report.Urgency.ToString()), "Submission");

                        if (user != null)
                        {
                            if(user.Cellphone.Length == 10)
                            {
                                StringBuilder sb = new StringBuilder();

                                sb.AppendLine($"Hi {user.Name} - this notification serves to acknowledge reciept of your report");
                                sb.AppendLine($" ");
                                sb.AppendLine($"Ref: {report.Reference}");
                                sb.AppendLine($"Type: {report.ReportType}");
                                sb.AppendLine($"Date: {DateTime.Now}");
                                sb.AppendLine($" ");
                                sb.AppendLine($"Forek Online");

                                //Helper.SendSMS(sb.ToString(), user.Cellphone);
                            }
                        }

                        return RedirectToAction(nameof(UserReports), new { IdPass = report.IdPass });
                    }
                    else
                    {
                        TempData["error"] = "Error: Unable to save report!!!";
                    }
                }
                else
                {
                    TempData["error"] = "Error: something went wrong!!!";

                }
            }
            else
            {
                TempData["error"] = "Error: All fields are required!!!";
            }

            return View();
        }

        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        public async Task<IActionResult> UserReports(string IdPass)
        {
            List<User> users = await _userContext.OnLoadItemsAsync();

            List<Report> reports = await _context.OnLoadItemsAsync();

            var getUser = from n in users

                          where n.IDPass == IdPass

                          select n;

            var reportObj = from n in reports

                            where n.IdPass == IdPass

                            select n;

            if (getUser.Count() == 0)
            {
                return RedirectToAction(nameof(IncorrectID));
            }


            ViewData["UName"] = $"{getUser.FirstOrDefault().Name} {getUser.FirstOrDefault().LastName}";

            ViewData["UId"] = $"{IdPass}";

            return View(reportObj);
        }

        public async Task<IActionResult> OnPreviewReport(Guid reportId)
        {
            var report = await _context.OnLoadItemAsync(reportId);

            return View(report);

        }

        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        [HttpGet]
        public async Task<IActionResult> ModifyReport(Guid ReportId)
        {
            if (ReportId == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            Report report = await _context.OnLoadItemAsync(ReportId);

            if (report is null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            var users = await _userContext.OnLoadItemsAsync();

            IEnumerable<SelectListItem> getUsers = from n in users

                                                   where n.Role == Enums.eSysRole.Facilitator

                                                   select new SelectListItem
                                                   {
                                                       Value = n.Id.ToString(),

                                                       Text = $"({n.LastName} {n.Name} | {n.StudentNumber})"
                                                   };

            ViewBag.UserId = new SelectList(getUsers, "Value", "Text");

            return View(report);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifyReport(Report report)
        {
            report.ModifiedBy = Helper.loggedInUser;

            report.ModifiedOn = Helper.OnGetCurrentDateTime();

            if (ModelState.IsValid)
            {
                var reportObj = await _context.OnModifyItemAsync(report);

                if (reportObj != null)
                {
                    TempData["success"] = "Report Modified successfully";

                    return RedirectToAction(nameof(UserReports));
                }
            }

            return View();
        }
        [Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
        public async Task<IActionResult> RemoveReport(Guid ReportId)
        {
            if (ReportId == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            Report report = await _context.OnLoadItemAsync(ReportId);

            if (report is null)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            int del = await _context.OnRemoveItemAsync(ReportId);

            if (del > 0)
            {
                TempData["success"] = "Report successfully removed!";

                return RedirectToAction(nameof(UserReports));
            }

            return View();

        }
        public async void AttachmentUploader(Report attachment)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;

            string fileName = Path.GetFileNameWithoutExtension(attachment.DocumentFile.FileName);

            string extension = Path.GetExtension(attachment.DocumentFile.FileName);

            attachment.Document = fileName = fileName + Helper.GenerateGuid() + extension;

            string path = Path.Combine(wwwRootPath + "/Reports/", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await attachment.DocumentFile.CopyToAsync(fileStream);
            }
        }
        public async Task<IActionResult> AttachmentDownload(string filename)
        {
            if (filename == null)

                return Content("Sorry NO Attachment found!!!");


            var path1 = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot");

            string folder = path1 + @"\Reports\" + filename;

            var memory = new MemoryStream();

            using (var stream = new FileStream(folder, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, Helper.GetContentType(folder), Path.GetFileName(folder));
        }
        public IActionResult IncorrectID()
        {
            return View();

        }
        private async Task<User> OnGetUser(string IdPass)
        {
            List<User> users = await _userContext.OnLoadItemsAsync();

            IEnumerable<User> userFilter = null;

            if (!string.IsNullOrEmpty(IdPass))
            {
                userFilter = from n in users

                             where n.IsActive == true

                             && n.Role == Enums.eSysRole.Facilitator || n.Role == Enums.eSysRole.Admin

                             && n.IDPass == IdPass

                             select n;

            }
            else
            {
                 RedirectToAction("RouteNotFound", "Global");
            }

            return userFilter.FirstOrDefault(m => m.IDPass == IdPass);
        }
    }
}


