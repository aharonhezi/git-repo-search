using GitHubSearchApi.Middleware;
using GitHubSearchApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GitHubSearchApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookmarksController : ControllerBase
{

    [HttpGet]
    public IActionResult GetBookmarks()
    {
        var session = HttpContext.GetSession();
        var username = User.Identity?.Name;

        if (string.IsNullOrEmpty(username) || session.Username != username)
        {
            return Unauthorized(new { message = "Session does not match authenticated user" });
        }

        return Ok(session.Bookmarks);
    }

    [HttpPost]
    public IActionResult AddBookmark([FromBody] GitHubRepoDto repo)
    {
        if (repo == null || repo.Id == 0)
        {
            return BadRequest(new { message = "Invalid repository data" });
        }

        var session = HttpContext.GetSession();
        var username = User.Identity?.Name;

        if (string.IsNullOrEmpty(username) || session.Username != username)
        {
            return Unauthorized(new { message = "Session does not match authenticated user" });
        }

        if (session.Bookmarks.Any(b => b.Id == repo.Id))
        {
            return Conflict(new { message = "Repository is already bookmarked" });
        }

        session.Bookmarks.Add(repo);
        session.UpdateAccessTime();

        return Ok(new { message = "Bookmark added successfully" });
    }

    [HttpDelete("{repoId}")]
    public IActionResult RemoveBookmark(long repoId)
    {
        var session = HttpContext.GetSession();
        var username = User.Identity?.Name;

        if (string.IsNullOrEmpty(username) || session.Username != username)
        {
            return Unauthorized(new { message = "Session does not match authenticated user" });
        }

        var bookmark = session.Bookmarks.FirstOrDefault(b => b.Id == repoId);
        if (bookmark == null)
        {
            return NotFound(new { message = "Bookmark not found" });
        }

        session.Bookmarks.Remove(bookmark);
        session.UpdateAccessTime();

        return Ok(new { message = "Bookmark removed successfully" });
    }
}

