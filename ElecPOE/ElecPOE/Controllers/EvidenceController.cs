
using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.DTO;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace ElecPOE.Controllers
{
    [Authorize]
    public class EvidenceController : Controller
    {

        private readonly IUnitOfWork<Evidence> _context;

        private IWebHostEnvironment _hostEnvironment;
        public INotyfService _notify { get; }
        public EvidenceController(IUnitOfWork<Evidence> context,
                                IWebHostEnvironment hostEnvironment,
                                INotyfService notify)
        {
            _context = context;

            _hostEnvironment = hostEnvironment;

            _notify = notify;

        }

        public async Task<IActionResult> ViewStudentFile(Guid EvidenceId,string StudentNumber)
        {
            if(EvidenceId == Guid.Empty)
            {
                return NotFound();
            }

            Evidence evidence = await _context.OnLoadItemAsync(EvidenceId);

            Student student = await GetStudent(StudentNumber);

            ViewData["StudInfo"] = $"{student.FirstName} {student.LastName}";

            ViewData["By"] = evidence.CreatedBy;

            ViewData["On"] = evidence.CreatedOn;

            ViewData["Active"] = evidence.IsActive;

            ViewData["Module"] = evidence.Module;

            return View(await _context.OnLoadItemAsync(EvidenceId));
        }
        [HttpGet]
        public async Task<IActionResult> StudentGallery(string StudentNumber)
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

            List<Evidence> galleryList = await _context.OnLoadItemsAsync();

            var filterGallery = from n in galleryList

                                where n.StudentNumber == StudentNumber

                                select n;

            var finalList = filterGallery.ToList();

            Student student = await GetStudent(StudentNumber);

            ViewData["StudData"] = $"{student.FirstName} {student.LastName}";

            return View(finalList);
        }
        [HttpGet]
        public async Task<IActionResult> UploadEvidence(string StudentNumber)
        {
            Student student = await GetStudent(StudentNumber);

            if (student is null)
            {
                return NotFound();
            }

            ViewData["name"] = $"{student.FirstName} {student.LastName}";

            ViewData["StudentId"] = $"{student.StudentId}";

            ViewData["StudentNumber"] = $"{student.StudentNumber}";

            return View(nameof(UploadEvidence));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadEvidence(Evidence evidence, string StudentNumber)
        {
            evidence.IsActive = true;

            evidence.CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}";

            evidence.CreatedOn = Helper.OnGetCurrentDateTime();

            AttachmentUploader(evidence);

            Evidence uploads = await _context.OnItemCreationAsync(evidence);

            if(uploads != null)
            {
                int rc = await _context.ItemSaveAsync();

                if(rc > 0)
                {
                    TempData["success"] = "File Uploaded Successfully";

                }
                else
                {
                    TempData["error"] = "Error: Unable to upload item";
                }
            }

            return RedirectToAction(nameof(UploadEvidence), new { StudentNumber = evidence.StudentNumber});
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

                
            }

            return student;
        }
        public async void AttachmentUploader(Evidence evidence)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;

            string fileName = Path.GetFileNameWithoutExtension(evidence.PhotoFile.FileName);

            string extension = Path.GetExtension(evidence.PhotoFile.FileName);

            evidence.Photo = fileName = fileName + Helper.GenerateGuid() + extension;

            string path = Path.Combine(wwwRootPath + "/Gallery/", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await evidence.PhotoFile.CopyToAsync(fileStream);
            }
        }
        public async Task<IActionResult> AttachmentDownload(string filename)
        {
            if (filename == null)

                return Content("Sorry NO Attachment found!!!");


            var path1 = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot");

            string folder = path1 + @"\Gallery\" + filename;

            var memory = new MemoryStream();

            using (var stream = new FileStream(folder, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, Helper.GetContentType(folder), Path.GetFileName(folder));
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

        [HttpGet]
        public async Task<IActionResult> OnViewAllFiles(string StudentNumber)
        {
            List<Evidence> studentEvidence = await _context.OnLoadItemsAsync();

            EvidenceViewModel evidence = null;

            List<EvidenceViewModel> evidenceList = new List<EvidenceViewModel>();

            var filteredEvidence = from n in studentEvidence

                                   where n.StudentNumber == StudentNumber

                                   select n;

            var list = filteredEvidence.ToList();

            foreach (var item in list)
            {
                evidence = new EvidenceViewModel
                {
                    Module = item.Module,

                    Photo = item.Photo,

                    StudentId = item.StudentId,

                    StudentNumber = item.StudentNumber,
                };

                evidenceList.Add(evidence);

            }


            ViewData["Number"] = evidenceList.Count;

            Student student = await GetStudent(StudentNumber);

            ViewData["HolderNumber"] = student.StudentNumber;

            ViewData["EvidenceHolder"] = $"{student.FirstName} {student.LastName} ({student.StudentNumber})";

            return View(evidenceList);
        }
    }
}
