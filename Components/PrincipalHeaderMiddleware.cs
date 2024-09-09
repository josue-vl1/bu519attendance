using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

public class PrincipalHeaderMiddleware
{
    private readonly RequestDelegate _next;

    public PrincipalHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-MS-CLIENT-PRINCIPAL-NAME", out var principalName))
        {
            var claims = new[] { new Claim(ClaimTypes.Name, principalName) };
            var identity = new ClaimsIdentity(claims, "Header");
            var principal = new ClaimsPrincipal(identity);
            context.User = principal;
        }

        await _next(context);
    }
}