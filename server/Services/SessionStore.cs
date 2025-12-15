using System.Collections.Concurrent;
using GitHubSearchApi.Models;

namespace GitHubSearchApi.Services;

public class SessionStore
{
    private readonly ConcurrentDictionary<string, SessionData> _sessions = new();
    private readonly int _expirationMinutes;
    private readonly Timer _cleanupTimer;

    public SessionStore(IConfiguration configuration)
    {
        _expirationMinutes = int.Parse(configuration["Session:ExpirationMinutes"] ?? "30");
        _cleanupTimer = new Timer(CleanupExpiredSessions, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }

    private string GetSessionKey(string sessionId, string username)
    {
        return !string.IsNullOrEmpty(username) ? $"user:{username}" : $"session:{sessionId}";
    }

    public SessionData GetOrCreateSession(string sessionId, string username)
    {
        var sessionKey = GetSessionKey(sessionId, username);
        
        if (!string.IsNullOrEmpty(username))
        {
            var anonymousKey = GetSessionKey(sessionId, string.Empty);
            if (_sessions.TryGetValue(anonymousKey, out var anonymousSession) && 
                string.IsNullOrEmpty(anonymousSession.Username))
            {
                var authenticatedSession = new SessionData 
                { 
                    Username = username,
                    Bookmarks = new List<GitHubRepoDto>(anonymousSession.Bookmarks),
                    LastAccess = DateTime.UtcNow
                };
                _sessions.TryRemove(anonymousKey, out _);
                _sessions.TryAdd(sessionKey, authenticatedSession);
                return authenticatedSession;
            }
        }
        
        return _sessions.AddOrUpdate(
            sessionKey,
            new SessionData { Username = username },
            (key, existing) =>
            {
                if (existing.Username != username)
                {
                    existing.Username = username;
                }
                existing.UpdateAccessTime();
                return existing;
            });
    }

    public SessionData? GetSession(string sessionId, string username = "")
    {
        var sessionKey = GetSessionKey(sessionId, username);
        
        if (_sessions.TryGetValue(sessionKey, out var session))
        {
            if (IsExpired(session))
            {
                _sessions.TryRemove(sessionKey, out _);
                return null;
            }

            session.UpdateAccessTime();
            return session;
        }

        return null;
    }

    public void RemoveSession(string sessionId)
    {
        _sessions.TryRemove(sessionId, out _);
    }

    private bool IsExpired(SessionData session)
    {
        return DateTime.UtcNow - session.LastAccess > TimeSpan.FromMinutes(_expirationMinutes);
    }

    private void CleanupExpiredSessions(object? state)
    {
        var expiredKeys = _sessions
            .Where(kvp => IsExpired(kvp.Value))
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            _sessions.TryRemove(key, out _);
        }
    }
}

