# Frontend - Sistema Bancario

Aplicación Angular para la gestión de clientes, cuentas y movimientos bancarios.

## Instalación

```bash
npm install
```

## Desarrollo

```bash
npm start
```

La aplicación se abrirá en `http://localhost:4200`

## Build para Producción

```bash
npm run build
```

Los archivos compilados estarán en `dist/`

## Pruebas

```bash
npm test
```

## Estructura

```
src/
├── app/
│   ├── core/           # Servicios y modelos
│   ├── shared/         # Componentes compartidos
│   ├── layout/         # Layout principal
│   └── modules/        # Módulos por funcionalidad
│       ├── dashboard/
│       ├── clientes/
│       ├── cuentas/
│       ├── movimientos/
│       └── reportes/
```

## Configuración de la API

Editar `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api'
};
```

## Funcionalidades

- CRUD de clientes, cuentas y movimientos
- Búsqueda en tiempo real en las tablas
- Validaciones de formularios
- Notificaciones de éxito/error
- Generación de reportes con descarga de PDF
- Diseño responsive sin usar frameworks de componentes (todo CSS personalizado)

## Notas Técnicas

- Se usa Reactive Forms para todos los formularios
- Los errores de la API se manejan automáticamente con un interceptor
- El pipe `searchFilter` permite buscar en múltiples campos
- Los PDFs se descargan desde strings Base64 que devuelve el backend
