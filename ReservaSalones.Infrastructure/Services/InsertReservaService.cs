using Microsoft.EntityFrameworkCore;
using ReservaSalones.Application.Models;
using ReservaSalones.Application.Services;
using ReservaSalones.Domain.Entities;
using ReservaSalones.Infrastructure.Persistence.Infrastructure.Persistence;

namespace ReservaSalones.Infrastructure.Services
{
    public class InsertReservaService : IInsertReservaService
    {
        private readonly ApplicationDbContext _context;

        public InsertReservaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReservaResponseMessage> InsertReserva(int idSalon, string name, DateTime dateFrom, DateTime dateTo)
        {
            var inicioConBuffer = dateFrom.AddMinutes(-30);

            var reservaConConflicto = await _context.Set<Reserva>()
                                  .FirstOrDefaultAsync(r => r.IdSalon == idSalon &&
                                                        (
                                                            // Condición de superposición de horarios
                                                            (r.DateFrom < dateTo && r.DateTo > dateFrom) ||
                                                            // Condición de 30 minutos: la reserva existente termina
                                                            // dentro del período de 30 minutos antes de la nueva
                                                            (r.DateTo > dateFrom.AddMinutes(-30) && r.DateTo <= dateFrom) ||

                                                            // Condición de 30 minutos: la nueva reserva termina
                                                            // dentro del período de 30 minutos antes de la existente
                                                            (r.DateFrom > dateTo.AddMinutes(-30) && r.DateFrom <= dateTo)
                                                        ));

            if (reservaConConflicto != null)
            {
                return new ReservaResponseMessage()
                {
                    Error = "Existe un conflicto de horarios. Asegúrate de que no haya superposición y que haya un mínimo de 30 minutos entre reservas.",
                    IsSuccessStatusCode = false,
                    StatusCode = System.Net.HttpStatusCode.Conflict
                };
            }

            var nuevaReserva = new Reserva
            {
                IdSalon = idSalon,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Name = name
            };

            _context.Set<Reserva>().Add(nuevaReserva);
            await _context.SaveChangesAsync();

            return new ReservaResponseMessage()
            {
                IsSuccessStatusCode = true,
                StatusCode = System.Net.HttpStatusCode.OK
            };
        }
    }
}
