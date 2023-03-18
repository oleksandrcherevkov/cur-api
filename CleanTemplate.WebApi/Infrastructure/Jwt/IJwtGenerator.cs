using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CleanTemplate.WebApi.Infrastructure.Jwt;

public interface IJwtGenerator
{
    JwtSecurityToken Generate(List<Claim> authClaims);
}
