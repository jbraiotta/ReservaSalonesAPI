using ClaimsServices.ART.Aplicacion.Handlers.Traslados.Command.Update;
using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ReservaSalones.API.Middleware;
using ReservaSalones.Application.Behaviours;
using ReservaSalones.Application.Handlers.Reserva.Commands.Insert;
using ReservaSalones.Application.Handlers.Reserva.Queries;
using ReservaSalones.Application.Services;
using ReservaSalones.Infrastructure.Persistence.Infrastructure.Persistence;
using ReservaSalones.Infrastructure.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Establece el camino a tu archivo XML
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.AddSecurityDefinition("apiKey", new()
    {
        Name = "apiKey",
        Description = "apiKey de autenticación",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header
    });
    options.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new() { Type = ReferenceType.SecurityScheme, Id = "apiKey" }
            },
            Array.Empty<string>()
        }
    });
    options.SwaggerDoc("v1", new() { Title = "Reserva de salones API" });
});

var getHandlersAssembly = typeof(GetReservaQueryHandler).Assembly;
var insertHandlersAssembly = typeof(InsertReservaCommandHandler).Assembly;

builder.Services.AddMediatR(mediatr =>
{
    mediatr.RegisterServicesFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
    mediatr.AddOpenBehavior(typeof(ExcepcionNoControladaBehaviour<,>));
    mediatr.AddOpenBehavior(typeof(ValidacionBehaviour<,>));
    mediatr.RegisterServicesFromAssembly(getHandlersAssembly);
    mediatr.RegisterServicesFromAssembly(insertHandlersAssembly);
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<InsertReservaCommandValidator>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(InsertReservaCommandProfile).Assembly);
    cfg.AddMaps(typeof(GetReservaQueryProfile).Assembly);
});

builder.Services.AddScoped<IInsertReservaService, InsertReservaService>();
builder.Services.AddScoped<IGetReservaService, GetReservaService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("ReservaSalonesDb")); // 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ApiKeyAuthorizationMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();

    // Esto crea la base de datos en memoria
    dbContext.Database.EnsureCreated();
}

app.Run();
