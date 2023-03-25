using CleanTemplate.Domain.Enums;

namespace CleanTemplate.WebApi.Models;

public class UserModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public RoleEnum Role { get; set; }
    public double Balance { get; set; }
}