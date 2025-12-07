using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SecurityAPI.Services;

// Simple authentication handler that validates a single hardcoded token in the Authorization header.
public class StaticTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string StaticToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkZW1vLXVzZXIiLCJpc3MiOiJteS1hcGkiLCJpYXQiOjE3MDAwMDAwMDAsImV4cCI6MTgwMDAwMDAwMCwicm9sZXMiOlsiVXNlciIsIkFkbWluIl19.dQ7fHc0uZQ1jR6fBzj2xv3hVtcv7h-dXJt9vB3k5rX8";

    public StaticTokenAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing or invalid Authorization header"));
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        if (!string.Equals(token, StaticToken, StringComparison.Ordinal))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid static token"));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "static"),
            new Claim(ClaimTypes.Name, "static-access"),
            new Claim(ClaimTypes.Role, "Admin")
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
