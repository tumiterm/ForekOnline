using ElecPOE.Models;

namespace ElecPOE.Contract
{
    /// <summary>
    /// Interface for helper methods used across the application.
    /// </summary>
    public interface IHelperService
    {

        /// <summary>
        /// Encrypts the specified value using SHA256 and encodes it in base64.
        /// </summary>
        /// <param name="value">The value to encrypt.</param>
        /// <returns>The encrypted value as a base64 string.</returns>
        string ValueEncryption(string value);

        /// <summary>
        /// Generates a new GUID.
        /// </summary>
        /// <returns>A new GUID.</returns>
        Guid GenerateGuid();

        /// <summary>
        /// Initializes and returns a new HttpClient with the base address set from configuration.
        /// </summary>
        /// <returns>A new HttpClient instance.</returns>
        HttpClient InitializeHttpClient();

        /// <summary>
        /// Gets the MIME type based on the file extension.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The MIME type of the file.</returns>
        string GetContentType(string path);

        /// <summary>
        /// Generates a random string of the specified size using alphanumeric characters.
        /// </summary>
        /// <param name="size">The size of the string to generate.</param>
        /// <returns>A randomly generated string.</returns>
        string GenerateRandomString(int size);

        /// <summary>
        /// Gets the current date and time formatted as a string.
        /// </summary>
        /// <returns>The current date and time as a string.</returns>
        string GetCurrentDateTime();

        /// <summary>
        /// Generates a formatted message string with the specified parameters.
        /// </summary>
        /// <param name="name">The name to include in the message.</param>
        /// <param name="type">The type of report.</param>
        /// <param name="date">The date of the report.</param>
        /// <param name="module">The module related to the report.</param>
        /// <param name="urgency">The urgency level of the report.</param>
        /// <returns>A formatted message string.</returns>
        string GenerateMessage(string name, string type, string date, string module, string urgency);


        /// <summary>
        /// Generates a notification string with the specified parameters.
        /// </summary>
        /// <param name="reference">The reference number of the report.</param>
        /// <param name="reportType">The type of report.</param>
        /// <param name="user">The user receiving the notification.</param>
        /// <param name="date">The date of the report.</param>
        /// <returns>A formatted notification string.</returns>
        string GenerateNotification(string reference, string reportType, string user, DateTime date);


        /// <summary>
        /// Sends an email notification with the specified parameters.
        /// </summary>
        /// <param name="receiver">The receiver's email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The body of the email.</param>
        /// <param name="header">The header to include in the email.</param>
        void SendMailNotification(string receiver, string subject, string message, string header);

        /// <summary>
        /// Sends an SMS message to the specified recipient.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="recipientNo">The recipient's phone number.</param>
        void SendSms(string message, string recipientNo);

        /// <summary>
        /// Generates a JWT token asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the JWT token as a string.</returns>
        Task<string> GenerateJwtToken();

        /// <summary>
        /// Shows a notification using the specified title, text, and type.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="text">The text of the notification.</param>
        /// <param name="type">The type of the notification.</param>
        /// <returns>A formatted notification string.</returns>
        string ShowNotification(string title, string text, string type);

        /// <summary>
        /// Gets the display name of the specified enum value.
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>The display name of the enum value.</returns>
        string GetDisplayName(Enum enumValue);

        /// <summary>
        /// Maps properties from the source object to a new target object of type TTarget.
        /// </summary>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <typeparam name="TTarget">The type of the target object.</typeparam>
        /// <param name="source">The source object.</param>
        /// <returns>A new target object with mapped properties.</returns>
        TTarget MapProperties<TSource, TTarget>(TSource source) where TTarget : new();

        /// <summary>
        /// Gets a list of students asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of students.</returns>
        Task<List<Student>> GetStudentListAsync();

        /// <summary>
        /// Increments the numeric part of the specified reference string by a specified amount.
        /// </summary>
        /// <param name="input">The reference string to increment.</param>
        /// <param name="incrementBy">The amount to increment by.</param>
        /// <returns>The incremented reference string.</returns>
        string IncrementReference(string input, int incrementBy);

        /// <summary>
        /// Retrieves a configuration value from the appsettings.json file.
        /// </summary>
        /// <param name="key">The key of the configuration value to retrieve.</param>
        /// <param name="defaultValue">The default value to return if the key is not found.</param>
        /// <returns>The configuration value associated with the specified key, or the default value if the key is not found.</returns>
        string GetConfigurationValue(string key, string defaultValue);
        string OnSendMessage(string name, string course, string refNumber);
        string OnSendMailToAdmin(string name, string course, string refNumber);




    }
}
