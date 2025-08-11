namespace ReservaSalones.Domain.Entities
{
    public class Salon
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Location { get; set; }

        public string? Street { get; set; }

        public int StreetNumber { get; set; }
        
        public string? Town { get; set; }

        public string? City { get; set; }

        public ICollection<Reserva> Reservas { get; set; }
    }
}
