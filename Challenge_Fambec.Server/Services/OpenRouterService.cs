using Challenge_Fambec.Shared.Models.DTOs;
using Challenge_Fambec.Shared.Models.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;

namespace Challenge_Fambec.Server.Services;

/// <summary>
/// Service to interact with the OpenRouter API for generating product summaries
/// </summary>
public class OpenRouterService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenRouterService> _logger;
    private readonly string _apiKey;

    // Available free models in order of preference
    private static readonly string[] FreeModels = {
        "deepseek/deepseek-chat-v3-0324:free",
        "deepseek/deepseek-chat:free",
        "mistralai/mistral-7b-instruct:free",
        "huggingface/zephyr-7b-beta:free",
        "openchat/openchat-7b:free",
        "meta-llama/llama-3.2-3b-instruct:free",
        "microsoft/phi-3-mini-128k-instruct:free"
    };

    // Default parameters for the API
    private static readonly object DefaultParams = new
    {
        temperature = 0.7,    // Controls creativity
        max_tokens = 500,     // Maximum number of tokens in response
        top_p = 0.9          // Controls response diversity
    };

    public OpenRouterService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenRouterService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        // Get API key from environment variable only
        _apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY") 
                  ?? throw new InvalidOperationException(
                      "OPENROUTER_API_KEY environment variable not set. Please configure it before running the application.");
        
        _httpClient.BaseAddress = new Uri("https://openrouter.ai/api/v1/");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    /// <summary>
    /// Generate AI summary for the provided products
    /// </summary>
    /// <param name="products">List of products to analyze</param>
    /// <returns>AI-generated summary response</returns>
    public async Task<ProductSummaryResponse> GenerateSummaryAsync(List<Product> products)
    {
        try
        {
            // Return message if there are no products to summarize
            if (!products.Any())
            {
                return new ProductSummaryResponse
                {
                    Success = false,
                    ErrorMessage = "No products available to generate summary",
                    TotalProducts = 0
                };
            }

            _logger.LogInformation("Generating summary for {Count} products", products.Count);

            // Try different models if the first one fails
            return await AttemptWithFallbackAsync(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenRouter Service Error: {Message}", ex.Message);
            return new ProductSummaryResponse
            {
                Success = false,
                ErrorMessage = $"Connection error: {ex.Message}",
                TotalProducts = products.Count
            };
        }
    }

    /// <summary>
    /// Attempts to generate summary with fallback to different models
    /// </summary>
    private async Task<ProductSummaryResponse> AttemptWithFallbackAsync(List<Product> products)
    {
        foreach (var model in FreeModels)
        {
            try
            {
                _logger.LogInformation("Trying model: {Model}", model);
                
                var requestBody = CreateRequestBody(model, products);
                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("chat/completions", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("OpenRouter API response: {Response}", responseContent);
                    
                    var apiResponse = JsonSerializer.Deserialize<OpenRouterResponse>(responseContent);
                    
                    var summary = apiResponse?.Choices?.FirstOrDefault()?.Message?.Content;
                    
                    if (!string.IsNullOrEmpty(summary))
                    {
                        _logger.LogInformation("Successfully extracted summary with {Length} characters", summary.Length);
                        return new ProductSummaryResponse
                        {
                            Success = true,
                            Summary = summary,
                            ModelUsed = model,
                            TotalProducts = products.Count
                        };
                    }
                    else
                    {
                        _logger.LogWarning("Summary extraction failed - content is null or empty");
                    }
                }
                else
                {
                    _logger.LogError("Model {Model} failed: {StatusCode} - {ReasonPhrase}", 
                        model, response.StatusCode, response.ReasonPhrase);
                    
                    // Log response body for debugging
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Response body: {ResponseBody}", errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error with model {Model}: {Message}", model, ex.Message);
            }
        }

        // If all models failed
        return new ProductSummaryResponse
        {
            Success = false,
            ErrorMessage = "All AI models failed to generate summary",
            TotalProducts = products.Count
        };
    }

    /// <summary>
    /// Creates the request body for the OpenRouter API
    /// </summary>
    private object CreateRequestBody(string model, List<Product> products)
    {
        return new
        {
            model,
            messages = new[]
            {
                new { role = "system", content = GetSystemMessage() },
                new { role = "user", content = CreateUserMessage(products) }
            },
            temperature = 0.7,
            max_tokens = 500,
            top_p = 0.9
        };
    }

    /// <summary>
    /// Creates a detailed user message with the list of products
    /// </summary>
    private string CreateUserMessage(List<Product> products)
    {
        _logger.LogInformation("Total products received: {Count}", products.Count);

        // Categorize products by type
        var productsByType = products.GroupBy(p => p.TipoItem).ToList();
        
        var message = new StringBuilder();
        message.AppendLine($"Product Inventory Analysis - Total: {products.Count} items");
        message.AppendLine();

        // Group products by type
        foreach (var group in productsByType.OrderBy(g => g.Key))
        {
            message.AppendLine($"{group.Key} ({group.Count()} items):");
            
            foreach (var product in group.Take(10)) // Limit to avoid token overflow
            {
                message.AppendLine($"- \"{product.DescrItem}\" (Code: {product.CodItem})");
                if (!string.IsNullOrEmpty(product.CodBarra))
                    message.Append($" [Barcode: {product.CodBarra}]");
                if (!string.IsNullOrEmpty(product.CodNcm))
                    message.Append($" [NCM: {product.CodNcm}]");
                message.AppendLine();
            }
            
            if (group.Count() > 10)
            {
                message.AppendLine($"... and {group.Count() - 10} more items of this type");
            }
            message.AppendLine();
        }

        // Add some analysis context
        message.AppendLine($"Analysis covers {productsByType.Count} different product categories.");
        message.AppendLine("Please provide insights about inventory composition, product diversity, and business recommendations.");

        var finalMessage = message.ToString();
        _logger.LogInformation("User message created with {Length} characters", finalMessage.Length);
        
        return finalMessage;
    }

    /// <summary>
    /// Defines the system message to guide the AI's response
    /// </summary>
    private string GetSystemMessage()
    {
        return @"You are a business analyst that analyzes product inventories and creates summaries in plain text.

IMPORTANT: Do not use markdown formatting (**bold**, *italic*, etc). Use only plain text.

FUNDAMENTAL RULE: ALWAYS mention products by their EXACT name as shown in the list. NEVER use generic terms. Use the real product description in quotes.

Analyze the provided product inventory and create a summary following EXACTLY this structure:

1. INVENTORY OVERVIEW: Mention the total products and how they are distributed by category
2. KEY INSIGHTS: Highlight important patterns, data quality issues, or business observations using the real product names
3. RECOMMENDATIONS: Specifically suggest business actions using the EXACT product names in quotes. Example: Consider expanding the ""Mechanical Keyboard Gaming"" product line.
4. CONCLUSION: End with actionable next steps for inventory management

Important rules:
- Use only plain text, no markdown formatting
- Be concise (maximum 4 short paragraphs)
- Use emojis only at the beginning of each section for highlighting
- ALWAYS cite the exact product description in quotes
- Always explain the reason for recommendations (market opportunity, data quality, inventory balance)
- Maintain a professional and business-focused tone

If there are no products, suggest starting to add inventory items.";
    }
    }

    /// <summary>
    /// Response model for OpenRouter API
    /// </summary>
    internal class OpenRouterResponse
    {
        [JsonPropertyName("choices")]
        public Choice[]? Choices { get; set; }
    }

    internal class Choice
    {
        [JsonPropertyName("message")]
        public Message? Message { get; set; }
    }

    internal class Message
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }
