using ClaimsServices.ART.Aplicacion.Handlers.Traslados.Command.Update;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using ReservaSalones.Application.Handlers.Reserva.Commands.Insert;
using ReservaSalones.Domain.Entities;
using ReservaSalones.Infrastructure.Persistence.Infrastructure.Persistence;
using ReservaSalones.Infrastructure.Services;

public class InsertReservaServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly InsertReservaService _service;
    private readonly InsertReservaCommandValidator _validator;

    public InsertReservaServiceTests()
    {
        _validator = new InsertReservaCommandValidator();

        // Use a unique database name for each test class
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _service = new InsertReservaService(_context);
    }

    public void Dispose()
    {
        // Clean up the in-memory database after each test
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    // --- Tests below ---

    [Fact]
    public async Task InsertReserva_WhenNoConflict_ShouldSucceed()
    {
        // Arrange
        var idSalon = 1;
        var name = "Test Reserva";
        var dateFrom = new DateTime(2025, 1, 1, 10, 0, 0);
        var dateTo = new DateTime(2025, 1, 1, 12, 0, 0);

        // Act
        var result = await _service.InsertReserva(idSalon, name, dateFrom, dateTo);
        var insertedReserva = await _context.Set<Reserva>().SingleOrDefaultAsync();

        // Assert
        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(insertedReserva);
        Assert.Equal(idSalon, insertedReserva.IdSalon);
    }

    [Fact]
    public async Task InsertReserva_WhenOverlap_ShouldReturnConflict()
    {
        // Arrange
        var existingReserva = new Reserva
        {
            IdSalon = 1,
            Name = "Existing Reserva",
            DateFrom = new DateTime(2025, 1, 1, 11, 0, 0),
            DateTo = new DateTime(2025, 1, 1, 13, 0, 0)
        };
        _context.Set<Reserva>().Add(existingReserva);
        await _context.SaveChangesAsync();

        var newReservaDateFrom = new DateTime(2025, 1, 1, 12, 0, 0);
        var newReservaDateTo = new DateTime(2025, 1, 1, 14, 0, 0);

        // Act
        var result = await _service.InsertReserva(1, "Conflicting Reserva", newReservaDateFrom, newReservaDateTo);
        var totalReservas = await _context.Set<Reserva>().CountAsync();

        // Assert
        Assert.False(result.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.Conflict, result.StatusCode);
        Assert.Equal(1, totalReservas); // Only the original reserva should exist
        Assert.Equal("Existe un conflicto de horarios. Asegúrate de que no haya superposición y que haya un mínimo de 30 minutos entre reservas.", result.Error);
    }

    [Fact]
    public async Task InsertReserva_When30MinuteBufferOverlap_ShouldReturnConflict()
    {
        // Arrange
        var existingReserva = new Reserva
        {
            IdSalon = 1,
            Name = "Existing Reserva",
            DateFrom = new DateTime(2025, 1, 1, 10, 0, 0),
            DateTo = new DateTime(2025, 1, 1, 12, 0, 0)
        };
        _context.Set<Reserva>().Add(existingReserva);
        await _context.SaveChangesAsync();

        // New reserva starts 15 minutes after the existing one ends
        var newReservaDateFrom = new DateTime(2025, 1, 1, 12, 15, 0);
        var newReservaDateTo = new DateTime(2025, 1, 1, 14, 15, 0);

        // Act
        var result = await _service.InsertReserva(1, "Conflicting Reserva", newReservaDateFrom, newReservaDateTo);
        var totalReservas = await _context.Set<Reserva>().CountAsync();

        // Assert
        Assert.False(result.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.Conflict, result.StatusCode);
        Assert.Equal(1, totalReservas);
    }

    [Fact]
    public async Task InsertReserva_When30MinuteBufferIsRespected_ShouldSucceed()
    {
        // Arrange
        var existingReserva = new Reserva
        {
            IdSalon = 1,
            Name = "Existing Reserva",
            DateFrom = new DateTime(2025, 1, 1, 10, 0, 0),
            DateTo = new DateTime(2025, 1, 1, 12, 0, 0)
        };
        _context.Set<Reserva>().Add(existingReserva);
        await _context.SaveChangesAsync();

        // New reserva starts exactly 30 minutes after the existing one ends
        var newReservaDateFrom = new DateTime(2025, 1, 1, 12, 30, 0);
        var newReservaDateTo = new DateTime(2025, 1, 1, 14, 30, 0);

        // Act
        var result = await _service.InsertReserva(1, "Valid Reserva", newReservaDateFrom, newReservaDateTo);
        var totalReservas = await _context.Set<Reserva>().CountAsync();

        // Assert
        Assert.True(result.IsSuccessStatusCode);
        Assert.Equal(2, totalReservas); // Both the original and new reserva should exist
    }

    [Fact]
    public void Should_Not_Have_Validation_Errors_When_Command_Is_Valid()
    {
        var command = new InsertReservaCommand(IdSalon: 1, Name: "Test", DateFrom : new DateTime(2025, 8, 11, 10, 0, 0), DateTo: new DateTime(2025, 8, 11, 12, 0, 0));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    // --- Pruebas de Escenarios Fallidos ---

    [Fact]
    public void Should_Have_Error_When_DateFrom_Is_After_DateTo()
    {
        var command = new InsertReservaCommand(IdSalon: 1, Name: "Reserva Fallida", DateFrom: new DateTime(2025, 8, 11, 15, 0, 0), DateTo: new DateTime(2025, 8, 11, 13, 0, 0));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DateFrom)
              .WithErrorMessage("'DateFrom' debe ser menor que 'DateTo'");
    }

    [Fact]
    public void Should_Have_Error_When_DateFrom_Is_Out_Of_Hours()
    {
        // Arrange: Start time is 8:00, which is outside the 9-16 range
        var command = new InsertReservaCommand(IdSalon: 1, Name: "Reserva Fuera de Horario", DateFrom: new DateTime(2025, 8, 11, 8, 0, 0), DateTo: new DateTime(2025, 8, 11, 10, 0, 0));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DateFrom.Hour)
              .WithErrorMessage("La reserva debe iniciar entre las 9:00 y las 16:00.");
    }

    [Fact]
    public void Should_Have_Error_When_Duration_Is_Not_Two_Hours()
    {
        // Arrange: Duration is 3 hours
        var command = new InsertReservaCommand(IdSalon: 1, Name: "Reserva Fuera de Horario", DateFrom: new DateTime(2025, 8, 11, 10, 0, 0), DateTo: new DateTime(2025, 8, 11, 13, 0, 0));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
              .WithErrorMessage("La duración de la reserva debe ser de exactamente 2 horas.");
    }
}