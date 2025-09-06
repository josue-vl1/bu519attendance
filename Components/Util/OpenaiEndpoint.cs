using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Azure.Core;
using static System.Environment;
using System.Text.Json;
using System.Text;
using OpenAI.Chat;


public class OpenaiEndpoint
{
    public string URL { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    private readonly ILogger _logger;
    private static readonly HttpClient _httpClient = new HttpClient();

    public OpenaiEndpoint(ILogger logger)
    {
        _logger = logger;

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

    public async Task<string> PostAsync2(string message)
    {
        var credential = new DefaultAzureCredential();

        // Initialize the AzureOpenAIClient
        var azureClient = new AzureOpenAIClient(new Uri(URL), credential);

        // Initialize the ChatClient with the specified deployment name
        ChatClient chatClient = azureClient.GetChatClient(Model);

        // Create a list of chat messages
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(message),
        };

        // Create chat completion options
        var options = new ChatCompletionOptions
        {
            Temperature = (float)0.7,
            MaxOutputTokenCount = 4000,

            TopP = (float)0.95,
            FrequencyPenalty = (float)0,
            PresencePenalty = (float)0
        };

        try
        {
            // Create the chat completion request
            ChatCompletion completion = await chatClient.CompleteChatAsync(messages, options);

            // Print the response
            if (completion != null)
            {
                return  JsonSerializer.Serialize(completion, new JsonSerializerOptions() { WriteIndented = true });
                //return JsonSerializer.Deserialize<JsonElement>(response);
            }
            else
            {
                Console.WriteLine("No response received.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        return "nothing";

    }

}