using CleanTemplate.Application.Infrastructure.Exceptions;
using CleanTemplate.Application.Infrastructure.UserContexts;
using CleanTemplate.Persistence;
using CleanTemplate.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanTemplate.Application.Users;

public class TicketsService
{
    private readonly MyDbContext context;

    public TicketsService(MyDbContext context)
    {
        this.context = context;
    }

    public async Task<UserModel> GetById(string userId, CancellationToken cancellationToken = default)
    {
        var user = await context.Users
            .Where(e => e.Id == userId)
            .Select(e => new UserModel
            {
                Id = e.Id,
                Email = e.Email,
                Role = e.Role,
                Balance = e.BalanceSeconds,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            throw new ValidationException($"User with id {user} does not exist.");
        }

        return user;
    }
}