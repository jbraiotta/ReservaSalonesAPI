namespace ReservaSalones.Application.Models
{
    public class ReservaRequestDto
    {
        public int IdSalon { get; set; }

        public string Name { get; set; }

        public DateTime FechaDesde { get; set; }

        public DateTime FechaHasta { get; set; }
    }
}
