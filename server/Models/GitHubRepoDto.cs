namespace GitHubSearchApi.Models;

public class GitHubRepoDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string HtmlUrl { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int StargazersCount { get; set; }
    public OwnerDto Owner { get; set; } = new();
}