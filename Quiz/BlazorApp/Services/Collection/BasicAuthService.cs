using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text;

namespace BlazorApp.Services.Collection;

public interface IAuthentication
{
    bool IsAuthenticated(string keyId);
    bool HasRequest(string keyId);
    void MarkAuthenticated(string keyId);
    void Logout(string keyId);
    void Login(string keyId);
}

public interface IAuthorization
{
    bool IsAuthorized(string? authHeader, string user, string pass);
    void RequestAuthorization(HttpContext context, string realm);
}


public interface IBasicAuthService : IAuthentication, IAuthorization
{
    string GenerateKey(string seed);
}

public sealed class BasicAuthService(IDataService dataService) : IBasicAuthService
{
    private readonly IDataService _dataService = dataService;

    public string GenerateKey(string seed)
    {
        var x = new BasicAuthService(default);
        string secureKey = $"{seed}-SecureArea";
        if (!_dataService.Contains(secureKey))
        {
            _dataService.Set(secureKey, $"Secure-Area-{Guid.NewGuid()}");
        }
        return _dataService.Get<string>(secureKey, String.Empty)!;
    }

    public bool HasRequest(string keyId) => _dataService.Get<bool>($"{keyId}-IsRequestSent", false);
    public bool IsAuthenticated(string keyId) => _dataService.Get<bool>($"{keyId}-IsAuthenticated", false);

    public bool IsAuthorized(string? authHeader, string user, string pass)
    {
        if (String.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase)) return false;

        var encoded = authHeader["Basic ".Length..].Trim();
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
        var credentials = decoded.Split(':', 2);

        return credentials.Length == 2 &&
            String.Equals(credentials[0], user, StringComparison.Ordinal) &&
            String.Equals(credentials[1], pass, StringComparison.Ordinal);
    }

    public void Login(string keyId) => _dataService.Set($"{keyId}-IsRequestSent", true);

    public void Logout(string keyId)
    {
        _dataService.Remove($"{keyId}-IsAuthenticated");
        _dataService.Remove($"{keyId}-IsRequestSent");
        _dataService.Remove($"{keyId}-SecureArea");
    }

    public void MarkAuthenticated(string keyId)
    {
        _dataService.Set($"{keyId}-IsAuthenticated", true);
        _dataService.Set($"{keyId}-IsRequestSent", true);
    }

    public void RequestAuthorization(HttpContext context, string realm)
    {
        if (!context.Response.HasStarted)
        {
            context.Response.Headers.CacheControl = "no-store, no-cache, must-revalidate";
            context.Response.Headers.Pragma = "no-cache";
            context.Response.Headers.Expires = "0";
            context.Response.Headers.WWWAuthenticate = $"Basic realm=\"{realm}\"";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }   
    }
}