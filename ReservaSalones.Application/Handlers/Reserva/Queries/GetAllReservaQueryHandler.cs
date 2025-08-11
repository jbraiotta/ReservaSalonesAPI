namespace ReservaSalones.Application.Handlers.Reserva.Queries;

using MediatR;
using ReservaSalones.Application.Models;
using ReservaSalones.Application.Services;

public record GetAllReservaQuery(int idSalon) : IRequest<List<ReservaResponseDto>>;

public class GetAllReservaQueryHandler : IRequestHandler<GetAllReservaQuery, List<ReservaResponseDto>>
{
    private readonly IGetReservaService _service;

    public GetAllReservaQueryHandler(IGetReservaService service)
    {
        _service = service;
    }

    public async Task<List<ReservaResponseDto>> Handle(GetAllReservaQuery request, CancellationToken cancellationToken)
    {
        return await _service.GetAllReserva(request.idSalon, cancellationToken);
    }
}
