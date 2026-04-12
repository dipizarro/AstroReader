using System;
using System.Threading;
using System.Threading.Tasks;
using AstroReader.AstroEngine.Exceptions;
using AstroReader.Application.SavedCharts.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AstroReader.Api.Infrastructure;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // 1. Manejo de Errores de Negocio / Semánticos (Ej. Formato de fecha con error de lógica)
        if (exception is ArgumentException argumentException)
        {
            var problemDetails = CreateProblemDetails(
                httpContext,
                StatusCodes.Status400BadRequest,
                "Error de Validación",
                argumentException.Message);

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true; // Excepción manejada
        }

        if (exception is KeyNotFoundException keyNotFoundException)
        {
            var problemDetails = CreateProblemDetails(
                httpContext,
                StatusCodes.Status404NotFound,
                "Recurso no encontrado",
                keyNotFoundException.Message);

            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }

        if (exception is SavedChartIntegrityException integrityException)
        {
            var problemDetails = CreateProblemDetails(
                httpContext,
                StatusCodes.Status422UnprocessableEntity,
                "Error de integridad del guardado",
                integrityException.Message);

            httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }

        if (exception is AstroCalculationException astroCalculationException)
        {
            var statusCode = MapAstroCalculationStatusCode(astroCalculationException.Code);
            var problemDetails = CreateProblemDetails(
                httpContext,
                statusCode,
                "Error del motor astral",
                astroCalculationException.PublicMessage);

            problemDetails.Extensions["code"] = astroCalculationException.Code.ToString();

            _logger.LogError(
                astroCalculationException,
                "Astro engine failure. Code: {ErrorCode}. Path: {Path}. Diagnostic: {DiagnosticMessage}",
                astroCalculationException.Code,
                httpContext.Request.Path,
                astroCalculationException.Message);

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }

        // 2. Manejo Genérico (Fallos del motor de cálculo, nulos, desconexiones, etc.)
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        _logger.LogError(
            exception,
            "Unhandled server exception. Path: {Path}",
            httpContext.Request.Path);

        var serverErrorDetails = CreateProblemDetails(
            httpContext,
            StatusCodes.Status500InternalServerError,
            "Error Interno del Servidor",
            "Ha ocurrido un error inesperado al procesar la solicitud.");

        await httpContext.Response.WriteAsJsonAsync(serverErrorDetails, cancellationToken);
        return true; 
    }

    private static int MapAstroCalculationStatusCode(AstroCalculationErrorCode code)
    {
        return code switch
        {
            AstroCalculationErrorCode.Configuration => StatusCodes.Status500InternalServerError,
            AstroCalculationErrorCode.Wrapper => StatusCodes.Status500InternalServerError,
            AstroCalculationErrorCode.Ephemerides => StatusCodes.Status503ServiceUnavailable,
            AstroCalculationErrorCode.Calculation => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private static ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int statusCode,
        string title,
        string detail)
    {
        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };
    }
}
