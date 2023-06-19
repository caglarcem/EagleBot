using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace EagleRock.Gateway
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IConfiguration _configuration;

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IConfiguration configuration)
        : base(options, logger, encoder, clock)
        {
            _configuration = configuration;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization header");
            }

            string authHeader = Request.Headers["Authorization"];
            if (authHeader == null || !authHeader.StartsWith("Basic "))
            {
                return AuthenticateResult.Fail("Invalid Authorization header");
            }

            string encodedCredentials = authHeader.Substring(6).Trim();
            byte[] decodedBytes = Convert.FromBase64String(encodedCredentials);
            string decodedCredentials = Encoding.UTF8.GetString(decodedBytes);
            string[] parts = decodedCredentials.Split(':', 2);

            string username = parts[0];
            string password = parts[1];

            bool isValid = ValidateCredentials(username, password, _configuration);
            if (!isValid)
            {
                return AuthenticateResult.Fail("Invalid username or password");
            }

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, username),
            new Claim(ClaimTypes.Name, username)
        };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        bool ValidateCredentials(string usernameInput, string passwordInput, IConfiguration configuration)
        {
            var actualUsername = configuration["ApiAuth:Username"];
            var actualPassword = configuration["ApiAuth:Password"];

            return usernameInput == actualUsername && passwordInput == actualPassword;
        }
    }
}
