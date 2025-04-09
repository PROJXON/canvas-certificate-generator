namespace CanvasCertificateGenerator.Services;

using System;
using System.Threading.Tasks;
using DotNetEnv;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.ComponentModel.DataAnnotations;

public class SendEmailWithAttachment
{
    //
    public static async Task Send(string email, string participant, string course)
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

            // Send the email
            await smtpClient.SendMailAsync(mailMessage);
            Console.WriteLine("Email sent successfully!");
        }
    }

    public static bool ValidateRecipientEmailAddress(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var emailAttribute = new EmailAddressAttribute();
        return emailAttribute.IsValid(email);
    }
}
