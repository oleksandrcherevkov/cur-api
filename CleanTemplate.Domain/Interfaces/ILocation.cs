using NetTopologySuite.Geometries;

namespace CleanTemplate.Domain.Interfaces;

public interface ILocation
{
    public Point Location { get; set; }
}