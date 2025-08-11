namespace ClaimsServices.ART.Aplicacion.Handlers.Traslados.Command.Update;

using AutoMapper;
using ReservaSalones.Application.Handlers.Reserva.Commands.Insert;
using ReservaSalones.Domain.Entities;

public class InsertReservaCommandProfile : Profile
{
    public InsertReservaCommandProfile()
    {
        CreateMap<InsertReservaCommand, Reserva>()
            .ForMember(x => x.IdSalon, opt => opt.MapFrom(src => src.IdSalon))
            .ForMember(x => x.DateFrom, opt => opt.MapFrom(src => src.DateFrom))
            .ForMember(x => x.DateTo, opt => opt.MapFrom(src => src.DateTo));
    }
}