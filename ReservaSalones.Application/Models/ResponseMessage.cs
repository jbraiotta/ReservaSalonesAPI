using System.Net;

namespace ReservaSalones.Application.Models
{
    public abstract class ResponseMessage
    {
        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccessStatusCode { get; set; }

        public string Error { get; set; }
    }
}
