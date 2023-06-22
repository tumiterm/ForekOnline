using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.Data;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ElecPOE.Controllers
{
    public class MinuteController : Controller
    {
        private readonly IUnitOfWork<Minute> _context;

        private readonly INotyfService _notify;

        private IWebHostEnvironment _hostEnvironment;



        public MinuteController(IUnitOfWork<Minute> context, IWebHostEnvironment hostEnvironment, INotyfService notify)
        {
            _context = context;

            _hostEnvironment = hostEnvironment;

            _notify = notify;
        }

        [HttpGet]
        public async Task<IActionResult> OnCreateMinutes()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OnCreateMinutes(Minute minute)
        {
            Guid globalGuid = Helper.GenerateGuid();
            minute.MinuteId = globalGuid;

            if(ModelState.IsValid)
            {
                Minute minutes = new Minute
                {
                    MinuteId = globalGuid,

                    CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}",

                    IsActive = true,

                    CreatedOn = Helper.OnGetCurrentDateTime(),

                    MinuteName = minute.MinuteName,

                    Venue = minute.Venue,

                    Date = minute.Date,

                    RequestedBy = minute.RequestedBy,

                    MeetingType = minute.MeetingType,
                }; 

                if(minutes != null )
                {
                    Minute Minutes = await _context.OnItemCreationAsync(minute);

                    if (Minutes != null)
                    {
                        int rc = await _context.ItemSaveAsync();

                        if (rc > 0)
                        {
                            _notify.Success("Minutes saved successfully", 5);
                        }
                    }
                }

            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult>OnModifyMinutes(Guid MinuteId)
        {
            if (MinuteId == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            var minutes = await _context.OnLoadItemAsync(MinuteId);

            Minute minute = null;

            if (minutes != null)
            {
                minute = new Minute
                {
                    MinuteName = minutes.MinuteName,

                    Venue = minutes.Venue,

                    Date = minutes.Date,

                    RequestedBy = minutes.RequestedBy,

                    MeetingType = minutes.MeetingType,
                   

                };
            }
            return View(minute);
        }

        [HttpPost]
        public async Task<IActionResult>OnModifyMinutes(Minute minute)
        {
           
            if (ModelState.IsValid)
            {

            }
            return View(minute);
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
    }
}
