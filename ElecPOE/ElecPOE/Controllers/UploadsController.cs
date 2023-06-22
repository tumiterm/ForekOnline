using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Dynamic;

namespace ElecPOE.Controllers
{
    [Authorize]
    public class UploadsController : Controller
    {

        private readonly IUnitOfWork<Material> _context;

        private IWebHostEnvironment _hostEnvironment;
        public INotyfService _notify { get; }
        public UploadsController(IUnitOfWork<Material> context,
                                IWebHostEnvironment hostEnvironment,
                                INotyfService notify)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

            _notify = notify ?? throw new ArgumentNullException(nameof(notify));

        }
        [HttpGet]
        public async Task<IActionResult> StudentMaterial()
        {
            var material = await _context.OnLoadItemsAsync();

            //var filterAttachments = from n in material

            //                        where n.InstructorId == InstructorId

            //                        select n;

            List<Material> list = material.ToList();

            dynamic refObj = new ExpandoObject();

            refObj.Material = list;

            return View(refObj);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudentMaterial(Material material)
        {
            material.MaterialId = Helper.GenerateGuid();

            material.IsActive = true;

            material.CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}";

            material.CreatedOn = Helper.OnGetCurrentDateTime();

            material.InstructorId = Helper.ValueEncryption("9408015272088");

            if (ModelState.IsValid)
            {
                Material materialObj = await _context.OnItemCreationAsync(material);

                if(materialObj != null)
                {
                    int rc = await _context.ItemSaveAsync();

                    if(rc > 0)
                    {
                        _notify.Success("Learner Material Uploaded Successfully", 5);

                        return View();
                    }
                }
            }

            return View();
        }

        private User OnGetCurrentUser()
        {
            try
            {
                string sessionUserJson = HttpContext.Session.GetString("SessionUser");

                if (string.IsNullOrEmpty(sessionUserJson))
                {
                    return null;
                }

                User user = JsonConvert.DeserializeObject<User>(sessionUserJson);
                return user;
            }
            catch (JsonException)
            {
                return null;
            }
        }

    }
}
