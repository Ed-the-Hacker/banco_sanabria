# Banco Sanabria - Sistema de Gestión Bancaria

Sistema completo de gestión de cuentas bancarias desarrollado con .NET 6 y Angular.

## 📋 Tabla de Contenidos

- [Descripción](#descripción)
- [Tecnologías](#tecnologías)
- [Arquitectura](#arquitectura)
- [Requisitos Previos](#requisitos-previos)
- [Instalación](#instalación)
- [Configuración](#configuración)
- [Ejecución](#ejecución)
- [Testing](#testing)
- [API Endpoints](#api-endpoints)
- [Docker](#docker)

## 📖 Descripción

Aplicación web para la gestión integral de cuentas bancarias que incluye:

- Gestión de clientes
- Administración de cuentas bancarias
- Registro de movimientos (débitos y créditos)
- Generación de reportes con exportación a PDF
- Validaciones de negocio (límites diarios, saldos disponibles)

## 🛠️ Tecnologías

### Backend
- .NET 6
- Entity Framework Core 6
- SQL Server / PostgreSQL
- QuestPDF (generación de PDFs)
- xUnit (pruebas unitarias)
- Swagger/OpenAPI

### Frontend (Próximamente)
- Angular 16+
- TypeScript
- RxJS
- CSS/SCSS personalizado

## 🏗️ Arquitectura

El proyecto sigue una **arquitectura limpia (Clean Architecture)** con las siguientes capas:

```
banco_sanabria/
├── src/
│   ├── BancoSanabria.API/          # Capa de presentación (Controllers, Middleware)
│   ├── BancoSanabria.Application/  # Lógica de negocio (Services, DTOs, Strategies)
│   ├── BancoSanabria.Domain/       # Entidades del dominio
│   └── BancoSanabria.Infrastructure/ # Acceso a datos (Repositories, DbContext)
└── tests/
    └── BancoSanabria.Tests/        # Pruebas unitarias
```

### Patrones Implementados

- **Repository Pattern**: Abstracción del acceso a datos
- **Unit of Work**: Gestión de transacciones
- **Strategy Pattern**: Manejo de tipos de movimiento (Crédito/Débito)
- **Dependency Injection**: Inversión de control
- **CQRS**: Separación de comandos y consultas

## ✅ Requisitos Previos

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [SQL Server](https://www.microsoft.com/sql-server) o [PostgreSQL](https://www.postgresql.org/)
- [Docker](https://www.docker.com/) (opcional)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)

## 📦 Instalación

### 1. Clonar el repositorio

```bash
git clone https://github.com/tuusuario/banco_sanabria.git
cd banco_sanabria
```

### 2. Restaurar paquetes NuGet

```bash
dotnet restore
```

### 3. Configurar la base de datos

Edita el archivo `src/BancoSanabria.API/appsettings.json` con tu cadena de conexión:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BancoSanabria;User Id=sa;Password=TuPassword;TrustServerCertificate=True;"
  }
}
```

### 4. Ejecutar el script de base de datos

```bash
# Con SQL Server Management Studio o Azure Data Studio
sqlcmd -S localhost -U sa -P TuPassword -i BaseDatos.sql
```

O ejecutar las migraciones de Entity Framework:

```bash
cd src/BancoSanabria.API
dotnet ef database update
```

## 🚀 Ejecución

### Ejecución Local

```bash
cd src/BancoSanabria.API
dotnet run
```

La API estará disponible en:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

### Ejecución con Docker

```bash
# Construir y ejecutar con Docker Compose
docker-compose up -d

# La API estará disponible en http://localhost:5000
```

## 🧪 Testing

Ejecutar todas las pruebas:

```bash
dotnet test
```

Ejecutar con cobertura:

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## 📚 API Endpoints

### Clientes

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/clientes` | Obtener todos los clientes |
| GET | `/api/clientes/{id}` | Obtener cliente por ID |
| POST | `/api/clientes` | Crear nuevo cliente |
| PUT | `/api/clientes/{id}` | Actualizar cliente completo |
| PATCH | `/api/clientes/{id}` | Actualizar cliente parcialmente |
| DELETE | `/api/clientes/{id}` | Eliminar cliente |

### Cuentas

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/cuentas` | Obtener todas las cuentas |
| GET | `/api/cuentas/{id}` | Obtener cuenta por ID |
| GET | `/api/cuentas/cliente/{clienteId}` | Obtener cuentas por cliente |
| POST | `/api/cuentas` | Crear nueva cuenta |
| PUT | `/api/cuentas/{id}` | Actualizar cuenta |
| PATCH | `/api/cuentas/{id}` | Actualizar cuenta parcialmente |
| DELETE | `/api/cuentas/{id}` | Eliminar cuenta |

### Movimientos

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/movimientos` | Obtener todos los movimientos |
| GET | `/api/movimientos/{id}` | Obtener movimiento por ID |
| GET | `/api/movimientos/cuenta/{cuentaId}` | Obtener movimientos por cuenta |
| POST | `/api/movimientos` | Registrar nuevo movimiento |
| DELETE | `/api/movimientos/{id}` | Eliminar movimiento |

### Reportes

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/reportes?fechaInicio={fecha}&fechaFin={fecha}&clienteId={id}` | Generar reporte en JSON + PDF Base64 |
| POST | `/api/reportes` | Generar reporte (alternativa POST) |

## 🐳 Docker

### Construir imagen

```bash
docker build -t banco-sanabria-api .
```

### Ejecutar contenedor

```bash
docker run -d -p 5000:80 --name banco-api banco-sanabria-api
```

### Docker Compose (Recomendado)

```bash
# Iniciar todos los servicios (API + SQL Server)
docker-compose up -d

# Ver logs
docker-compose logs -f

# Detener servicios
docker-compose down
```

## 🔒 Validaciones de Negocio

### Movimientos - Débitos

1. **Saldo Disponible**: No permite débitos si el saldo es 0 o insuficiente
   - Mensaje: `"Saldo no disponible"`

2. **Límite Diario**: Máximo $1000 en retiros por día
   - Mensaje: `"Cupo diario Excedido"`

### Movimientos - Créditos

Los créditos no tienen restricciones especiales.

## 📄 Datos de Prueba

El script `BaseDatos.sql` incluye datos de ejemplo:

### Clientes
- Jose Lema (ID: 1234567890)
- Marianela Montalvo (ID: 0987654321)
- Juan Osorio (ID: 1122334455)

### Cuentas
- 478758 (Jose Lema - Ahorros)
- 225487 (Marianela Montalvo - Corriente)
- 495878 (Juan Osorio - Ahorros)
- 496825 (Marianela Montalvo - Ahorros)

## 📝 Notas de Desarrollo

### Características Destacadas

✅ Arquitectura limpia y escalable  
✅ Patrón Repository y Unit of Work  
✅ Patrón Strategy para tipos de movimiento  
✅ Middleware global de manejo de excepciones  
✅ Validaciones a nivel de modelo (DataAnnotations)  
✅ Uso de LINQ y programación funcional  
✅ Generación de PDFs con QuestPDF  
✅ Pruebas unitarias con xUnit, Moq y FluentAssertions  
✅ Documentación con Swagger/OpenAPI  
✅ Soporte para Docker  
✅ CORS configurado para Angular  

### Próximos Pasos

- [ ] Implementar frontend Angular
- [ ] Agregar autenticación JWT
- [ ] Implementar logging con Serilog
- [ ] Agregar cache con Redis
- [ ] Implementar versionado de API
- [ ] Agregar health checks

## 👨‍💻 Autor

Desarrollado como prueba técnica para demostrar conocimientos en:
- Arquitectura de software
- .NET y Entity Framework Core
- Patrones de diseño
- Pruebas unitarias
- DevOps (Docker)

## 📄 Licencia

Este proyecto es de código abierto y está disponible bajo la licencia MIT.

