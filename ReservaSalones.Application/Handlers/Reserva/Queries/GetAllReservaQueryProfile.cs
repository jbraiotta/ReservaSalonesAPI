namespace ReservaSalones.Application.Handlers.Reserva.Queries;

using AutoMapper;
using ReservaSalones.Application.Models;
using ReservaSalones.Domain.Entities;

public class GetAllReservaQueryProfile : Profile
{
    public GetAllReservaQueryProfile()
    {
        CreateMap<Reserva, ReservaResponseDto>()
            .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(x => x.SalonName, opt => opt.MapFrom(src => src.Salon.Name))
            .ForMember(x => x.DateFrom, opt => opt.MapFrom(src => src.DateFrom))
            .ForMember(x => x.DateFrom, opt => opt.MapFrom(src => src.DateFrom))
            .ForMember(x => x.DateTo, opt => opt.MapFrom(src => src.DateTo));
    }
}