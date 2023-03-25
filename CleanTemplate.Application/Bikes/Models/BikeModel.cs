using CleanTemplate.Application.Stations.Models;
using CleanTemplate.Domain.Entities;
using CleanTemplate.Domain.Enums;

namespace CleanTemplate.Application.Bikes.Models;

public class BikeModel
{
    public BikeModel()
    { }
    public BikeModel(Bike bike)
    {
        if (bike is null)
        {
            throw new ArgumentNullException("Null bike entity passed to bike model constructor.");
        }

        Id = bike.Id;
        Latitude = bike.Location.X;
        Longtitude = bike.Location.Y;
        Status = bike.Status;
        Station = bike.CurrentStation is not null ? new StationModel(bike.CurrentStation) : null;
    }
    public string Id { get; set; }
    public double Latitude { get; set; }
    public double Longtitude { get; set; }
    public BikeStatusEnum Status { get; set; }
    public StationModel? Station { get; set; }
}