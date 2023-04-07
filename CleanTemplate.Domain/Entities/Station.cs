using CleanTemplate.Domain.Interfaces;
using NetTopologySuite.Geometries;

namespace CleanTemplate.Domain.Entities;

public class Station : ILocation
{
    public int Id { get; }
    public string? Name { get; set; }
    public Point Location { get; set; }
    public ICollection<Bike> Bikes = new List<Bike>();
}