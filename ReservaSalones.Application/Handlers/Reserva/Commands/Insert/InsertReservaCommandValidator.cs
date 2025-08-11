namespace ClaimsServices.ART.Aplicacion.Handlers.Traslados.Command.Update;

using FluentValidation;
using ReservaSalones.Application.Handlers.Reserva.Commands.Insert;

public class InsertReservaCommandValidator : AbstractValidator<InsertReservaCommand>
{
    public InsertReservaCommandValidator()
    {
        RuleFor(c => c.IdSalon).NotEqual(0);
        
        RuleFor(c => c.Name).NotEmpty().WithMessage("El nombre no puede estar vacío.");

        RuleFor(x => x.DateFrom)
            .LessThan(x => x.DateTo)
            .WithMessage("'DateFrom' debe ser menor que 'DateTo'");

        RuleFor(c => c.DateFrom.Hour)
                .InclusiveBetween(9, 16)
                .WithMessage("La reserva debe iniciar entre las 9:00 y las 16:00.");

        RuleFor(c => c.DateTo.Hour)
            .InclusiveBetween(11, 18)
            .WithMessage("La reserva debe finalizar entre las 11:00 y las 18:00.");

        // Enforce that the reservation duration is exactly 2 hours
        RuleFor(c => c)
            .Must(c => (c.DateTo - c.DateFrom).TotalHours == 2)
            .WithMessage("La duración de la reserva debe ser de exactamente 2 horas.");
    }
}