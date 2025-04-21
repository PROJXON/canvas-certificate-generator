using System;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using System.Net.Http;
using System.Text.Json;

namespace CanvasCertificateGenerator.Services;

public class EmailService
{
    public string To { get; set; } = "";
    public string Name { get; set; } = "";
    public string Course { get; set; } = "";
    public string Base64Pdf { get; set; } = "";

    public static async Task SendEmailViaLambdaAsync(string email, string name, string course, byte[] pdfBytes)
    {
        var payload = new EmailService
        {
            To = email,
            Name = name,
            Course = course,
            Base64Pdf = Convert.ToBase64String(pdfBytes)
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        using var httpClient = new HttpClient();

        var lambdaEndpoint = "placeholder";

        var response = await httpClient.PostAsync(lambdaEndpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to send email: {response.StatusCode}\n{error}");
        }
    }

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
