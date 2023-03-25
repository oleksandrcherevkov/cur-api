using CleanTemplate.Application.Infrastructure.UserContexts;
using CleanTemplate.Application.Stations.Models;
using CleanTemplate.Domain.Enums;
using CleanTemplate.Persistence;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace CleanTemplate.Application.Stations;

public class StationsService
{
    private readonly MyDbContext context;
    private readonly UserContext userContext;

    public StationsService(MyDbContext context, UserContext userContext)
    {
        this.context = context;
        this.userContext = userContext;
    }

    public async Task<List<StationModel>> GetAllInRadius(double latitude, double longtitude, double radius, CancellationToken cancellationToken)
    {
        var location = new Point(latitude, longtitude) { SRID = 4326 };
        var bikes = await context.Stations
            .AsNoTracking()
            .Where(e => e.Location.Distance(location) <= radius)
            .Select(e => new StationModel
            {
                Id = e.Id,
                Latitude = e.Location.X,
                Longtitude = e.Location.Y,
                Name = e.Name,
                BikesCount = e.Bikes.Where(e => e.Status == BikeStatusEnum.Active).Count(),
            })
            .ToListAsync(cancellationToken);

        return bikes;
    }
}