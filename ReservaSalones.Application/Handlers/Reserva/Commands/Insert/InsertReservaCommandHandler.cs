namespace ReservaSalones.Application.Handlers.Reserva.Commands.Insert;

using MediatR;
using ReservaSalones.Application.Models;
using ReservaSalones.Application.Services;

public record InsertReservaCommand(int IdSalon, string Name, DateTime DateFrom, DateTime DateTo) : IRequest<ReservaResponseMessage>;

public class InsertReservaCommandHandler : IRequestHandler<InsertReservaCommand, ReservaResponseMessage>
{
    private readonly IInsertReservaService _service;

    public InsertReservaCommandHandler(IInsertReservaService service)
    {
        _service = service;
    }

    public async Task<ReservaResponseMessage> Handle(InsertReservaCommand request, CancellationToken cancellationToken)
    {
        return await _service.InsertReserva(request.IdSalon, request.Name, request.DateFrom, request.DateTo);
    }
}
