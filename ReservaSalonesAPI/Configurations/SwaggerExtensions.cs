namespace ReservaSalones.API.Configurations;

using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;

public static class SwaggerExtensions
{

    private const string ApiName = "ReservaSalones.Api";

    public static void AddSwaggerConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRouting(options => options.LowercaseUrls = true);
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });
        services.AddSwaggerGen(swagger =>
        {
            var forwardedPrefix = configuration["ForwardedPrefix"];

            if (!string.IsNullOrEmpty(forwardedPrefix))
            {
                swagger.AddServer(new OpenApiServer
                {
                    Url = $"{forwardedPrefix}"
                });
            }

            swagger.AddSecurityDefinition("apiKey",
                            new()
                            {
                                Name = "apiKey",
                                Description = "apiKey",
                                Type = SecuritySchemeType.ApiKey,
                                In = ParameterLocation.Header
                            });

            swagger.AddSecurityRequirement(new()
                        {
                            {
                                new() { Reference = new() { Type = ReferenceType.SecurityScheme, Id = "apiKey" } },
                                Array.Empty<string>()
                            }
                        });

            swagger.SwaggerDoc("v1", new OpenApiInfo { Title = ApiName, Version = "v1" });
        });
    }

    public static void UseSwaggerConfig(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(swagger =>
            swagger.SwaggerEndpoint("v1/swagger.json", ApiName + " v1"));

    }

}