namespace ReservaSalones.Application.Behaviours;

using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;

public class ExcepcionNoControladaBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> logger;
    private readonly Action<ILogger, TRequest, Exception> logError = LoggerMessage.Define<TRequest>(LogLevel.Error, new EventId(1, "ExcepcionNoControlada"), "ClaimsServices.ART.TrasladoAPI Request: Excepción no controlada para la solicitud {Param1}");

    public ExcepcionNoControladaBehaviour(ILogger<TRequest> logger) => this.logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex) when (ex is not ApplicationException and not ValidationException)
        {
            logError(logger, request, ex);

            throw;
        }
    }
}