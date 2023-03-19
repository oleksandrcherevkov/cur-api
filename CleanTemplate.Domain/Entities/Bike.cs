using NetTopologySuite.Geometries;

namespace CleanTemplate.Domain.Entities;

public class Bike
{
    public Bike()
    {
        _id = Guid.NewGuid().ToString();
    }

    private string _id;
    public string Id => _id;
    public Point Location { get; set; }
    public int? CurrentStationId { get; set; }
    public Station? CurrentStation { get; set; }
    public ICollection<Ticket> Tickets { get; } = new List<Ticket>();
}
