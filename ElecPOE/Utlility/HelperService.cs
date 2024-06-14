using ElecPOE.Contract;
using ElecPOE.DTO;
using ElecPOE.Models;
using Newtonsoft.Json;
using RestSharp;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace ElecPOE.Utlility
{

    /// <summary>
    /// Provides various utility methods used across the application.
    /// </summary>
    public class HelperService : IHelperService
    {
        #region Private ReadOnly Variables

        private readonly IConfiguration _configuration;
        private readonly string _username;
        private readonly string _password;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smsApiKey;
        private readonly string _jwtUrl;
        private readonly string _jwtUsername;
        private readonly string _jwtPassword;
        private readonly string _apiBaseAddress;
        private static readonly Random rand = new Random();

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="Helper"/> class.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        public HelperService(IConfiguration configuration)
        {
            _configuration = configuration;
            _username = GetConfigurationValue("AppSettings:Username", "defaultUsername");
            _password = GetConfigurationValue("AppSettings:Password", "defaultPassword");
            _smtpHost = GetConfigurationValue("AppSettings:SmtpHost", "defaultSmtpHost");
            _smtpPort = int.Parse(GetConfigurationValue("AppSettings:SmtpPort", "25"));
            _smsApiKey = GetConfigurationValue("AppSettings:SmsApiKey", "defaultSmsApiKey");
            _jwtUrl = GetConfigurationValue("AppSettings:JwtUrl", "defaultJwtUrl");
            _jwtUsername = GetConfigurationValue("AppSettings:JwtUsername", "defaultJwtUsername");
            _jwtPassword = GetConfigurationValue("AppSettings:JwtPassword", "defaultJwtPassword");
            _apiBaseAddress = GetConfigurationValue("AppSettings:ApiBaseAddress", "defaultApiBaseAddress");
        }

        #region Public Constants

        public const string Alphabet = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public static string LoggedInUser { get; set; } = string.Empty;

        #endregion

        /// <summary>
        /// Generates a new GUID.
        /// </summary>
        /// <returns>A new GUID.</returns>
        public Guid GenerateGuid()
        {
            return Guid.NewGuid();
        }

        /// <summary>
        /// Generates a JWT token asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the JWT token as a string.</returns>
        public async Task<string> GenerateJwtToken()
        {
            var client = new RestClient($"{_jwtUrl}?Username={_jwtUsername}&Password={_jwtPassword}");

            var request = new RestRequest();

            var body = new User { Username = _jwtUsername, Password = _jwtPassword };

            request.AddJsonBody(body);

            var response = await client.PostAsync(request);

            var content = response.Content;

            if (!response.IsSuccessful)
            {
                // Handle error
            }

            return content.Substring(31, 280);
        }

        /// <summary>
        /// Generates a formatted message string with the specified parameters.
        /// </summary>
        /// <param name="name">The name to include in the message.</param>
        /// <param name="type">The type of report.</param>
        /// <param name="date">The date of the report.</param>
        /// <param name="module">The module related to the report.</param>
        /// <param name="urgency">The urgency level of the report.</param>
        /// <returns>A formatted message string.</returns>
        public string GenerateMessage(string name, string type, string date, string module, string urgency)
        {
            return $"Good day Sir<br/><hr/> This message serves to confirm that {name} has successfully compiled & submitted their report<br/>" +
                   $" Report details are recorded as follows:<br/><hr/> Report Type: {type}<br/>" +
                   $"Date: {date}<br/> Module: {module}<br/> Urgency: {urgency}<br/><br/>Warm Regards";
        }

        /// <summary>
        /// Generates a notification string with the specified parameters.
        /// </summary>
        /// <param name="reference">The reference number of the report.</param>
        /// <param name="reportType">The type of report.</param>
        /// <param name="user">The user receiving the notification.</param>
        /// <param name="date">The date of the report.</param>
        /// <returns>A formatted notification string.</returns>
        public string GenerateNotification(string reference, string reportType, string user, DateTime date)
        {
            return $"Good day {user} this notification serves as confirmation that you've successfully submitted your report<br/>" +
                  $"1) Ref: {reference}<br/>" +
                  $"2) Report Type: {reportType}<br/>" +
                  $"3) Date: {date}<br/>";
        }

        /// <summary>
        /// Generates a random string of the specified size using alphanumeric characters.
        /// </summary>
        /// <param name="size">The size of the string to generate.</param>
        /// <returns>A randomly generated string.</returns>
        public string GenerateRandomString(int size)
        {
            var chars = new char[size];

            for (int i = 0; i < size; i++)
            {
                chars[i] = Alphabet[rand.Next(Alphabet.Length)];
            }

            return new string(chars);
        }

        /// <summary>
        /// Gets the MIME type based on the file extension.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The MIME type of the file.</returns>
        public string GetContentType(string path)
        {
            var types = GetMimeTypes();

            var ext = Path.GetExtension(path).ToLowerInvariant();

            return types[ext];
        }

        /// <summary>
        /// Gets the current date and time formatted as a string.
        /// </summary>
        /// <returns>The current date and time as a string.</returns>
        public string GetCurrentDateTime()
        {
            return DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt");
        }

        /// <summary>
        /// Gets the display name of the specified enum value.
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>The display name of the enum value.</returns>
        public string GetDisplayName(Enum enumValue)
        {
            var displayAttribute = enumValue.GetType()

               .GetMember(enumValue.ToString())

               .FirstOrDefault()

               ?.GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.Name ?? enumValue.ToString();
        }

        /// <summary>
        /// Gets a list of students asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of students.</returns>
        public async Task<List<Student>> GetStudentListAsync()
        {
            List<Student> students = new();

            try
            {
                var token = await GenerateJwtToken();

                using var client = InitializeHttpClient();

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var response = await client.GetAsync($"{_apiBaseAddress}Students/");

                if (response.IsSuccessStatusCode)
                {
                    var results = await response.Content.ReadAsStringAsync();

                    students = JsonConvert.DeserializeObject<List<Student>>(results);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    LogError($"Error fetching students. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}, Content: {errorContent}");
                }
            }
            catch (HttpRequestException httpRequestException)
            {
                LogError($"HttpRequestException occurred: {httpRequestException.Message}");
            }
            catch (Exception ex)
            {
                LogError($"An unexpected error occurred: {ex.Message}");
            }

            if (students == null || students.Count == 0)
            {
                students = GetFallbackStudentList();
            }

            return students;
        }

        /// <summary>
        /// Increments the numeric part of the specified reference string by a specified amount.
        /// </summary>
        /// <param name="input">The reference string to increment.</param>
        /// <param name="incrementBy">The amount to increment by.</param>
        /// <returns>The incremented reference string.</returns>
        /// <exception cref="ArgumentException">Thrown when the input format is invalid or the numeric part is invalid.</exception>
        public string IncrementReference(string input, int incrementBy)
        {
            int slashIndex = input.IndexOf('/');

            if (slashIndex == -1 || slashIndex == input.Length - 1)
            {
                throw new ArgumentException("Invalid input format");
            }

            string prefix = input.Substring(0, slashIndex + 1);

            string numericPart = input.Substring(slashIndex + 1);

            if (!int.TryParse(numericPart, out int number))
            {
                throw new ArgumentException("Invalid numeric part");
            }

            number += incrementBy;

            string newNumericPart = number.ToString("D" + numericPart.Length);

            return prefix + newNumericPart;
        }

        /// <summary>
        /// Initializes and returns a new HttpClient with the base address set from configuration.
        /// </summary>
        /// <returns>A new HttpClient instance.</returns>
        public HttpClient InitializeHttpClient()
        {
            return new HttpClient { BaseAddress = new Uri(_apiBaseAddress) };
        }

        /// <summary>
        /// Maps properties from the source object to a new target object of type TTarget.
        /// </summary>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <typeparam name="TTarget">The type of the target object.</typeparam>
        /// <param name="source">The source object.</param>
        /// <returns>A new target object with mapped properties.</returns>
        public TTarget MapProperties<TSource, TTarget>(TSource source) where TTarget : new()
        {
            var target = new TTarget();

            foreach (var sourceProperty in typeof(TSource).GetProperties())
            {
                var targetProperty = typeof(TTarget).GetProperty(sourceProperty.Name);

                if (targetProperty != null && targetProperty.CanWrite)
                {
                    targetProperty.SetValue(target, sourceProperty.GetValue(source));
                }
            }

            return target;
        }

        /// <summary>
        /// Sends an email notification with the specified parameters.
        /// </summary>
        /// <param name="receiver">The receiver's email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The body of the email.</param>
        /// <param name="header">The header to include in the email.</param>
        public void SendMailNotification(string receiver, string subject, string message, string header)
        {
            var senderMail = new MailAddress(_username, "Forek Online");

            var receiverMail = new MailAddress(receiver, header);

            using var smtp = new SmtpClient(_smtpHost)
            {
                Port = _smtpPort,

                EnableSsl = true,

                DeliveryMethod = SmtpDeliveryMethod.Network,

                UseDefaultCredentials = false,

                Credentials = new NetworkCredential(senderMail.Address, _password)
            };

            using var mailMessage = new MailMessage(senderMail, receiverMail)
            {
                Subject = subject,

                Body = message,

                IsBodyHtml = true
            };

            smtp.Send(mailMessage);
        }

        /// <summary>
        /// Sends an SMS message to the specified recipient.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="recipientNo">The recipient's phone number.</param>
        public void SendSms(string message, string recipientNo)
        {
            var client = new RestClient("https://www.winsms.co.za/api/rest/v1/sms/outgoing/send/");

            var request = new RestRequest();

            request.AddHeader("Authorization", _smsApiKey);

            var sms = new SMSDTO
            {
                message = message,

                recipients = new List<Recipient>
                {
                    new Recipient { mobileNumber = recipientNo }
                }
            };

            request.AddJsonBody(sms);

            var response = client.Post(request);

            var content = response.Content;

            if (!response.IsSuccessful)
            {
                // Handle error
            }
        }

        /// <summary>
        /// Shows a notification using the specified title, text, and type.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="text">The text of the notification.</param>
        /// <param name="type">The type of the notification.</param>
        /// <returns>A formatted notification string.</returns>
        public string ShowNotification(string title, string text, string type)
        {
            return $"Swal.fire('{title}', '{text}', '{type}')";
        }

        /// <summary>
        /// Encrypts the specified value using SHA256 and encodes it in base64.
        /// </summary>
        /// <param name="value">The value to encrypt.</param>
        /// <returns>The encrypted value as a base64 string.</returns>
        public string ValueEncryption(string value)
        {
            using var sha256 = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(value);

            return Convert.ToBase64String(sha256.ComputeHash(bytes));
        }

        /// <summary>
        /// Retrieves a configuration value from the appsettings.json file.
        /// </summary>
        /// <param name="key">The key of the configuration value to retrieve.</param>
        /// <param name="defaultValue">The default value to return if the key is not found.</param>
        /// <returns>The configuration value associated with the specified key, or the default value if the key is not found.</returns>
        public string GetConfigurationValue(string key, string defaultValue)
        {
            return _configuration[key] ?? defaultValue;
        }
        public string OnSendMessage(string name, string course, string refNumber)
        {
            int year = DateTime.Now.Year;

            string template = @"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {
                            font-family: Arial, sans-serif;
                            background-color: #f5f5f5;
                            margin: 0;
                            padding: 0;
                        }
                        .container {
                            width: 100%;
                            max-width: 600px;
                            margin: 0 auto;
                            background-color: #ffffff;
                            padding: 20px;
                            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                        }
                        .header {
                            text-align: center;
                        }
                        .title {
                            font-size: 24px;
                            color: #333333;
                        }
                        .message {
                            margin: 20px 0;
                            font-size: 16px;
                            color: #555555;
                        }
                        .footer {
                            text-align: center;
                            margin-top: 30px;
                        }
                        .button {
                            background-color: #8B0000;
                            color: #ffffff;
                            padding: 15px 25px;
                            font-size: 18px;
                            display: inline-block;
                            margin: 20px 0;
                            border-radius: 5px;
                        }
                        .line {
                            border-top: 1px solid #cccccc;
                            margin: 20px 0;
                        }
                        .copyright {
                            font-size: 14px;
                            color: #888888;
                            text-align: center;
                        }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1 class='title'>Forek Online Applications</h1>
                        </div>
                        <div class='message'>
                            <p>Dear {name},</p>
                            <p>Thank you for applying for {course}. We acknowledge your application and will process it in due time.</p>
                            <p>Below is your reference number. Which must be quoted in all forms of correspondence to the institution.</p>
                        </div>
                        <div class='footer'>
                            <div class='button'>Reference Number: {ref}</div>
                        </div>
                        <div class='line'></div>
                        <div class='copyright'>
                            &copy; Copyright {year} Forek Institute of Technology
                        </div>
                    </div>
                </body>
                </html>";

            return template.Replace("{name}", name)
                           .Replace("{ref}", refNumber)
                           .Replace("{course}", course)
                           .Replace("{year}", year.ToString());
        }

        public string OnSendMailToAdmin(string name, string course, string refNumber)
        {
            int year = DateTime.Now.Year;

            string template = @"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {
                            font-family: Arial, sans-serif;
                            background-color: #f5f5f5;
                            margin: 0;
                            padding: 0;
                        }
                        .container {
                            width: 100%;
                            max-width: 600px;
                            margin: 0 auto;
                            background-color: #ffffff;
                            padding: 20px;
                            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                        }
                        .header {
                            text-align: center;
                        }
                        .title {
                            font-size: 24px;
                            color: #333333;
                        }
                        .message {
                            margin: 20px 0;
                            font-size: 16px;
                            color: #555555;
                        }
                        .footer {
                            text-align: center;
                            margin-top: 30px;
                        }
                        .button {
                            background-color: #8B0000;
                            color: #ffffff;
                            padding: 15px 25px;
                            font-size: 18px;
                            display: inline-block;
                            margin: 20px 0;
                            border-radius: 5px;
                        }
                        .line {
                            border-top: 1px solid #cccccc;
                            margin: 20px 0;
                        }
                        .copyright {
                            font-size: 14px;
                            color: #888888;
                            text-align: center;
                        }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1 class='title'>Forek Online Applications</h1>
                        </div>
                        <div class='message'>
                            <p>Hi Campus Manager</p>
                            <p>We are pleased to report that we have a new applicant awaiting your feedback. Details are as follow:</p>
                            <p>Student Name: {name}</p>
                            <p>Course Applied For: {course}</p>
                            <p>Application Date: {date}</p>
                            <p>Reference Number: {ref}</p>
                        </div>
                        <div class='footer'>
                            <div class='button'>Online Applications 2024</div>
                        </div>
                        <div class='line'></div>
                        <div class='copyright'>
                            &copy; Copyright {year} Forek Institute of Technology
                        </div>
                    </div>
                </body>
                </html>";

            return template.Replace("{name}", name)
                           .Replace("{ref}", refNumber)
                           .Replace("{course}", course)
                           .Replace("{date}", DateTime.Now.ToString())
                           .Replace("{year}", year.ToString());
        }

        #region Private Helper Methods

        /// <summary>
        /// Gets the MIME types for common file extensions.
        /// </summary>
        /// <returns>A dictionary mapping file extensions to MIME types.</returns>
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                { ".txt", "text/plain" },
                { ".pdf", "application/pdf" },
                { ".doc", "application/vnd.ms-word" },
                { ".docx", "application/vnd.ms-word" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".gif", "image/gif" },
                { ".csv", "text/csv" }
            };
        }
        private List<Student> GetFallbackStudentList()
        {
            return new List<Student>
            {
                new Student
                {
                    StudentId = Guid.NewGuid(),
                    StudentNumber = "FIT-DUMMY",
                    AdmissionDate = DateTime.Now,
                    FirstName = "Itumeleng",
                    MiddleName = "Mr Itu",
                    LastName = "Oliphant",
                    IDNumber = "ID1234567899",
                    StudyPermitNumber = "SP123456",
                    PassportNumber = "P123456",
                    DateofBirth = DateTime.Parse("2000-01-01"),
                    Gender = "Male",
                    PlaceofBirth = "City A",
                    Nationality = "South Africa",
                    Language = "English",
                    AdmissionCategory = "Category A",
                    StreetAddressLine1 = "123 Main St",
                    StreetAddressLine2 = "Apt 4B",
                    Cellphone = "1234567890",
                    Email = "ifoliphant@forekisntitute.co.za",
                    HighestGrade = "Grade 12",
                    NameofSchool = "Dummy School",
                    IsActive = true,
                    Deregistered = false,
                    EnrollmentHistory = new List<EnrollmentHistory>
                    {
                        new EnrollmentHistory
                        {
                            EnrollmentId = Guid.NewGuid(),
                            StudentId = Guid.NewGuid(),
                            CourseId = Guid.NewGuid(),
                            CourseTitle = "IT",
                            CourseType = "Software Dev",
                            EnrollmentStatus = "Completed",
                            StartDate = DateTime.Parse("2020-09-01"),
                            IsActive = true
                        }
                    }
                },
                new Student
                {
                    StudentId = Guid.NewGuid(),
                    StudentNumber = "FITDUMMY2",
                    AdmissionDate = DateTime.Now,
                    FirstName = "Jacob",
                    MiddleName = "G",
                    LastName = "Zuma",
                    IDNumber = "ID6543215556",
                    StudyPermitNumber = "SP654321",
                    PassportNumber = "P654321",
                    DateofBirth = DateTime.Parse("1999-05-15"),
                    Gender = "Female",
                    PlaceofBirth = "Nkandla",
                    Nationality = "South African",
                    Language = "Zulu",
                    AdmissionCategory = "Category B",
                    StreetAddressLine1 = "456 Elm St",
                    StreetAddressLine2 = "Suite 2A",
                    Cellphone = "098-765-4321",
                    Email = "jacob.zuma@gmail.com",
                    HighestGrade = "Grade 11",
                    NameofSchool = "School B",
                    IsActive = true,
                    Deregistered = false,
                    EnrollmentHistory = new List<EnrollmentHistory>
                    {
                        new EnrollmentHistory
                        {
                            EnrollmentId = Guid.NewGuid(),
                            StudentId = Guid.NewGuid(),
                            CourseId = Guid.NewGuid(),
                            CourseTitle = "Course 202",
                            CourseType = "Type B",
                            EnrollmentStatus = "In Progress",
                            StartDate = DateTime.Parse("2021-01-15"),
                            IsActive = true
                        }
                    }
                }
            };
        }
        private void LogError(string message)
        {
            Console.WriteLine(message);
        }

        #endregion
    }
}
