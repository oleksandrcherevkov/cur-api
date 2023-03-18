using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CleanTemplate.WebApi.Infrastructure.Jwt.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CleanTemplate.WebApi.Infrastructure.Jwt;
public class JwtGenerator : IJwtGenerator
{
    private readonly JwtOptions options;
    public JwtGenerator(IOptions<JwtOptions> options)
    {
        this.options = options.Value;
    }

    public JwtSecurityToken Generate(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret));

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            expires: DateTime.Now.AddHours(options.ExpirationTime),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        return token;
    }
}
