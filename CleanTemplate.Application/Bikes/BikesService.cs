using CleanTemplate.Application.Bikes.Models;
using CleanTemplate.Application.Infrastructure.Exceptions;
using CleanTemplate.Application.Infrastructure.Topology;
using CleanTemplate.Application.Infrastructure.UserContexts;
using CleanTemplate.Application.Stations.Models;
using CleanTemplate.Domain.Entities;
using CleanTemplate.Domain.Enums;
using CleanTemplate.Persistence;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace CleanTemplate.Application.Bikes;

public interface IBikesService
{
    Task<List<BikeModel>> GetActiveBikesInRadius(double longtitude, double altitude, double radius, CancellationToken cancellationToken);
    Task<BikeModel> GetBikeById(string id, CancellationToken cancellationToken);
    Task<Bike> ActivateBikeAsync(string bikeId, CancellationToken cancellationToken);
    Task<Bike> CreateBikeAsync(CancellationToken cancellationToken);
    Task<Bike> RepairBikeAsync(string bikeId, CancellationToken cancellationToken);
    Task<Bike> WasteBikeAsync(string bikeId, CancellationToken cancellationToken);
}

public class BikesService : IBikesService
{

    private readonly MyDbContext context;
    private readonly UserContext userContext;

    public BikesService(MyDbContext context, UserContext userContext)
    {
        this.context = context;
        this.userContext = userContext;
    }

    public async Task<BikeModel> GetBikeById(string id, CancellationToken cancellationToken)
    {
        var bike = await context.Bikes
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Where(e => e.Status == BikeStatusEnum.Active)
            .Select(e => new BikeModel
            {
                Id = e.Id,
                Longtitude = e.Location.X,
                Altitude = e.Location.Y,
                Status = e.Status,
                Station = e.CurrentStationId != null ?
                    new StationModel
                    {
                        Id = e.CurrentStation!.Id,
                        Name = e.CurrentStation.Name,
                        Longtitude = e.CurrentStation.Location.X,
                        Altitude = e.CurrentStation.Location.Y,
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

    public async Task<List<BikeModel>> GetActiveBikesInRadius(double longtitude, double altitude, double radius, CancellationToken cancellationToken)
    {
        var location = new Point(longtitude, altitude) { SRID = 4326 };
        var bikes = await context.Bikes
            .AsNoTracking()
            .Where(e => e.Location.Distance(location) <= radius)
            .Select(e => new BikeModel
            {
                Id = e.Id,
                Longtitude = e.Location.X,
                Altitude = e.Location.Y,
                Status = e.Status,
            })
            .ToListAsync(cancellationToken);

        return bikes;
    }

    public async Task<Bike> CreateBikeAsync(CancellationToken cancellationToken)
    {
        var random = new Random();
        var longtitude = random.NextDouble() *
            (TopologyConstants.LongtitudeBordersUkraine.max - TopologyConstants.LongtitudeBordersUkraine.min) +
            TopologyConstants.LongtitudeBordersUkraine.min;
        var altitude = random.NextDouble() *
            (TopologyConstants.AltitudeBordersUkraine.max - TopologyConstants.AltitudeBordersUkraine.min) +
            TopologyConstants.AltitudeBordersUkraine.min;
        var bike = new Bike();
        bike.Location = new Point(longtitude, altitude) { SRID = 4326 };
        context.Add(bike);
        await context.SaveChangesAsync(cancellationToken);
        return bike;
    }

    public async Task<Bike> ActivateBikeAsync(string bikeId, CancellationToken cancellationToken)
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
        return bike;
    }

    public async Task<Bike> RepairBikeAsync(string bikeId, CancellationToken cancellationToken)
    {
        var bike = await context.Bikes.FirstOrDefaultAsync(e => e.Id == bikeId, cancellationToken);
        if (bike is null)
        {
            throw new NotFoundException($"Bike with Id {bikeId} does not exist.");
        }
        if (bike.Status != BikeStatusEnum.Active)
        {
            throw new ValidationException($"Bike with Id {bikeId} can not be set on repairing. Current status: {bike.Status}.");
        }
        bike.Status = BikeStatusEnum.Repairing;
        bike.CurrentStationId = null;
        context.Update(bike);
        await context.SaveChangesAsync(cancellationToken);
        return bike;
    }

    public async Task<Bike> WasteBikeAsync(string bikeId, CancellationToken cancellationToken)
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
        return bike;
    }
}