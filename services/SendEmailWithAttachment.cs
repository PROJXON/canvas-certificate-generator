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
    public static async Task Send(string email)
    {
        UserCredential credential;
        using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
        {
            string credPath = "token.json";
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
            ApplicationName = "Gmail API C# Send Example",
        });

        // Compose the email
        var message = new Message
        {
            Raw = Base64UrlEncode(CreateEmail("recipient@example.com", "me", "Hello from Gmail API", "This is the body of the email."))
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

    static bool ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var emailAttribute = new EmailAddressAttribute();
        return emailAttribute.IsValid(email);
    }
}
