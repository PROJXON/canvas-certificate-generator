namespace CanvasCertificateGenerator.Services;

using System;
using RestSharp;
using RestSharp.Authenticators;
using System.Threading.Tasks;
using DotNetEnv;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Text;
using System.Threading;
using System.ComponentModel.DataAnnotations;

public class SendEmailWithAttachment
{
    //
    public static async Task Send(string email, string participant, string course)
    {
        string participantFirstName = participant.Split(" ")[0];

        UserCredential credential;
        using (var stream = new FileStream("./assets/credentials.json", FileMode.Open, FileAccess.Read))
        {
            string credPath = "../assets";
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                new[] { GmailService.Scope.GmailSend },
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true));
        }

        // Create the Gmail API service
        var service = new GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Canvas Certificate Generator",
        });

        // Compose the email
        string bodyText = $"Congratulations, {participantFirstName}!\n\nThis is your certificate for the successful completion of the {course} course on Canvas.";
        string subjectText = $"{course} Certificate";
        var message = new Message
        {
            Raw = Base64UrlEncode(CreateEmail(email, "PROJXON Programs", subjectText, bodyText))
        };

        // Send the email
        var result = await service.Users.Messages.Send(message, "me").ExecuteAsync();
        Console.WriteLine("Message ID: " + result.Id);
    }

    static string CreateEmail(string to, string from, string subject, string body)
    {
        var email = new StringBuilder();
        email.AppendLine($"To: {to}");
        email.AppendLine($"From: {from}");
        email.AppendLine($"Subject: {subject}");
        email.AppendLine("Content-Type: text/plain; charset=utf-8");
        email.AppendLine();
        email.AppendLine(body);
        return email.ToString();
    }

    static string Base64UrlEncode(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");
    }

    public static bool ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var emailAttribute = new EmailAddressAttribute();
        return emailAttribute.IsValid(email);
    }
}
