using CleanTemplate.Application.Tickets;
using CleanTemplate.Domain.Entities;
using CleanTemplate.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace CleanTemplate.WebApi.Controllers;

[Authorize]
public class TicketsController : BaseController
{
    private readonly TicketsService ticketsService;

    public TicketsController(IMediator mediator, TicketsService ticketsService) : base(mediator)
    {
        this.ticketsService = ticketsService;
    }

    [HttpGet]
    public async Task<IActionResult> GeyMy()
    {
        return Ok(await ticketsService.GetMyAsync(HttpContext.RequestAborted));
    }

    [HttpGet("active")]
    public async Task<IActionResult> GeyMyActive()
    {
        return Ok(await ticketsService.GetActiveAsync(HttpContext.RequestAborted));
    }

    [HttpPost("{bikeId}")]
    public async Task<IActionResult> Open([FromRoute] string bikeId)
    {
        await ticketsService.OpenTicketAsync(bikeId, HttpContext.RequestAborted);
        return NoContent();
    }

    [HttpDelete("{bikeId}")]

    public async Task<IActionResult> Close([FromRoute] string bikeId)
    {
        await ticketsService.CloseTicketAsync(bikeId, cancellationToken: HttpContext.RequestAborted);
        return NoContent();
    }
}