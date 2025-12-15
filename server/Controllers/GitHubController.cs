using System.Net;
using System.Net.Http;
using GitHubSearchApi.Models;
using GitHubSearchApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GitHubSearchApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GitHubController : ControllerBase
{
    private readonly GitHubService _gitHubService;
    private readonly ILogger<GitHubController> _logger;

    public GitHubController(GitHubService gitHubService, ILogger<GitHubController> logger)
    {
        _gitHubService = gitHubService;
        _logger = logger;
    }

    [HttpGet("search")]
    public async Task<ActionResult<GitHubSearchResponseDto>> Search(
        [FromQuery] string query,
        [FromQuery] int perPage = 30,
        [FromQuery] int page = 1)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest(new { message = "Query parameter is required" });
        }

        if (perPage < 1 || perPage > 100)
        {
            perPage = 30;
        }

        if (page < 1)
        {
            page = 1;
        }

        try
        {
            var result = await _gitHubService.SearchRepositoriesAsync(query, perPage, page);
            return Ok(result);
        }
        catch (HttpRequestException ex) when (ex.Data.Contains("StatusCode"))
        {
            if (ex.Data["StatusCode"] is System.Net.HttpStatusCode statusCode &&
                statusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                return StatusCode(429, new { message = "GitHub API rate limit exceeded. Please try again later." });
            }

            _logger.LogError(ex, "Error searching GitHub repositories");
            return StatusCode(500, new { message = "Failed to search repositories. Please try again later." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error searching GitHub repositories");
            return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
        }
    }
}

