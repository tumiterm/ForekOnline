using AspNetCoreHero.ToastNotification.Abstractions;
using ElecPOE.Contract;
using ElecPOE.DTO;
using ElecPOE.Models;
using ElecPOE.Utlility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace ElecPOE.Controllers
{
    public class LearnerAssessmentController : Controller
    {
        private readonly IUnitOfWork<AssessmentAttachment> _context;
        public INotyfService _notify { get; }
        public LearnerAssessmentController(IUnitOfWork<AssessmentAttachment> context,
                                          INotyfService notify)
        {
            _context = context;

            _notify = notify;

        }
        [HttpGet]
        public async Task<IActionResult> StudAssessments()
        {
            List<AssessmentAttachment> list1 = await _context.OnLoadItemsAsync();
            List<LearnerAssessmentDTO> list2 = new List<LearnerAssessmentDTO>();

            LearnerAssessmentDTO? assessmentDTO = null;
           
            foreach (var item in list1) 
            {
                assessmentDTO = new LearnerAssessmentDTO
                {
                    Assessment = new AssessmentDTO
                    {
                        Module = item.Module,

                        Student = await GetStudent(item.StudentNumber),

                        CreatedBy= item.CreatedBy,

                        CreatedOn= item.CreatedOn,

                        Type= item.Type,
                    } 
                };

                list2.Add(assessmentDTO);   
            }

            return View(list2);
        }
        private async Task<string> GetStudent(string StudentNumber)
        {
            string res = string.Empty;

            Student student = new Student();

            var client = new RestClient($"http://forekapi.dreamline-ict.co.za/api/StudentId?StudentNumber={StudentNumber}");

            var request = new RestRequest();

            request.AddParameter("StudentNumber", StudentNumber);

            request.AddHeader("Authorization", $"Bearer {Helper.GenerateJWTToken()}");

            var response = await client.ExecuteGetAsync(request);

            if (response.IsSuccessful)
            {
                res = response.Content.ToString();

                student = JsonConvert.DeserializeObject<Student>(res);

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

            return $"{student.FirstName} {student.LastName} [{student.EnrollmentHistory.First().CourseTitle}]";
        }

    }
}
