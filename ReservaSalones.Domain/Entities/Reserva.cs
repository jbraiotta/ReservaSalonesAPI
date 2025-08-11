namespace ReservaSalones.Domain.Entities
{
    public class Reserva
    {
        public int Id { get; set; }

        public int IdSalon { get; set; }

        public string Name { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public virtual Salon Salon { get; set; }
    }
}
