using System.Net;
using CleanTemplate.Application.Infrastructure.Exceptions;
using Newtonsoft.Json;

namespace CleanTemplate.WebApi.Infrastructure.ExceptionsHandling;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate next;
    private readonly IWebHostEnvironment env;

    public ExceptionHandlerMiddleware(RequestDelegate next, IWebHostEnvironment env)
    {
        this.next = next;
        this.env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            ExceptionInfo info = GetExceptionInfo(ex, out int code);
            string infoJson = JsonConvert.SerializeObject(info);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = code;
            await context.Response.WriteAsync(infoJson);
        }
    }

    private ExceptionInfo GetExceptionInfo(Exception exception, out int statusCode)
    {
        ExceptionInfo info = new();

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = (int)HttpStatusCode.BadRequest;
                ValidationExceptionInfo valInfo = new();
                valInfo.Message = validationException.Message;
                valInfo.Errors = validationException.Failures;
                info = valInfo;
                break;
            case NotFoundException notFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                info.Message = notFoundException.Message;
                break;
            case UnauthorizedException unauthorizedException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                info.Message = unauthorizedException.Message;
                break;
            case ForbiddenException forbiddenException:
                statusCode = (int)HttpStatusCode.Forbidden;
                info.Message = forbiddenException.Message;
                break;
            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                if (!env.IsProduction())
                    info.Message = exception.Message;
                else
                    info.Message = "Internal server error.";
                break;
        }

        if (!env.IsProduction())
            info.StackTrace = exception.StackTrace ?? string.Empty;

        return info;
    }
}
