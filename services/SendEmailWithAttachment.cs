namespace CanvasCertificateGenerator.Services;

using System;
using RestSharp;
using RestSharp.Authenticators;
using System.Threading;
using System.Threading.Tasks;
using DotNetEnv;

public class SendEmailWithAttachment
{
    public static async Task<RestResponse> Send()
    {
        DotNetEnv.Env.Load();
        string apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? "API_KEY";
        string domainName = Environment.GetEnvironmentVariable("DOMAIN_NAME") ?? "DOMAIN_NAME";

        var options = new RestClientOptions("https://api.mailgun.net")
        {
            Authenticator = new HttpBasicAuthenticator("api", apiKey)
        };
        var client = new RestClient(options);
        var request = new RestRequest($"/v3/{domainName}/messages", Method.Post);
        Console.WriteLine(domainName);

        request.AlwaysMultipartFormData = true;
        request.AddParameter("from", "Mailgun Sandbox <postmaster@sandboxb27c97ca9e014666944b213c7c4597ea.mailgun.org>");
        request.AddParameter("to", "Craig Wagner <craig.wagner.projxon@gmail.com>");
        request.AddParameter("subject", "Hello Craig Wagner");
        request.AddParameter("text", "Congratulations Craig Wagner, you just sent an email with Mailgun! You are truly awesome!");

        return await client.ExecuteAsync(request);
    }
}
