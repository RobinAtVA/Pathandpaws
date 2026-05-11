using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using PathAndPaws.Models;

namespace PathAndPaws.Services;

public class EmailOctopusService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ILogger<EmailOctopusService> _logger;

    public EmailOctopusService(
        HttpClient http,
        IConfiguration config,
        ILogger<EmailOctopusService> logger)
    {
        _http = http;
        _config = config;
        _logger = logger;
    }

    public async Task AddContactAsync(ContactForm form)
    {
        var listId = _config["EmailOctopus:ListId"];
        var apiKey = _config["EmailOctopus:ApiKey"];

        var url = $"https://api.emailoctopus.com/lists/{listId}/contacts";

        var payload = new
        {
            email_address = form.Email,
            fields = new
            {
                FirstName = form.Name,
                Company = form.Company
            },
            status = "subscribed"
        };

        var json = JsonSerializer.Serialize(payload);

        _logger.LogInformation("EmailOctopus payload: {Payload}", json);

        var request = new HttpRequestMessage(HttpMethod.Post, url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        request.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        var response = await _http.SendAsync(request);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(
                "EmailOctopus success: {Response}",
                responseContent);
        }
        else
        {
            _logger.LogWarning(
                "EmailOctopus failed: {Status} {Response}",
                response.StatusCode,
                responseContent);
        }
    }
}