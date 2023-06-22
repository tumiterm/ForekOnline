using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Composition;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace ElecPOE.Controllers
{
    public class MonthlyReportController : Controller
    {
        private readonly IUnitOfWork<User> _context;
        private readonly IUnitOfWork<MonthlyReport> _monthlyContext;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly INotyfService _notify;

        public MonthlyReportController(IUnitOfWork<MonthlyReport> monthlyContext,
                                IUnitOfWork<User> context,
                                IWebHostEnvironment hostEnvironment,
                                INotyfService notify)
        {
            _monthlyContext = monthlyContext ?? throw new ArgumentNullException(nameof(monthlyContext));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

            _notify = notify ?? throw new ArgumentNullException(nameof(notify));

        }

        [HttpPost]
        public async Task FileUploader(MonthlyReport monthlyReport, 
            IFormFile file, string destinationFolder, 
            string propertyName, MonthlyReport Hod)
        {
            
            List<User> users = await _context.OnLoadItemsAsync();

            IEnumerable<User> facilitator = from n in users

                                            where n.IsActive == true &&

                                            n.Role == Enums.eSysRole.Facilitator

                                            select n;
            Hod.IsHOD = true;

           
                try
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                string folderPath = Path.Combine(wwwRootPath, destinationFolder);

                string absolutePath = wwwRootPath + folderPath;

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fileName = Path.GetFileNameWithoutExtension(file.FileName);

                string extension = Path.GetExtension(file.FileName);

                PropertyInfo property = typeof(User).GetProperty(propertyName + "Doc");

                if (property != null)
                {
                    property.SetValue(monthlyReport, fileName + Helper.GenerateGuid() + extension);
                }

                string path = wwwRootPath + Path.Combine(folderPath, fileName + extension);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
            }
        }
        //private async Task UploadFileIfNotNullAsync(MonthlyReport monthlyReport, IFormFile file, string directory, string fileName)
        //{
        //    if (file != null)
        //    {
        //        await FileUploader(monthlyReport, file, directory, fileName);
        //    }
        //}
        private void OnSendSMS(MonthlyReport model)
        {

            Helper.SendSMS("Dear Sir - a new report be submitted" +
                            $" was just recieved - ref: {model.ReferenceNumber}", "0661401781");
        }

        public IActionResult DownloadReport(string fileName, string destinationFolder)
        {
            try
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                string folderPath = Path.Combine(wwwRootPath, destinationFolder);

                string filePath = Path.Combine(folderPath, fileName);

                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                return File(fileStream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";

                return RedirectToAction(nameof(User));
            }
        }
 

    }
}



