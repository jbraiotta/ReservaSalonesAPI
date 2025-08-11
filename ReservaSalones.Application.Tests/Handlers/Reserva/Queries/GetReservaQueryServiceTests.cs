namespace ReservaSalones.Application.Tests.Handlers.Reserva.Queries;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using ReservaSalones.Application.Models;
using ReservaSalones.Infrastructure.Persistence.Infrastructure.Persistence;
using ReservaSalones.Infrastructure.Services;
using System.Net;
using ReservaSalones.Domain.Entities;

public class GetReservaServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetReservaService _service;

    public GetReservaServiceTests()
    {
        // Configura la base de datos en memoria
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new ApplicationDbContext(options);

        // Configura el mock del mapper
        _mapperMock = new Mock<IMapper>();

        // Crea una instancia del servicio con las dependencias
        _service = new GetReservaService(_context, _mapperMock.Object);
    }

    public void Dispose()
    {
        // Limpia la base de datos después de cada prueba
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    // --- Pruebas para GetAllReserva ---

    [Fact]
    public async Task GetAllReserva_Should_Return_MappedReservas_When_ReservasExist()
    {
        // Arrange
        var idSalon = 1;
        var cancellationToken = CancellationToken.None;

        // Create and add the required Salon entity
        var salon = new Salon
        {
            Id = 1,
            Name = "Salón Principal",
            Location = "Planta Baja",
            Street = "Av. Siempre Viva",
            StreetNumber = 742,
            Town = "Springfield",
            City = "Springfield"
        };    

        _context.Set<Salon>().Add(salon);

        var reservas = new List<Reserva>
        {
            new Reserva
            {
                Id = 1,
                IdSalon = idSalon,
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddHours(2),
                Name = "Reserva 1",
                Salon = salon // Link to the Salon entity
            },
            new Reserva
            {
                Id = 2,
                IdSalon = idSalon,
                DateFrom = DateTime.Now.AddDays(1),
                DateTo = DateTime.Now.AddDays(1).AddHours(2),
                Name = "Reserva 2",
                Salon = salon // Link to the Salon entity
            }
        };

        _context.Set<Reserva>().AddRange(reservas);
        _context.SaveChanges(); // The error is likely here, when saving

        // Configure the mock of the mapper to return DTOs
        _mapperMock.Setup(m => m.Map<List<ReservaResponseDto>>(It.IsAny<List<Reserva>>()))
                    .Returns(new List<ReservaResponseDto>
                    {
                    new ReservaResponseDto { IsSuccessStatusCode = true },
                    new ReservaResponseDto { IsSuccessStatusCode = true }
                    });

        // Act
        var result = await _service.GetAllReserva(idSalon, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.True(result.All(r => r.IsSuccessStatusCode));
        _mapperMock.Verify(m => m.Map<List<ReservaResponseDto>>(It.IsAny<List<Reserva>>()), Times.Once);
    }

    [Fact]
    public async Task GetAllReserva_Should_Return_NotFound_Error_When_NoReservasExist()
    {
        // Arrange
        var idSalon = 1;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _service.GetAllReserva(idSalon, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.False(result.First().IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, result.First().StatusCode);
        Assert.Contains("No hay reservas para ese salon", result.First().Error);
        _mapperMock.Verify(m => m.Map<List<ReservaResponseDto>>(It.IsAny<List<Reserva>>()), Times.Never);
    }

    // --- Pruebas para GetReserva ---

    [Fact]
    public async Task GetReserva_Should_Return_NotFound_When_NoReservationExists()
    {
        // Arrange
        var reservedDate = new DateTime(2025, 1, 1, 10, 0, 0); // No hay reservas en esta fecha
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _service.GetReserva(reservedDate, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.Contains("No Existe una reserva en ese horario.", result.Error);
        _mapperMock.Verify(m => m.Map<ReservaResponseDto>(It.IsAny<Reserva>()), Times.Never);
    }
}