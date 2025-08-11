using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReservaSalones.Application.Models;
using ReservaSalones.Application.Services;
using ReservaSalones.Domain.Entities;
using ReservaSalones.Infrastructure.Persistence.Infrastructure.Persistence;

namespace ReservaSalones.Infrastructure.Services
{
    public class GetReservaService : IGetReservaService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetReservaService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ReservaResponseDto>> GetAllReserva(int idSalon, CancellationToken cancellationToken)
        {
            var reservas = await _context.Set<Reserva>().Include("Salon").Where(x => x.IdSalon == idSalon).ToListAsync(cancellationToken);

            if (reservas != null && reservas.Any())
            {
                return _mapper.Map<List<ReservaResponseDto>>(reservas);
            }

            var empty = new ReservaResponseDto()
            {
                Error = "No hay reservas para ese salon",
                IsSuccessStatusCode = false,
                StatusCode = System.Net.HttpStatusCode.NotFound
            };
            
            return new List<ReservaResponseDto> { empty };
        }

        public async Task<ReservaResponseDto> GetReserva(DateTime reservedDate, CancellationToken cancellationToken)
        {
            var reservaOcupada = await _context.Set<Reserva>().Include("Salon").FirstOrDefaultAsync(r => r.DateFrom <= reservedDate && r.DateTo > reservedDate);

            if (reservaOcupada == null)
            {
                return new ReservaResponseDto() {
                    Error = "No Existe una reserva en ese horario.",
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    IsSuccessStatusCode = false
                };
            }

            var response = _mapper.Map<ReservaResponseDto>(reservaOcupada);

            response.StatusCode = System.Net.HttpStatusCode.OK;
            response.IsSuccessStatusCode = true;

            return response;
        }
    }
}
