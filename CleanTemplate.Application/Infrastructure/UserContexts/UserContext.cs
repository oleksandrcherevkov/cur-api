using CleanTemplate.Domain.Enums;

namespace CleanTemplate.Application.Infrastructure.UserContexts;
public class UserContext
{
    public string Email { get; set; }
    public RoleEnum Role { get; set; }
}
