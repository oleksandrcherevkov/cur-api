using CleanTemplate.Domain.Entities;

namespace CleanTemplate.Application.Stations.Models;

public class StationModel
{
    public StationModel()
    { }
    public StationModel(Station station)
    {
        Id = station.Id;
        Longtitude = station.Location.X;
        Latitude = station.Location.Y;
        Name = station.Name;
    }
    public int Id { get; set; }
    public string? Name { get; set; }
    public double Latitude { get; set; }
    public double Longtitude { get; set; }
    public int BikesCount { get; set; }
}