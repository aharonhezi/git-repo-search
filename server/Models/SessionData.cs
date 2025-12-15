namespace GitHubSearchApi.Models;

public class SessionData
{
    public string Username { get; set; } = string.Empty;
    public List<GitHubRepoDto> Bookmarks { get; set; } = new();
    public DateTime LastAccess { get; set; } = DateTime.UtcNow;

    public void UpdateAccessTime()
    {
        LastAccess = DateTime.UtcNow;
    }
}