using CleanTemplate.Domain.Enums;
using CleanTemplate.Domain.Interfaces;
using NetTopologySuite.Geometries;

namespace CleanTemplate.Domain.Entities;

public class Bike : ILocation
{
    private static string StatusMissmatchExceptionMessage = "Bike cannot be transferred to provided status. Current: {0}, New: {1}";
    public Bike()
    {
        _id = Guid.NewGuid().ToString();
    }

    private string _id;
    public string Id => _id;
    public Point Location { get; set; }
    public int? CurrentStationId { get; set; }
    private BikeStatusEnum _status = BikeStatusEnum.Repairing;
    public BikeStatusEnum Status
    {
        get => _status;
        set
        {
            switch (_status)
            {
                case BikeStatusEnum.Active:
                    switch (value)
                    {
                        case BikeStatusEnum.Rented:
                        case BikeStatusEnum.Repairing:
                            break;
                        default:
                            throw new ArgumentException(string.Format(StatusMissmatchExceptionMessage, _status, value));
                    }
                    break;
                case BikeStatusEnum.Rented:
                    switch (value)
                    {
                        case BikeStatusEnum.Active:
                        case BikeStatusEnum.Repairing:
                            break;
                        default:
                            throw new ArgumentException(string.Format(StatusMissmatchExceptionMessage, _status, value));
                    }
                    break;
                case BikeStatusEnum.Repairing:
                    switch (value)
                    {
                        case BikeStatusEnum.Active:
                        case BikeStatusEnum.Broken:
                            break;
                        default:
                            throw new ArgumentException(string.Format(StatusMissmatchExceptionMessage, _status, value));
                    }
                    break;
                case BikeStatusEnum.Broken:
                    throw new ArgumentException(string.Format(StatusMissmatchExceptionMessage, _status, value));
                default:
                    throw new Exception($"Unknown bike status: {_status}");
            }
            _status = value;
        }
    }
    public Station? CurrentStation { get; set; }
    public ICollection<Ticket> Tickets { get; } = new List<Ticket>();
}
