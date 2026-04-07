using System;
using System.Threading;
using System.Threading.Tasks;
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
