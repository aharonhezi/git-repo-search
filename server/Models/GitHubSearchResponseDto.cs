namespace GitHubSearchApi.Models;

public class GitHubSearchResponseDto
{
    public int TotalCount { get; set; }
    public List<GitHubRepoDto> Items { get; set; } = new();
}