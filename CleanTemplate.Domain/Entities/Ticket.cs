using System.Data;

namespace CleanTemplate.Domain.Entities;

public class Ticket
{
    public int Id { get; }

    public Ticket(string bikeId, string userId, DateTime startDate)
    {
        BikeId = bikeId;
        UserId = userId;
        StartDate = startDate;
    }

    public string BikeId { get; }
    public string UserId { get; }
    public DateTime StartDate { get; }
    public DateTime? EndDate { get; private set; } = null;

    public Bike Bike { get; }
    public User User { get; }

    public void SetEndDate(DateTime date)
    {
        if (EndDate != null)
        {
            throw new ReadOnlyException("Ending date of ticket already been set. This ticket cannot be changed.");
        }
        EndDate = date;
    }
}
