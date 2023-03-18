using CleanTemplate.Application.Infrastructurel;
using CleanTemplate.Persistence;
using CleanTemplate.WebApi.Infrastructure.Data.Extensions;
using CleanTemplate.WebApi.Infrastructure.ExceptionsHandling.Extensions;
using CleanTemplate.WebApi.Infrastructure.Jwt.Extentions;
using CleanTemplate.WebApi.Infrastructure.UserContexts.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Standandard setup
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Context
builder.Services.AddDbContext<MyDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("Default"));
});

// Application atchitecture
builder.Services.AddMediatR(typeof(ValidationException).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehaviour<,>));
AssemblyScanner.FindValidatorsInAssembly(typeof(ValidationException).Assembly)
    .ForEach(item => builder.Services.AddScoped(item.InterfaceType, item.ValidatorType));

// Jwt
builder.Services.AddJwtGenerator(configuration);

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
