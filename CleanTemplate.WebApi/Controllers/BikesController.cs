using CleanTemplate.Application.Bikes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanTemplate.WebApi.Controllers;

[Authorize]
public class BikesController : BaseController
{
    private readonly BikesService bikesService;

    public BikesController(IMediator mediator, BikesService bikesService) : base(mediator)
    {
        this.bikesService = bikesService;
    }

    [HttpGet]
    public async Task<IActionResult> GetInRadius([FromQuery] double lat, [FromQuery] double lon, [FromQuery] double radius)
    {
        return Ok(await bikesService.GetActiveBikesInRadiusAsync(lat, lon, radius, HttpContext.RequestAborted));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        return Ok(await bikesService.GetBikeByIdAsync(id, HttpContext.RequestAborted));
    }

    [HttpPost]
    [Authorize(Roles = "Technician")]
    public async Task<IActionResult> Create()
    {
        return Ok(await bikesService.CreateBikeAsync(HttpContext.RequestAborted));
    }

    [HttpPatch("{id}/activate")]
    [Authorize(Roles = "Technician")]
    public async Task<IActionResult> Activate([FromRoute] string id)
    {
        return Ok(await bikesService.ActivateBikeAsync(id, HttpContext.RequestAborted));
    }

    [HttpPatch("{id}/repair")]
    [Authorize(Roles = "Technician")]
    public async Task<IActionResult> Repair([FromRoute] string id)
    {
        return Ok(await bikesService.RepairBikeAsync(id, HttpContext.RequestAborted));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Technician")]
    public async Task<IActionResult> Waste([FromRoute] string id)
    {
        return Ok(await bikesService.WasteBikeAsync(id, HttpContext.RequestAborted));
    }
}