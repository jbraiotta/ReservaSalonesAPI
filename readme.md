**Reserva de Salones API**
Este proyecto es una API back-end para gestionar reservas de salones. Está construida con ASP.NET Core 8, siguiendo un patrón de arquitectura limpia, y utiliza MediatR para un enfoque de Segregación de Responsabilidad de Comandos y Consultas (CQRS).

🚀 **Cómo Empezar**
Para ejecutar el proyecto, asegúrate de tener el SDK de .NET 8 instalado.

Clona el repositorio:

Bash

git clone [la-url-de-tu-repo]
Navega al directorio del proyecto:

Bash

cd ReservaSalones
Ejecuta la aplicación:

Bash

dotnet run --project ReservaSalones.API
La API estará disponible en https://localhost:7064.

📦 **Estructura del Proyecto**

La solución está organizada en múltiples proyectos para mantener una clara separación de responsabilidades:

ReservaSalones.API: El punto de entrada de la aplicación. Contiene los controladores, middleware y la configuración de inyección de dependencias (Program.cs).

ReservaSalones.Application: La lógica de negocio principal. Incluye los comandos, consultas, handlers, DTOs y los comportamientos de validación para MediatR.

ReservaSalones.Domain: El modelo de dominio. Define las entidades principales (Reserva, Salon) y sus reglas de negocio.

ReservaSalones.Infrastructure: Contiene la capa de acceso a datos (DbContext de Entity Framework Core, repositorios) y servicios externos.

✅ **Buenas Prácticas y Tecnologías Implementadas**

MediatR & CQRS
Este proyecto utiliza MediatR para implementar el patrón CQRS. Las solicitudes se manejan como Comandos (para modificar datos) o Consultas (para recuperar datos). Esta práctica desacopla el envío de la solicitud de su manejo.

**Comandos**
* InsertReservaCommand: Se utiliza para generar reservas
* GetAllReservaQuery: Se utiliza para obtener todas las reservas de un salón específico.
* GetReservaQuery: Se utiliza para obtener una reserva en particular por fecha y hora.

**FluentValidation**
FluentValidation se utiliza para validar todos los comandos y consultas entrantes. Se implementa un ValidationBehaviour en el pipeline de MediatR para ejecutar automáticamente los validadores antes de que se ejecute el handler, lo que garantiza la integridad de los datos desde el primer momento.

**Restricciones de Reserva**
Las siguientes restricciones se aplican a las reservas y son validadas por FluentValidation:

Duración de la Reserva: Cada reserva debe tener una duración exacta de 2 horas.

Horario de Atención: Las reservas solo pueden iniciar entre las 9:00 AM y las 4:00 PM como máximo, y finalizar a las 6:00 PM.

Buffer entre Reservas: Debe haber un período de 30 minutos entre reservas consecutivas en el mismo salón para evitar solapamientos.

**Autenticación con Clave de API**
Se utiliza un ApiKeyAuthorizationMiddleware para asegurar los endpoints de la API. Todas las solicitudes deben incluir una apiKey válida en el encabezado, lo que añade una capa de seguridad básica y efectiva.

**Pruebas Unitarias (xUnit)**
El proyecto incluye proyectos de prueba dedicados (.Tests) para validar la lógica de negocio de forma aislada. xUnit es el framework de pruebas elegido, y utiliza una base de datos en memoria para probar las interacciones con DbContext sin depender de una base de datos real.

**Documentación de API**
La API está documentada con Swagger/OpenAPI. Los comentarios XML en los controladores y modelos de datos se utilizan para generar automáticamente una documentación detallada y fácil de usar, que se puede consultar en la interfaz de Swagger UI.
