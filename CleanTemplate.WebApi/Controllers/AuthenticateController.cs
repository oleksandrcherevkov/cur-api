﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CleanTemplate.Application.Infrastructure.Exceptions;
using CleanTemplate.Domain.Entities;
using CleanTemplate.Persistence;
using CleanTemplate.WebApi.Infrastructure.ExceptionsHandling;
using CleanTemplate.WebApi.Infrastructure.Jwt;
using CleanTemplate.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanTemplate.WebApi.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IJwtGenerator generator;
    private readonly MyDbContext context;

    public AuthController(IJwtGenerator generator, MyDbContext context)
    {
        this.generator = generator;
        this.context = context;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginReponceModel))]
    [ProducesResponseType(typeof(ExceptionInfo), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Login([FromBody] LoginModel model)
    {
        var user = context.Users.FirstOrDefault(e => e.Email == model.Email);

        if (user == null)
        {
            throw new UnauthorizedException($"User with email: \'{model.Email}\' doesn't exist");
        }

        if (!user.VerifyPassword(model.Password))
        {
            throw new UnauthorizedException($"User with email: \'{model.Email}\' entered incorrect password");
        }

        var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sid, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("role", user.Role.ToString() ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

        var token = generator.Generate(authClaims);

        var wrapper = new LoginReponceModel
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
        };

        return Ok(wrapper);
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationExceptionInfo), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Register([FromBody] RegisterModel model)
    {
        var user = new User(model.Email, model.Password, model.Role);
        context.Add(user);
        await context.SaveChangesAsync(HttpContext.RequestAborted);
        return NoContent();
    }
}
