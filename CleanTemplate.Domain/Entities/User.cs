using System.ComponentModel.DataAnnotations;
using CleanTemplate.Auth.Models;
using CleanTemplate.Domain.Enums;

namespace CleanTemplate.Domain.Entities;

public class User : UserModel<RoleEnum>
{
    public User()
        : base()
    {
    }
    public User(string email, string password, RoleEnum role)
        : base(email, password, role)
    {
    }
}