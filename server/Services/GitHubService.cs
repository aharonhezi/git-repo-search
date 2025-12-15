using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using GitHubSearchApi.Models;

namespace GitHubSearchApi.Services;

public class GitHubService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GitHubService> _logger;

    public GitHubService(HttpClient httpClient, ILogger<GitHubService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<GitHubSearchResponseDto> SearchRepositoriesAsync(string query, int perPage = 30, int page = 1)
    {
        try
        {
            var url = $"/search/repositories?q={Uri.EscapeDataString(query)}&per_page={perPage}&page={page}";
            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                var rateLimitRemaining = response.Headers.Contains("X-RateLimit-Remaining")
                    ? response.Headers.GetValues("X-RateLimit-Remaining").FirstOrDefault()
                    : "unknown";

                if (rateLimitRemaining == "0")
                {
                    _logger.LogWarning("GitHub API rate limit exceeded");
                    throw new HttpRequestException(
                        "GitHub API rate limit exceeded. Please try again later.",
                        null,
                        HttpStatusCode.TooManyRequests);
                }
            }

            response.EnsureSuccessStatusCode();

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var githubResponse = await response.Content.ReadFromJsonAsync<GitHubApiResponse>(jsonOptions);
            
            if (githubResponse == null)
            {
                throw new HttpRequestException("Failed to parse GitHub API response");
            }

            return new GitHubSearchResponseDto
            {
                TotalCount = githubResponse.TotalCount,
                Items = githubResponse.Items?.Select(item => new GitHubRepoDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    FullName = item.FullName,
                    HtmlUrl = item.HtmlUrl,
                    Description = item.Description,
                    StargazersCount = item.StargazersCount,
                    Owner = new OwnerDto
                    {
                        Login = item.Owner?.Login ?? string.Empty,
                        AvatarUrl = item.Owner?.AvatarUrl ?? string.Empty
                    }
                }).ToList() ?? new List<GitHubRepoDto>()
            };
        }
        catch (HttpRequestException ex) when (ex.Data.Contains("StatusCode") && 
                                              ex.Data["StatusCode"] is HttpStatusCode statusCode &&
                                              statusCode == HttpStatusCode.TooManyRequests)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error calling GitHub API");
            throw new HttpRequestException("Failed to search GitHub repositories. Please try again later.", ex);
        }
    }

    private class GitHubApiResponse
    {
        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }
        
        [JsonPropertyName("items")]
        public List<GitHubApiItem>? Items { get; set; }
    }

    private class GitHubApiItem
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("full_name")]
        public string FullName { get; set; } = string.Empty;
        
        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = string.Empty;
        
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        
        [JsonPropertyName("stargazers_count")]
        public int StargazersCount { get; set; }
        
        [JsonPropertyName("owner")]
        public GitHubApiOwner? Owner { get; set; }
    }

    private class GitHubApiOwner
    {
        [JsonPropertyName("login")]
        public string Login { get; set; } = string.Empty;
        
        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; } = string.Empty;
    }
}

