# Sistema de Gestión Bancaria

Aplicación full-stack para la gestión de clientes, cuentas y movimientos bancarios.

## Stack Tecnológico

**Backend:**
- .NET 6
- Entity Framework Core
- SQL Server
- QuestPDF para reportes

**Frontend:**
- Angular 16
- TypeScript
- SCSS

## Requisitos

- .NET 6 SDK
- Node.js 16+
- SQL Server (o usar Docker)

## Instalación y Ejecución

### Usando Docker (recomendado)

```bash
docker-compose up -d
```

1. **Inicializar la base de datos dentro del contenedor**
   ```bash
   docker cp BaseDatos.sql banco-sqlserver:/tmp/BaseDatos.sql
   docker exec -it banco-sqlserver /opt/mssql-tools/bin/sqlcmd \
     -S localhost -U sa -P YourStrong@Password -i /tmp/BaseDatos.sql
   ```
2. **Iniciar el frontend apuntando al backend de Docker**
   ```bash
   cd frontend
   npm install
   npm run start:docker
   ```

- API disponible en `http://localhost:5000/api`
- Swagger en `http://localhost:5000/swagger`
- Frontend en `http://localhost:4200`

> Nota: cuando levantas la app en local (`npm start`) se usa la API en `https://localhost:5001/api`.  
> Con `npm run start:docker` automáticamente se reemplaza el `apiUrl` por `http://localhost:5000/api`.

### Manualmente

**Backend:**

```bash
# Restaurar dependencias
dotnet restore

# Crear la base de datos
sqlcmd -S localhost -U sa -P YourStrong@Password -i BaseDatos.sql

# Ejecutar la API
cd src/BancoSanabria.API
dotnet run
```

API disponible en `https://localhost:5001`  
Swagger en `https://localhost:5001/swagger`

**Frontend:**

```bash
cd frontend
npm install
npm start
```

Aplicación disponible en `http://localhost:4200`

## Configuración

Editar la cadena de conexión en `src/BancoSanabria.API/appsettings.json` si es necesario:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BancoSanabria;User Id=sa;Password=YourStrong@Password;TrustServerCertificate=True;"
  }
}
```

En Docker no es necesario editar el archivo. El valor se sobreescribe con la variable
de entorno `ConnectionStrings__DefaultConnection` definida en `docker-compose.yml`
(servidor `sqlserver` y usuario `sa`).

## Funcionalidades Implementadas

### Backend

- CRUD completo de Clientes, Cuentas y Movimientos
- Validación de saldo disponible antes de débitos
- Límite diario de retiros ($1000)
- Generación de reportes por cliente y rango de fechas
- Exportación de reportes en PDF (Base64)

**Endpoints principales:**
- `/api/clientes`
- `/api/cuentas`
- `/api/movimientos`
- `/api/reportes?fechaInicio={fecha}&fechaFin={fecha}&clienteId={id}`

### Frontend

- Gestión de clientes (crear, editar, listar, eliminar)
- Gestión de cuentas bancarias
- Registro de movimientos (crédito/débito)
- Búsqueda en tablas
- Generación y descarga de reportes en PDF
- Manejo de errores y validaciones

## Reglas de Negocio

1. Los créditos son valores positivos, los débitos son negativos
2. No se permiten débitos si el saldo es 0: mensaje "Saldo no disponible"
3. Límite de retiros diarios de $1000: mensaje "Cupo diario Excedido"
4. El saldo se actualiza automáticamente con cada movimiento

## Datos de Prueba

El script `BaseDatos.sql` incluye datos de ejemplo:

**Clientes:**
- Jose Lema (ID: 1234567890)
- Marianela Montalvo (ID: 0987654321)
- Juan Osorio (ID: 1122334455)

**Cuentas:**
- 478758 - Ahorros - Jose Lema
- 225487 - Corriente - Marianela Montalvo
- 495878 - Ahorros - Juan Osorio
- 496825 - Ahorros - Marianela Montalvo

## Pruebas

```bash
# Backend
dotnet test

# Frontend
cd frontend
npm test
```

## Estructura del Proyecto

```
├── src/
│   ├── BancoSanabria.API/          # Controllers y configuración
│   ├── BancoSanabria.Application/  # Lógica de negocio y DTOs
│   ├── BancoSanabria.Domain/       # Entidades
│   └── BancoSanabria.Infrastructure/ # Repositorios y DbContext
├── tests/
│   └── BancoSanabria.Tests/        # Pruebas unitarias
├── frontend/                        # Aplicación Angular
├── BaseDatos.sql                    # Script de BD
├── Dockerfile
└── docker-compose.yml
```

## Notas

- El frontend está desarrollado con CSS personalizado, sin usar librerías de componentes
- Se implementó el patrón Strategy para el manejo de tipos de movimiento
- Las validaciones de negocio se manejan tanto en backend como frontend
- Los reportes se generan en formato JSON con el PDF incluido en Base64

## Postman

Importar `Postman_Collection.json` para probar los endpoints de la API.
