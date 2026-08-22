using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using PathAndPaws.Models;

namespace PathAndPaws.Services;

public class ResendService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public ResendService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public async Task SendAsync(Lead form)
    {
        var apiKey = _config["Resend:ApiKey"];

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.resend.com/emails"
        );

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        var payload = new
        {
            from = "Visionary Analytics <onboarding@resend.dev>",
            to = new[] { "robin@visionaryanalytics.co.uk" },
            subject = "New Path and Paws Contact Submission",
            text =
$@"Owners Name: {form.OwnersName}
Dogs Name: {form.DogsName}
Email: {form.Email}
Phone: {form.Phone}

Notes:
{form.Notes}"
        };

        request.Content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Resend error: {error}");
        }
    }
}