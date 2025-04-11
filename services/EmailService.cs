namespace CanvasCertificateGenerator.Services;

using System;
using System.Threading.Tasks;
using DotNetEnv;
using System.Net.Mail;
using System.Net;
using System.ComponentModel.DataAnnotations;
using System.IO;

public class EmailService
{
    //
    public static async Task SendAsync(string email, string participant, string course, string pdfPath)
    {
        Env.Load();
        string smtpEmail = Environment.GetEnvironmentVariable("SMTP_EMAIL") ?? string.Empty;
        string smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? string.Empty;

        if (string.IsNullOrWhiteSpace(smtpEmail) || string.IsNullOrWhiteSpace(smtpPassword))
        {
            throw new InvalidOperationException("SMTP credentials are not configure.");
        }

        string participantFirstName = participant.Split(" ")[0];
        string subjectText = $"{course} Certificate";
        string bodyText = $"Congratulations, {participantFirstName}!\n\nThis is your certificate for the successful completion of the {course} course on Canvas.";

        // Set up the SMTP client
        using var smtpClient = new SmtpClient("smtp.gmail.com", 587);
        {
            smtpClient.Credentials = new NetworkCredential(smtpEmail, smtpPassword);
            smtpClient.EnableSsl = true;

            // Create the email message
            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpEmail, "PROJXON Programs"),
                Subject = subjectText,
                Body = bodyText,
                IsBodyHtml = false
            };
            mailMessage.To.Add(email);

            if (File.Exists(pdfPath))
            {
                var attachment = new Attachment(pdfPath, "application/pdf")
                {
                    Name = $"{course} Certificate"
                };
                mailMessage.Attachments.Add(attachment);

                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine("Email sent successfully!");
            }
            else
            {
                Console.WriteLine("Failed to find attachment.");
            }

        }
    }

    // Validate recipient's email address
    public static bool Validate(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var emailAttribute = new EmailAddressAttribute();
        return emailAttribute.IsValid(email);
    }
}
