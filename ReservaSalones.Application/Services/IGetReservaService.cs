using ReservaSalones.Application.Models;

namespace ReservaSalones.Application.Services
{
    public interface IGetReservaService
    {
        Task<ReservaResponseDto> GetReserva(DateTime reservedDate, CancellationToken cancellationToken);

        Task<List<ReservaResponseDto>> GetAllReserva(int idSalon, CancellationToken cancellationToken);
    }
}
