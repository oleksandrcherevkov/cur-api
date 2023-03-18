namespace CleanTemplate.WebApi.Infrastructure.Jwt.Options;
public class JwtOptions
{
    public static readonly string Section = "Jwt";
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public int ExpirationTime { get; set; }
    public string Secret { get; set; }
}
