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

**Middleware de la API**
La API utiliza middleware personalizado para manejar la seguridad y el manejo global de errores, lo que mejora la robustez y la experiencia del desarrollador.

ApiKeyAuthorizationMiddleware: Este middleware se encarga de la autenticación por clave de API. Todas las solicitudes deben incluir una clave de API válida en el encabezado para ser procesadas, proporcionando una capa de seguridad esencial.

ErrorHandlingMiddleware: Captura las excepciones no controladas en toda la aplicación. En lugar de permitir que la API falle, este middleware devuelve una respuesta estandarizada y fácil de usar (500 Internal Server Error), lo que hace que la API sea más estable y predecible para los clientes.

**Paso a Paso de Authenticación y pruebas**

Paso 1: Autenticación en el appSettings hay una sección con el apikey para poder auntenticar (ver archivo).

Paso 2: Colocar en swagger-> Authenticate el apikey
<img width="1894" height="980" alt="image" src="https://github.com/user-attachments/assets/6f73e93b-f194-422d-b65b-43ab5f64fe66" />

Paso 3: Crear Reserva

Caso exitoso:
<img width="1723" height="951" alt="image" src="https://github.com/user-attachments/assets/9fa918e9-8356-411a-b1ae-2decf6c1b573" />

<img width="1841" height="1000" alt="image" src="https://github.com/user-attachments/assets/8e2c4d51-4903-456a-a217-bfae19642f6d" />

<img width="1875" height="976" alt="image" src="https://github.com/user-attachments/assets/87527970-2c26-4f2b-8712-a008d34bee7b" />

Caso error:
<img width="1862" height="1022" alt="image" src="https://github.com/user-attachments/assets/20fd4903-60df-4df4-8bed-47aa45a38937" />

<img width="1853" height="1041" alt="image" src="https://github.com/user-attachments/assets/d9a56922-3bc2-4c32-bb15-2228334ca69c" />

Paso 4: Consulta por una reserva y todas las de un salon
<img width="1714" height="979" alt="image" src="https://github.com/user-attachments/assets/9956fb02-70c6-4530-b0bd-cf8abf874596" />

<img width="1593" height="875" alt="image" src="https://github.com/user-attachments/assets/7925c127-ec7e-4608-b729-9483905cf316" />







