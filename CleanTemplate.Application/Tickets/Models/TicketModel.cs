namespace CleanTemplate.Application.Tickets.Models;

public class TicketModel
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}