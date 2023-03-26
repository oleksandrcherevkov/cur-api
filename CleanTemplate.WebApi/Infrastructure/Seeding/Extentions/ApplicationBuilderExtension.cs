using CleanTemplate.Application.Infrastructure.Topology;
using CleanTemplate.Domain.Entities;
using CleanTemplate.Domain.Enums;
using CleanTemplate.Persistence;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace CleanTemplate.WebApi.Infrastructure.Seeding.Extentions;
public static class ApplicationBuilderExtension
{
    public static IApplicationBuilder SeedDatabase(this IApplicationBuilder appBuilder)
    {
        using (var scope = appBuilder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<MyDbContext>();

            var stations = SeedStations(200, context);
            SeedBikes(stations, 10, context);
        }

        return appBuilder;
    }

    private static List<Station> SeedStations(int count, MyDbContext context)
    {
        var random = new Random();
        Station CreateStation(int number)
        {
            var longtitude = random.NextDouble() *
                (TopologyConstants.LongtitudeBordersUkraine.max - TopologyConstants.LongtitudeBordersUkraine.min) +
                TopologyConstants.LongtitudeBordersUkraine.min;
            var latitude = random.NextDouble() *
                (TopologyConstants.LatitudeBordersUkraine.max - TopologyConstants.LatitudeBordersUkraine.min) +
                TopologyConstants.LatitudeBordersUkraine.min;

            return new Station()
            {
                Name = $"Station {number}",
                Location = new Point(longtitude, latitude) { SRID = 4326 },
            };
        }

        List<Station> stations = new List<Station>();
        for (int i = 0; i < count; i++)
        {
            stations.Add(CreateStation(i));
        }
        SeedCollection(stations, context);
        return stations;
    }

    private static List<Bike> SeedBikes(IList<Station> stations, int countPerStation, MyDbContext context)
    {
        var random = new Random();
        Bike CreateBike(Station station)
        {
            var longtitude = station.Location.X + (random.NextDouble() * 0.00007 * (random.Next(0, 2) * 2 - 1));
            var latitude = station.Location.Y + (random.NextDouble() * 0.00007 * (random.Next(0, 2) * 2 - 1));

            return new Bike()
            {
                CurrentStationId = station.Id,
                Location = new Point(longtitude, latitude) { SRID = 4326 },
                Status = BikeStatusEnum.Active,
            };
        }

        List<Bike> bikes = new List<Bike>();
        foreach (var station in stations)
        {
            for (int i = 0; i < countPerStation; i++)
            {
                bikes.Add(CreateBike(station));
            }
        }
        SeedCollection(bikes, context);
        return bikes;
    }

    private static IEnumerable<T> SeedCollection<T>(IEnumerable<T> collection, DbContext context)
        where T : class
    {
        if (!context.Set<T>().Any())
        {
            context.AddRange(collection);
            context.SaveChanges();
        }
        return collection;
    }
}
