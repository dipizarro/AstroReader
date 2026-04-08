using System;
using System.Threading;
using System.Threading.Tasks;
using AstroReader.Application.SavedCharts.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AstroReader.Api.Infrastructure;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // 1. Manejo de Errores de Negocio / Semánticos (Ej. Formato de fecha con error de lógica)
        if (exception is ArgumentException argumentException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Error de Validación",
                Detail = argumentException.Message
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true; // Excepción manejada
        }

        if (exception is KeyNotFoundException keyNotFoundException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Recurso no encontrado",
                Detail = keyNotFoundException.Message
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }

        if (exception is SavedChartIntegrityException integrityException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Title = "Error de integridad del guardado",
                Detail = integrityException.Message
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }

        // 2. Manejo Genérico (Fallos del motor de cálculo, nulos, desconexiones, etc.)
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var serverErrorDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Error Interno del Servidor",
            Detail = "Ha ocurrido un error inesperado al procesar la solicitud."
            // Importante: No pasamos exception.Message aquí para no exponer internals en Prod.
        };

        await httpContext.Response.WriteAsJsonAsync(serverErrorDetails, cancellationToken);
        return true; 
    }
}
