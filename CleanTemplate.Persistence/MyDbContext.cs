using CleanTemplate.Auth;
using CleanTemplate.Domain.Entities;
using CleanTemplate.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CleanTemplate.Persistence;

public class MyDbContext : AuthContext<User, RoleEnum>
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }
}
