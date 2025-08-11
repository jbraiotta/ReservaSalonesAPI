namespace ReservaSalones.Application.Models
{
    public class ReservaResponseDto : ResponseMessage
    {
        public string Name { get; set; }

        public string SalonName { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }
    }
}
