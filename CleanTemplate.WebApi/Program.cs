using System.Text;
using CleanTemplate.Application.Infrastructure.UserContexts;
using CleanTemplate.Application.Infrastructurel;
using CleanTemplate.Persistence;
using CleanTemplate.WebApi.Infrastructure.Data.Extensions;
using CleanTemplate.WebApi.Infrastructure.ExceptionsHandling.Extensions;
using CleanTemplate.WebApi.Infrastructure.Jwt.Extentions;
using CleanTemplate.WebApi.Infrastructure.UserContexts.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Standandard setup
builder.Services.AddControllers(options =>
    {
        var policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build();
        options.Filters.Add(new AuthorizeFilter(policy));
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Context
builder.Services.AddDbContext<MyDbContext>(options =>
    {
        options.UseSqlServer(configuration.GetConnectionString("Default"), x => x.UseNetTopologySuite());
    });

// Application atchitecture
builder.Services.AddMediatR(typeof(ValidationException).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehaviour<,>));
AssemblyScanner.FindValidatorsInAssembly(typeof(ValidationException).Assembly)
    .ForEach(item => builder.Services.AddScoped(item.InterfaceType, item.ValidatorType));

// Jwt
builder.Services.AddJwtGenerator(configuration);
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = configuration["Jwt:Audience"],
            ValidIssuer = configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"])),
        };
    });

// UserContext
builder.Services.AddScoped<UserContext>();

var app = builder.Build();

await app.SetupDatabaseAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCustomExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.UseCustomUserContext();

app.MapControllers();

app.Run();
