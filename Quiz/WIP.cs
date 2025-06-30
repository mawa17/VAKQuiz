@using Microsoft.Net.Http.Headers
@code {
    [Parameter, EditorRequired] public string Username { get; init; } = null!;
    [Parameter, EditorRequired] public string Password { get; init; } = null!;
    [Parameter] public RenderFragment? ChildContent { get; init; }
    [Parameter] public string Realm { get; init; } = "AuthRealm";
    [Inject] public IHttpContextAccessor accessor { get; init; } = default!;
    [Inject] public NavigationManager NavManager { get; init; } = default!;
    private HttpContext Context => accessor.HttpContext!;
    private bool IsSessionAuthenticated => Boolean.TryParse(Context?.Session?.GetString($"{Realm}-IsAuthenticated") ?? String.Empty, out bool result);
    private bool _isAuthenticated = false;
    private bool IsAuthenticated
    {
        get
        {

            if (IsSessionAuthenticated && !_isAuthenticated)
            {
                _isAuthenticated = true;
                Context.Session.Remove($"{Realm}-IsAuthenticated");
            }
            return _isAuthenticated;
        }
    }
    private bool IsAuthorized
    {
        get
        {
            IHeaderDictionary headers = Context.Request.Headers;
            var credentials = ParseAuthorizationHeader(headers.Authorization.ToString());
            return headers.ContainsKey(HeaderNames.Authorization) &&
                    credentials != null &&
                    ValidateUser(credentials.Value.username, credentials.Value.password);
        }
    }
    private string SecureArea
    {
        get
        {
            if (String.IsNullOrEmpty(Context.Session.GetString(Realm)))
                Context.Session.SetString(Realm, $"Secure-Area-{Guid.NewGuid()}");
            return (Context.Session.GetString(Realm)!);
        }
    }

    private void DisplayInfo(string? info = null) 
    {
#if DEBUG
    if (!String.IsNullOrEmpty(info)) {
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(info);
    Console.ResetColor();
    }
    Console.WriteLine("".PadRight(32, '-'));
    Console.WriteLine($"GET\tAUTH\t{(ParseAuthorizationHeader(Context.Request.Headers.Authorization)?.ToString() ?? "NULL")} -> ({Context.Request.Headers.Authorization})");
    Console.WriteLine($"GET\tWWW\t{Context.Request.Headers.WWWAuthenticate}");
    Console.WriteLine($"POST\tAUTH\t{(ParseAuthorizationHeader(Context.Response.Headers.Authorization)?.ToString() ?? "NULL")} -> ({Context.Response.Headers.Authorization})");
    Console.WriteLine($"POST\tWWW\t{Context.Response.Headers.WWWAuthenticate}");
    Console.WriteLine($"SESS\tAUTH\t{Context.Session.GetString($"{Realm}-IsAuthenticated") ?? "false"}");
    Console.WriteLine("".PadRight(32, '-'));
    Console.WriteLine($"IsAuthenticated {IsAuthenticated}");
    Console.WriteLine($"IsAuthorized {IsAuthorized}");
    Console.WriteLine("".PadRight(32, '-'));
#endif
    }

    protected override void OnInitialized()
    {
        if (accessor?.HttpContext == null)
            throw new NullReferenceException($"{nameof(accessor)} service missing!");

        if (String.IsNullOrEmpty(Username) || String.IsNullOrEmpty(Password))
            throw new ArgumentNullException($"{nameof(Username)} or {nameof(Password)} cannot be NULL!");

        DisplayInfo();

        Authorize();
    }

    private void Authorize()
    {
        if (!IsAuthenticated)
        {
            DisplayInfo("NEED Authorization");
            RequestAuthorization();
            return;
        }

        _isAuthenticated = true;
        Context.Session.SetString($"{Realm}-IsAuthenticated", "true");
        DisplayInfo("APPROVED Authorization");
    }


    private void RequestAuthorization()
    {
        if (!Context.Response.HasStarted)
        {
            Context.Response.Headers.CacheControl = "no-store, no-cache, must-revalidate";
            Context.Response.Headers.Pragma = "no-cache";
            Context.Response.Headers.Expires = "0";

            Context.Response.Headers.WWWAuthenticate = $"Basic realm=\"{SecureArea}\"";
            Context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            DisplayInfo("Send Headers");
        }
    }

    private (string username, string password)? ParseAuthorizationHeader(string authHeader)
    {
        if (String.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase)) return null;

        var encodedCredentials = authHeader["Basic ".Length..].Trim();
        var credentialBytes = Convert.FromBase64String(encodedCredentials);
        var credentials = System.Text.Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

        return credentials.Length == 2 ? (credentials[0], credentials[1]) : null;
    }

    private void Logout() 
    {
        _isAuthenticated = false;
        Context.Session.Clear();
    }


    private bool ValidateUser(string username, string password) =>
        String.Equals(username, this.Username, StringComparison.Ordinal) &&
        String.Equals(password, this.Password, StringComparison.Ordinal);
}

@if (IsAuthenticated)
{
    <button @onclick="Logout" type="button" class="btn btn-outline-danger btn-sm">Logout</button>
    @ChildContent
}
else
{
    <h1 class="d-flex justify-content-center align-items-center mt-5">401 Unauthorized - Authentication Required</h1>
}
