#region Using Directives

using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.DTO;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

#endregion

namespace ElecPOE.Controllers
{
    //[Authorize(Roles = "Admin,SuperAdmin,Facilitator")]
    public class CompanyController : Controller
    {
        #region Private DbContext

        private readonly IUnitOfWork<Company> _context;
        private readonly IUnitOfWork<ContactPerson> _contactContext;
        private readonly IUnitOfWork<Address> _addressContext;
        private readonly IUnitOfWork<Placement> _placementContext;
        private readonly IUnitOfWork<Course> _courseContext;
        private IWebHostEnvironment _hostEnvironment;
        private readonly IConfiguration _configuration;

        #endregion

        #region Public Variables
        public INotyfService _notify { get; }

        HelperService helper;

        #endregion
        public CompanyController(IUnitOfWork<Company> context,
                                    IUnitOfWork<ContactPerson> contactContext,
                                    IUnitOfWork<Address> addressContext,
                                    IUnitOfWork<Course> courseContext,
                                    IUnitOfWork<Placement> placementContext,
                                    IWebHostEnvironment hostEnvironment,
                                    IConfiguration configuration,
                                    INotyfService notify)
        {
            _context = context;

            _contactContext = contactContext;

            _placementContext = placementContext;

            _addressContext = addressContext;

            _courseContext = courseContext;

            _hostEnvironment = hostEnvironment;

            _notify = notify;

            _configuration = configuration;

            /* Not ideal location */
            helper = new(_configuration);

        }

        public IActionResult Companies()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Companies(CompanyAddressContactDTO companyDTO)
        {
            Guid scopedGuid = Helper.GenerateGuid();

            companyDTO.CompanyId = scopedGuid;

            if (ModelState.IsValid)
            {
                Company company = new Company
                {
                    CompanyId = scopedGuid,

                    CreatedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}",

                    IsActive = true,

                    CreatedOn = Helper.OnGetCurrentDateTime(),

                    CompanyName = companyDTO.CompanyName,

                    Phone = companyDTO.Phone,

                    Speciality = companyDTO.Speciality,

                    Address = new Address
                    {
                        City = companyDTO.City,

                        PostalCode = companyDTO.PostalCode,

                        AddressId = Helper.GenerateGuid(),

                        Line1 = companyDTO.Line1,

                        Province= companyDTO.Province,

                        StreetName= companyDTO.StreetName,

                        AssociativeId = scopedGuid

                    },

                    Contact = new ContactPerson
                    {
                        ContactId = Helper.GenerateGuid(),

                        AssociativeId = scopedGuid,

                        Cellphone = companyDTO.Cellphone,

                        Email = companyDTO.Email,   

                        LastName = companyDTO.LastName, 

                        Name = companyDTO.Name, 
                    }


                };

                if(company != null)
                {
                    Company companyObj = await _context.OnItemCreationAsync(company);

                    if(companyObj != null)
                    {
                        int rc = await _context.ItemSaveAsync();

                        if(rc > 0)
                        {
                            TempData["success"] = "Company successfully saved";
                        }
                        else
                        {
                            TempData["error"] = "Error: Company NOT saved!!!";
                        }
                    }
                    else
                    {
                        TempData["error"] = "Error: Unable to save company!!!";
                    }
                }
            }

            return View();
        }
        public async Task<IActionResult> HostCompanies()
        {
            var companies = await _context.OnLoadItemsAsync();

            var filterCompanies = from n in companies

                                  where n.IsActive == true

                                  select n;

            return View(filterCompanies.ToList());
        }

        [HttpGet]
        public async Task<IActionResult> OnModifyHostCompany(Guid CompanyId)
        {
            if(CompanyId == Guid.Empty)
            {
                return RedirectToAction("RouteNotFound", "Global");
            }

            Company company = await _context.OnLoadItemAsync(CompanyId);

            List<ContactPerson> contacts = await _contactContext.OnLoadItemsAsync();

            List<Address> addressList = await _addressContext.OnLoadItemsAsync();

            var list = from n in contacts

                       where n.AssociativeId == company.CompanyId

                       select n;

            var addresses = from n in addressList

                       where n.AssociativeId == company.CompanyId

                       select n;

            CompanyAddressContactDTO cacDTO = new CompanyAddressContactDTO
            {
                Phone = company.Phone,

                Speciality = company.Speciality,

                CompanyName = company.CompanyName, 

                CompanyId = CompanyId,

                Email = list.First().Email, 

                Cellphone = list.First().Cellphone,

                Name = list.First().Name,

                LastName= list.First().LastName,

                City = addressList.First().City,

                Line1 = addressList.First().Line1,  
                
                StreetName = addressList.First().StreetName,

                PostalCode= addressList.First().PostalCode,

                Province= addressList.First().Province,

                IsActive = company.IsActive,
                   

            };

            return View(cacDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnModifyHostCompany(CompanyAddressContactDTO model)
        {
            Company company = null;

            if(ModelState.IsValid)
            {
                company = new Company
                {
                    Phone = model.Phone,

                    Speciality = model.Speciality,

                    CompanyName = model.CompanyName,

                    CompanyId = model.CompanyId,

                    Address = new Address
                    {
                        City = model.City,

                        Line1 = model.Line1,

                        StreetName = model.StreetName,

                        PostalCode = model.PostalCode,

                        Province = model.Province,
                    },

                    Contact = new ContactPerson
                    {
                        Cellphone= model.Cellphone,

                        Email = model.Email,

                        Name = model.Name,

                        LastName = model.LastName,  
                    },

                    IsActive = model.IsActive,

                    ModifiedBy = $"{OnGetCurrentUser().Name} {OnGetCurrentUser().LastName}",

                    ModifiedOn = Helper.OnGetCurrentDateTime(),

                };

                if(company != null && company.Contact != null && company.Address != null)
                {
                    var compModel = await _context.OnModifyItemAsync(company);  

                    if (compModel != null)
                    {
                        TempData["success"] = "Company successfully saved";

                        return RedirectToAction(nameof(HostCompanies));
                    }
                }
            }

            return View();
        }

        public async Task<IActionResult> OnGetLearners(Guid CompanyId)
        {
            List<LearnerDTO> students = new List<LearnerDTO>(); 

            var placement = await _placementContext.OnLoadItemsAsync(); 
            
            if (placement is null)
            {
                return NotFound();
            }

            var filterPlacement = from n in placement

                                  where n.CompanyId  == CompanyId 

                                  select n;

            foreach (var studs in filterPlacement)
            {
                Guid guid = studs.CompanyId;

                LearnerDTO ldto = new LearnerDTO
                {
                    LearnerName = studs.Student,

                    End = studs.EndDate.Value.ToShortDateString(),

                    Start = studs.StartDate.Value.ToShortDateString(),

                    Status = studs.Status.ToString(),
                };

                students.Add(ldto);
            }
            var company = await OnConvertToCompany(CompanyId);

           var address =  _addressContext.OnLoadItemsAsync().Result.Where(m => m.AssociativeId == company.CompanyId).FirstOrDefault();

            ViewData["comp"] = $"{company.CompanyName} ({company.Speciality})";

            ViewData["address"] = $"{address.Line1} {address.StreetName} {address.City} {address.Province} {address.PostalCode}";

            return View(students);
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
        private async Task<Company> OnConvertToCompany(Guid CompanyId)
        {
            return await _context.OnLoadItemAsync(CompanyId);
        }

    }
}
