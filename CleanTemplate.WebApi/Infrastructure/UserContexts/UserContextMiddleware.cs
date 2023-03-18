using System.IdentityModel.Tokens.Jwt;
using CleanTemplate.Application.Infrastructure.UserContexts;
using CleanTemplate.Domain.Enums;

namespace CleanTemplate.WebApi.Infrastructure.UserContexts;
public class UserContextMiddleware
{
    private readonly RequestDelegate next;

    public UserContextMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context, UserContext userContext)
    {
        var token = context.Request?.Headers?.Authorization.FirstOrDefault()?.Split()[1];

        if (token is not null)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var email = jwt.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Email).Value;
            var role = jwt.Claims.First(claim => claim.Type == "role").Value;
            userContext.Email = email;
            userContext.Role = Enum.Parse<RoleEnum>(role);
        }

        await next(context);
    }
}
