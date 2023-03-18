using CleanTemplate.Domain.Enums;

namespace CleanTemplate.WebApi.Models;
public class RegisterModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public RoleEnum Role { get; set; }
}
