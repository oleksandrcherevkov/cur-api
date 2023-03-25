using CleanTemplate.Application.Stations;
using CleanTemplate.Domain.Entities;
using CleanTemplate.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace CleanTemplate.WebApi.Controllers;

[Authorize]
public class StationsController : BaseController
{
    private readonly StationsService stationsService;

    public StationsController(IMediator mediator, StationsService stationsService) : base(mediator)
    {
        this.stationsService = stationsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllInRadius([FromQuery] double lat, [FromQuery] double lon, [FromQuery] double radius)
    {
        return Ok(await stationsService.GetAllInRadius(lat, lon, radius, HttpContext.RequestAborted));
    }
}