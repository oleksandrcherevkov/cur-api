using CleanTemplate.Application.Infrastructure.Exceptions;
using CleanTemplate.Application.Users.Models;
using CleanTemplate.Persistence;
using CleanTemplate.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanTemplate.Application.Users;

public class UsersService
{
    private readonly MyDbContext context;

    public UsersService(MyDbContext context)
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

    public async Task<UserModel> AddToBalanceId(AddToBalanceModel toAdd, CancellationToken cancellationToken = default)
    {
        var user = await context.Users
            .Where(e => e.Id == toAdd.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            throw new ValidationException($"User with id {user} does not exist.");
        }

        user.BalanceSeconds += toAdd.Amount;

        context.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        var returnModel = new UserModel
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role,
            Balance = user.BalanceSeconds,
        };

        return returnModel;
    }
}