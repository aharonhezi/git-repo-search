using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GitHubSearchApi.Models;
using GitHubSearchApi.Services;

namespace GitHubSearchApi.Middleware;

public class SessionMiddleware
{
    private readonly RequestDelegate _next;
    private const string SessionCookieName = "sid";
    private const string SessionContextKey = "Session";

    public SessionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, SessionStore sessionStore)
    {
        string sessionId;
        var hasCookie = context.Request.Cookies.ContainsKey(SessionCookieName);
        var cookieValue = hasCookie ? context.Request.Cookies[SessionCookieName] : null;
        
        if (hasCookie)
        {
            sessionId = cookieValue!;
        }
        else
        {
            sessionId = Guid.NewGuid().ToString();
            context.Response.Cookies.Append(SessionCookieName, sessionId, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Set to true in production with HTTPS
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(1),
                Path = "/"
            });
        }

        // Extract username from JWT token (middleware runs before authentication)
        string username = string.Empty;
        
        if (context.User.Identity?.IsAuthenticated == true)
        {
            username = context.User.FindFirstValue(ClaimTypes.Name) 
                ?? context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? string.Empty;
        }
        
        if (string.IsNullOrEmpty(username))
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadJwtToken(token);
                    username = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name || c.Type == "unique_name" || c.Type == "sub")?.Value ?? string.Empty;
                }
                catch
                {
                    // Invalid token, ignore
                }
            }
        }

        // Get or create session - authenticated users get sessions keyed by username for isolation
        SessionData session;
        if (!string.IsNullOrEmpty(username))
        {
            session = sessionStore.GetOrCreateSession(sessionId, username);
        }
        else
        {
            var existingSession = sessionStore.GetSession(sessionId, string.Empty);
            if (existingSession != null)
            {
                session = existingSession;
            }
            else
            {
                session = sessionStore.GetOrCreateSession(sessionId, string.Empty);
            }
        }

        context.Items[SessionContextKey] = session;
        await _next(context);
    }
}

public static class HttpContextExtensions
{
    public static SessionData GetSession(this HttpContext context)
    {
        if (context.Items.TryGetValue("Session", out var session) && session is SessionData sessionData)
        {
            return sessionData;
        }

        throw new InvalidOperationException("Session not found in HttpContext. Ensure SessionMiddleware is registered.");
    }
}

