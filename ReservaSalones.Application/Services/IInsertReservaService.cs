using ReservaSalones.Application.Models;

namespace ReservaSalones.Application.Services
{
    public interface IInsertReservaService
    {
        Task<ReservaResponseMessage> InsertReserva(int idSalon, string name, DateTime dateFrom, DateTime dateTo);        
    }
}
