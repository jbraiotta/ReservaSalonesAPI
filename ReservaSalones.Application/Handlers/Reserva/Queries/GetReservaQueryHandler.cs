namespace ReservaSalones.Application.Handlers.Reserva.Queries;

using MediatR;
using ReservaSalones.Application.Models;
using ReservaSalones.Application.Services;

public record GetReservaQuery(DateTime ReservedDate) : IRequest<ReservaResponseDto>;

public class GetReservaQueryHandler : IRequestHandler<GetReservaQuery, ReservaResponseDto>
{
    private readonly IGetReservaService _service;

    public GetReservaQueryHandler(IGetReservaService service)
    {
        _service = service;
    }

    public async Task<ReservaResponseDto> Handle(GetReservaQuery request, CancellationToken cancellationToken)
    {
        return await _service.GetReserva(request.ReservedDate, cancellationToken);
    }
}
