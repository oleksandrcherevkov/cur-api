using CleanTemplate.Application.Infrastructure.Exceptions;
using CleanTemplate.Application.Infrastructure.UserContexts;
using CleanTemplate.Application.Tickets.Models;
using CleanTemplate.Domain.Entities;
using CleanTemplate.Domain.Enums;
using CleanTemplate.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanTemplate.Application.Tickets;

public class TicketsService
{
    private readonly MyDbContext context;
    private readonly UserContext userContext;

    public TicketsService(MyDbContext context, UserContext userContext)
    {
        this.context = context;
        this.userContext = userContext;
    }

    public async Task<List<TicketModel>> GetMyAsync(CancellationToken cancellationToken = default)
    {
        var tickets = await context.Tickets
            .AsNoTracking()
            .Where(e => e.UserId == userContext.Id)
            .Select(e => new TicketModel
            {
                Id = e.Id,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
            })
            .ToListAsync(cancellationToken);

        return tickets;
    }

    public async Task<TicketModelWrapper> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var wrapper = new TicketModelWrapper();
        var ticket = await context.Tickets
            .AsNoTracking()
            .Where(e => e.UserId == userContext.Id)
            .Where(e => e.EndDate == null)
            .Select(e => new TicketModel
            {
                Id = e.Id,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
            })
            .LastOrDefaultAsync(cancellationToken);
        wrapper.Ticket = ticket;
        return wrapper;
    }

    public async Task OpenTicketAsync(string bikeId, CancellationToken cancellationToken = default)
    {
        var user = await context.Users
            .Where(e => e.Id == userContext.Id)
            .FirstAsync(cancellationToken);

        var bike = await context.Bikes
            .Where(e => e.Id == bikeId)
            .FirstOrDefaultAsync(cancellationToken);

        if (bike == null)
        {
            throw new ValidationException($"Bike with id {bikeId} does not exist.");
        }

        if (bike.Status != BikeStatusEnum.Active)
        {
            throw new ValidationException($"Bike with id {bikeId} can not be rented.");
        }

        if (user.BalanceSeconds <= 0)
        {
            throw new ValidationException($"Your balance: {user.BalanceSeconds}. You can not rent bike.");
        }

        bike.CurrentStationId = null;
        bike.Status = BikeStatusEnum.Rented;
        context.Update(bike);

        var ticket = new Ticket(bike.Id, user.Id, DateTime.UtcNow);
        context.Add(ticket);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task CloseTicketAsync(string? bikeId = null, CancellationToken cancellationToken = default)
    {
        Ticket? ticket = null;

        if (bikeId != null)
        {
            ticket = await context.Tickets
                .Where(e => e.BikeId == bikeId)
                .Where(e => e.UserId == userContext.Id)
                .Where(e => e.EndDate == null)
                .OrderBy(e => e.Id)
                .LastOrDefaultAsync(cancellationToken);
        }
        else
        {
            ticket = await context.Tickets
                .Where(e => e.UserId == userContext.Id)
                .Where(e => e.EndDate == null)
                .LastOrDefaultAsync(cancellationToken);
        }

        if (ticket == null)
        {
            throw new ValidationException("No open ticket.");
        }

        var user = await context.Users
            .Where(e => e.Id == ticket.UserId)
            .FirstAsync(cancellationToken);

        var bike = await context.Bikes
            .Where(e => e.Id == ticket.BikeId)
            .FirstAsync(cancellationToken);

        ticket.SetEndDate(DateTime.UtcNow);
        context.Update(ticket);
        if (ticket.EndDate == null)
        {
            throw new Exception("Ticket end date hasn't saved.");
        }

        // find nearest station
        var stationId = await context.Stations
            .OrderBy(e => e.Location.Distance(bike.Location))
            .Select(e => e.Id)
            .FirstAsync(cancellationToken);
        bike.Status = BikeStatusEnum.Active;
        bike.CurrentStationId = stationId;
        context.Update(bike);

        DateTime endDate = (DateTime)ticket.EndDate;
        user.BalanceSeconds -= (endDate - ticket.StartDate).TotalSeconds;

        await context.SaveChangesAsync(cancellationToken);
    }
}