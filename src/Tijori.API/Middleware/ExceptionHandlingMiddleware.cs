using System.Net;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Tijori.Application.Common;

namespace Tijori.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Unhandled exception occurred.");

        var (statusCode, response) = exception switch
        {
            ValidationException validationException => (
                HttpStatusCode.BadRequest,
                ApiResponse.Fail(
                    "Validation failed.",
                    validationException.Errors.Select(x => x.ErrorMessage).ToList())),
            ValidationAppException validationAppException => (
                HttpStatusCode.BadRequest,
                ApiResponse.Fail(validationAppException.Message, validationAppException.Errors)),
            NotFoundAppException notFoundAppException => (
                HttpStatusCode.NotFound,
                ApiResponse.Fail(notFoundAppException.Message)),
            UnauthorizedAppException unauthorizedAppException => (
                HttpStatusCode.Unauthorized,
                ApiResponse.Fail(unauthorizedAppException.Message)),
            ConflictAppException conflictAppException => (
                HttpStatusCode.Conflict,
                ApiResponse.Fail(conflictAppException.Message)),
            AppException appException => (
                HttpStatusCode.BadRequest,
                ApiResponse.Fail(appException.Message)),
            DbUpdateException dbUpdateException => (
                HttpStatusCode.BadRequest,
                ApiResponse.Fail(BuildDatabaseErrorMessage(dbUpdateException))),
            _ => (
                HttpStatusCode.InternalServerError,
                ApiResponse.Fail(BuildUnexpectedErrorMessage(exception)))
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsJsonAsync(response);
    }

    private string BuildUnexpectedErrorMessage(Exception exception)
    {
        if (!_environment.IsDevelopment())
        {
            return "An unexpected error occurred.";
        }

        return exception.InnerException?.Message ?? exception.Message;
    }

    private string BuildDatabaseErrorMessage(DbUpdateException exception)
    {
        if (!_environment.IsDevelopment())
        {
            return "A database error occurred.";
        }

        return exception.InnerException?.Message ?? exception.Message;
    }
}
