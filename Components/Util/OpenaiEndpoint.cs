using Azure;
using Azure.Identity;
using Azure.Core;
using static System.Environment;
using System.Text.Json;
using System.Text;



public class OpenaiEndpoint
{
    public string URL { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    private readonly ILogger _logger;
    private readonly HttpClient _httpClient;

    public OpenaiEndpoint(ILogger logger)
    {
        _logger = logger;
        _httpClient = new HttpClient();
        URL = Environment.GetEnvironmentVariable("OPENAI_ENDPOINT") ?? string.Empty;
        Model = Environment.GetEnvironmentVariable("OPENAI_DEPLOYMENT") ?? string.Empty;

        if (string.IsNullOrEmpty(URL))
            _logger.LogError("No OPENAI_ENDPOINT environment variable set");
        if (string.IsNullOrEmpty(Model))
            _logger.LogError("No OPENAI_DEPLOYMENT environment variable set");
    }

    public async Task<JsonElement> PostAsync(string message)
    {
        var tokenCredential = new DefaultAzureCredential();
        var token = tokenCredential.GetToken(new TokenRequestContext(new[] { "https://cognitiveservices.azure.com" }));

        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Token);
        var content = new StringContent(message, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{URL}/openai/deployments/{Model}/chat/completions?api-version=2024-02-15-preview", content);
        var responseString = await response.Content.ReadAsStringAsync();
        _logger.LogInformation(responseString);

        return JsonSerializer.Deserialize<JsonElement>(responseString);
    }
}