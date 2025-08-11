using FluentValidation;
using MediatR;
using System;

namespace ReservaSalones.Application.Behaviours;

public class ValidacionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> validadores;

    public ValidacionBehaviour(IEnumerable<IValidator<TRequest>> validadores) => this.validadores = validadores;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validadores.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var fallos = validadores.Select(v => v.Validate(context)).Where(x => x.Errors.Any()).SelectMany(r => r.Errors);

            if (fallos.Any())
            {
                throw new ValidationException(fallos);
            }
        }

        return await next();
    }
}