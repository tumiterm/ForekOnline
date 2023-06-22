using ElecPOE.DTO;
using ElecPOE.Models;
using Newtonsoft.Json;
using RestSharp;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;

namespace ElecPOE.Utlility
{
    public static class Helper
    {
        static Random rand = new Random();
        private static string _username = $"chantelletheledi";
        private static string _password = "P@chantelletheledi";
        public static string contact = "chantelletheledi";

        public const string Alphabet =
        "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string loggedInUser = "";
        public static string ValueEncryption(string value)
        {
            return Convert.ToBase64String(

                System.Security.Cryptography.SHA256.Create()

                .ComputeHash(Encoding.UTF8.GetBytes(value))

                );
        }
        public static Guid GenerateGuid()
        {
            return Guid.NewGuid();
        }
        public static HttpClient Initialize(string baseAddress)
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri(baseAddress);

            return client;
        }
        public static string GetContentType(string path)
        {
            var types = GetMimeTypes();

            var ext = Path.GetExtension(path).ToLowerInvariant();

            return types[ext];
        }
        public static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
        public static string RandomStringGenerator(int size)
        {
            char[] chars = new char[size];

            for (int i = 0; i < size; i++)
            {
                chars[i] = Alphabet[rand.Next(Alphabet.Length)];
            }

            return new string(chars);

        }
        public static string OnGetCurrentDateTime()
        {
            return DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt");
        }
        public static string OnSendMessage(string name, string type, string date, string module, string urgency)
        {
            return $"Good day Sir<br/><hr/> " +
                $"This message serves to confirm that {name} has successfully compiled & submitted their report<br/>" +
                $" Report details are recorded as follows:<br/><hr/> Report Type: {type}<br/>" +
                $"Date: {date}<br/> Module: {module}<br/> Urgency: {urgency}<br/><br/>Warm Regards";

        }
        public static string OnSendNotification(string reference, string reportType, string user, DateTime date)
        {
            return $"Good day {user} this notification servers as confirmation that you've successfully submitted your report<br/>" +
                $"1) Ref: {reference}<br/>" +
                $"2) Report Type: {reportType}<br/>" +
                $"3) Date: {date}<br/>";
        }
        public static void OnSendMailNotification(string reciever, string subject, string message, string header)
        {
            var senderMail = new MailAddress(_username, $"Forek Online");

            var recieverMail = new MailAddress(reciever, header);

            var password = _password;

            var sub = subject;

            var body = message;

            var smtp = new SmtpClient
            {
                Host = "smtp.forek.co.za",

                Port = 587,

                EnableSsl = true,

                DeliveryMethod = SmtpDeliveryMethod.Network,

                UseDefaultCredentials = false,

                Credentials = new NetworkCredential(senderMail.Address, password)
            };

            using (var mess = new MailMessage(senderMail, recieverMail)
            {
                Subject = subject,

                Body = body,

                IsBodyHtml = true,

            })

            {
                //mess.Attachments.Add(new Attachment("C:\\file.zip"));

                smtp.Send(mess);
            }
        }
        public static void SendSMS(string message, string recipientNo)
        {
            var client = new RestClient("https://www.winsms.co.za/api/rest/v1/sms/outgoing/send/");

            var request = new RestRequest();

            request.AddHeader("Authorization", "chantelletheledi");

            SMSDTO sms = new SMSDTO
            {
                message = message,

                recipients = new List<Recipient>
                {
                    new Recipient { mobileNumber = recipientNo}
                }

            };

            request.AddJsonBody(sms);

            var response = client.Post(request);

            string content = response.Content.ToString();

            if (!response.IsSuccessful)
            {

            }

        }
        public static string GenerateJWTToken()
        {
            var client = new RestClient("http://forekapi.dreamline-ict.co.za/authenticate/?Username=forekapi@forekinstitute.co.za&Password=api.P@ssw0rd");

            var request = new RestRequest();

            var body = new User { Username = "chantelletheledi", Password = "chantelletheledi" };

            request.AddJsonBody(body);

            var response = client.Post(request);

            string content = response.Content.Substring(31, 280);

            if (!response.IsSuccessful)
            {
               // ViewData["data"] = "Error: Server encountered an error";
            }

            return content;
        }
        public static string ShowNotification(string title, string text, string type)
        {
            return $"Swal.fire('{title}', '{text}', '{type}')";
        }
        public static string GetDisplayName(this Enum enumValue)
        {
            var displayAttribute = enumValue.GetType()

                .GetMember(enumValue.ToString())

                .FirstOrDefault()

                ?.GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.Name ?? enumValue.ToString();
        }
    }
}

