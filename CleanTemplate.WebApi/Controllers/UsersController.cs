using CleanTemplate.Application.Infrastructure.UserContexts;
using CleanTemplate.Application.Users;
using CleanTemplate.Application.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanTemplate.WebApi.Controllers;

[Authorize]
public class UsersController : BaseController
{
    private readonly UsersService usersService;

    public UsersController(IMediator mediator, UsersService usersService) : base(mediator)
    {
        this.usersService = usersService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLogged([FromServices] UserContext userContext)
    {
        return Ok(await usersService.GetById(userContext.Id, HttpContext.RequestAborted));
    }

    [HttpPost("balance/add")]
    public async Task<IActionResult> AddToBalanceId([FromBody] AddToBalanceModel request)
    {
        return Ok(await usersService.AddToBalanceId(request, HttpContext.RequestAborted));
    }
}