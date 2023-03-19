using CleanTemplate.Domain.Enums;
using NetTopologySuite.Geometries;

namespace CleanTemplate.Domain.Entities;

public class Bike
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
    private BikeStatus _status = BikeStatus.Repairing;
    public BikeStatus Status
    {
        get => _status;
        set
        {
            switch (_status)
            {
                case BikeStatus.Active:
                    switch (value)
                    {
                        case BikeStatus.Rented:
                        case BikeStatus.Repairing:
                            break;
                        default:
                            throw new ArgumentException(string.Format(StatusMissmatchExceptionMessage, _status, value));
                    }
                    break;
                case BikeStatus.Rented:
                    switch (value)
                    {
                        case BikeStatus.Active:
                        case BikeStatus.Repairing:
                            break;
                        default:
                            throw new ArgumentException(string.Format(StatusMissmatchExceptionMessage, _status, value));
                    }
                    break;
                case BikeStatus.Repairing:
                    switch (value)
                    {
                        case BikeStatus.Active:
                        case BikeStatus.Broken:
                            break;
                        default:
                            throw new ArgumentException(string.Format(StatusMissmatchExceptionMessage, _status, value));
                    }
                    break;
                case BikeStatus.Broken:
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
