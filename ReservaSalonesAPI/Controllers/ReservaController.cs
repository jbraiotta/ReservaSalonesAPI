using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReservaSalones.Application.Handlers.Reserva.Commands.Insert;
using ReservaSalones.Application.Handlers.Reserva.Queries;
using ReservaSalones.Application.Models;

namespace ReservaSalones.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReservaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Metodo que realiza la reserva al salon y fecha hora recibida
        /// </summary>
        /// <param name="reserva"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReservaRequestDto reserva)
        {
            var request = new InsertReservaCommand(reserva.IdSalon, reserva.Name, reserva.FechaDesde, reserva.FechaHasta);

            var response = await _mediator.Send(request);
            
            if (response.IsSuccessStatusCode)
            {
                return Ok("Se cargó reserva satisfactoriamente!");
            }
            else
            {
                if(response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    return Conflict(response.Error);
                }
                else
                {
                    return BadRequest(response.Error);
                }
            }
        }

        /// <summary>
        /// Metodo que devuelve la reserva que coincida con la fecha y hora de entrada
        /// </summary>
        /// <param name="fecha"></param>
        /// <returns></returns>
        [HttpGet("{fecha}")]
        public async Task<IActionResult> Get(string fecha)
        {
            if (!DateTime.TryParse(fecha, out var reservedDate))
                return BadRequest("Fecha inválida");

            var request = new GetReservaQuery(reservedDate);

            var response = await _mediator.Send(request); 
            
            if (response.IsSuccessStatusCode) 
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Error);
            }
        }


        /// <summary>
        /// Metodo que devuelve todas las reservas con el id de salon como entrada
        /// </summary>
        /// <param name="idSalon"></param>
        /// <returns></returns>
        [HttpGet("/api/ReservaAll/{idSalon}")]
        public async Task<IActionResult> GetAll(int idSalon)
        {
            var request = new GetAllReservaQuery(idSalon);

            var response = await _mediator.Send(request);

            ReservaResponseDto first = null;

            if (response != null && response.FirstOrDefault() != null)
            {
                first = response.First();
            }

            if (first.Error != null)
            {
                return BadRequest(first.Error);
            }
            else
            {
                return Ok(response);
            }
        }
    }
}
