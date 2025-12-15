namespace GitHubSearchApi.Services;

public class AuthenticationService
{
    private readonly Dictionary<string, string> _validCredentials = new()
    {
        { "admin", "admin#pass#" },
        { "user1", "password123" },
        { "user2", "secret@2024" }
    };

    public bool ValidateCredentials(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        return _validCredentials.TryGetValue(username, out var validPassword) &&
               password.Equals(validPassword, StringComparison.Ordinal);
    }
}
