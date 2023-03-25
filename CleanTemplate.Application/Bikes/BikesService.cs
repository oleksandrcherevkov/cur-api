using CleanTemplate.Application.Bikes.Models;
using CleanTemplate.Application.Infrastructure.Exceptions;
using CleanTemplate.Application.Infrastructure.Topology;
using CleanTemplate.Application.Infrastructure.UserContexts;
using CleanTemplate.Application.Stations.Models;
using CleanTemplate.Application.Tickets;
using CleanTemplate.Domain.Entities;
using CleanTemplate.Domain.Enums;
using CleanTemplate.Persistence;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace CleanTemplate.Application.Bikes;

public interface IBikesService
{
    Task<BikeModel> ActivateBikeAsync(string bikeId, CancellationToken cancellationToken = default);
    Task<BikeModel> CreateBikeAsync(CancellationToken cancellationToken = default);
    Task<List<BikeModel>> GetActiveBikesInRadiusAsync(double longtitude, double latitude, double radius, CancellationToken cancellationToken = default);
    Task<BikeModel> GetBikeByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<BikeModel> RepairBikeAsync(string bikeId, CancellationToken cancellationToken = default);
    Task<BikeModel> WasteBikeAsync(string bikeId, CancellationToken cancellationToken = default);
}

public class BikesService : IBikesService
{
    private readonly MyDbContext context;
    private readonly UserContext userContext;
    private readonly TicketsService ticketsService;

    public BikesService(MyDbContext context, UserContext userContext, TicketsService ticketsService)
    {
        this.context = context;
        this.userContext = userContext;
        this.ticketsService = ticketsService;
    }

    public async Task<BikeModel> GetBikeByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var bike = await context.Bikes
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new BikeModel
            {
                Id = e.Id,
                Latitude = e.Location.X,
                Longtitude = e.Location.Y,
                Status = e.Status,
                Station = e.CurrentStationId != null ?
                    new StationModel
                    {
                        Id = e.CurrentStation!.Id,
                        Name = e.CurrentStation.Name,
                        Latitude = e.CurrentStation.Location.X,
                        Longtitude = e.CurrentStation.Location.Y,
                    }
                    : null,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (bike is null)
        {
            throw new NotFoundException($"Bike with Id {id} does not exist.");
        }

        return bike;
    }

    public async Task<List<BikeModel>> GetActiveBikesInRadiusAsync(double longtitude, double latitude, double radius, CancellationToken cancellationToken = default)
    {
        var location = new Point(longtitude, latitude) { SRID = 4326 };
        var bikes = await context.Bikes
            .AsNoTracking()
            .Where(e => e.Location.Distance(location) <= radius)
            .Where(e => e.Status == BikeStatusEnum.Active)
            .Select(e => new BikeModel
            {
                Id = e.Id,
                Latitude = e.Location.X,
                Longtitude = e.Location.Y,
                Status = e.Status,
            })
            .ToListAsync(cancellationToken);

        return bikes;
    }

    public async Task<BikeModel> CreateBikeAsync(CancellationToken cancellationToken = default)
    {
        var random = new Random();
        var longtitude = random.NextDouble() *
            (TopologyConstants.LongtitudeBordersUkraine.max - TopologyConstants.LongtitudeBordersUkraine.min) +
            TopologyConstants.LongtitudeBordersUkraine.min;
        var latitude = random.NextDouble() *
            (TopologyConstants.LatitudeBordersUkraine.max - TopologyConstants.LatitudeBordersUkraine.min) +
            TopologyConstants.LatitudeBordersUkraine.min;
        var bike = new Bike();
        bike.Location = new Point(longtitude, latitude) { SRID = 4326 };
        context.Add(bike);
        await context.SaveChangesAsync(cancellationToken);
        return new BikeModel(bike);
    }

    public async Task<BikeModel> ActivateBikeAsync(string bikeId, CancellationToken cancellationToken = default)
    {
        var bike = await context.Bikes.FirstOrDefaultAsync(e => e.Id == bikeId, cancellationToken);
        if (bike is null)
        {
            throw new NotFoundException($"Bike with Id {bikeId} does not exist.");
        }
        if (bike.Status != BikeStatusEnum.Repairing)
        {
            throw new ValidationException($"Bike with Id {bikeId} can not be activated. Current status: {bike.Status}.");
        }
        // find nearest station
        var stationId = await context.Stations
            .OrderBy(e => e.Location.Distance(bike.Location))
            .Select(e => e.Id)
            .FirstAsync(cancellationToken);
        bike.Status = BikeStatusEnum.Active;
        bike.CurrentStationId = stationId;
        context.Update(bike);
        await context.SaveChangesAsync(cancellationToken);
        return new BikeModel(bike);
    }

    public async Task<BikeModel> RepairBikeAsync(string bikeId, CancellationToken cancellationToken = default)
    {
        var bike = await context.Bikes.FirstOrDefaultAsync(e => e.Id == bikeId, cancellationToken);
        if (bike is null)
        {
            throw new NotFoundException($"Bike with Id {bikeId} does not exist.");
        }
        if (bike.Status != BikeStatusEnum.Active || bike.Status != BikeStatusEnum.Rented)
        {
            throw new ValidationException($"Bike with Id {bikeId} can not be set on repairing. Current status: {bike.Status}.");
        }

        if (bike.Status == BikeStatusEnum.Rented)
        {
            await ticketsService.CloseTicketAsync(bike.Id, cancellationToken);
        }

        bike.Status = BikeStatusEnum.Repairing;
        bike.CurrentStationId = null;
        context.Update(bike);
        await context.SaveChangesAsync(cancellationToken);
        return new BikeModel(bike);
    }

    public async Task<BikeModel> WasteBikeAsync(string bikeId, CancellationToken cancellationToken = default)
    {
        var bike = await context.Bikes.FirstOrDefaultAsync(e => e.Id == bikeId, cancellationToken);
        if (bike is null)
        {
            throw new NotFoundException($"Bike with Id {bikeId} does not exist.");
        }
        if (bike.Status != BikeStatusEnum.Repairing)
        {
            throw new ValidationException($"Bike with Id {bikeId} can not disposed. Current status: {bike.Status}.");
        }
        bike.Status = BikeStatusEnum.Broken;
        bike.CurrentStationId = null;
        context.Update(bike);
        await context.SaveChangesAsync(cancellationToken);
        return new BikeModel(bike);
    }
}