-- =============================================
-- Script de creación de base de datos
-- Banco Sanabria - Sistema de Gestión Bancaria
-- =============================================

USE master;
GO

-- Crear base de datos si no existe
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BancoSanabria')
BEGIN
    CREATE DATABASE BancoSanabria;
END
GO

USE BancoSanabria;
GO

-- =============================================
-- Crear tablas
-- =============================================

-- Tabla Clientes
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clientes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Clientes] (
        [ClienteId] INT IDENTITY(1,1) PRIMARY KEY,
        [Nombre] NVARCHAR(100) NOT NULL,
        [Genero] NVARCHAR(10) NOT NULL,
        [Edad] INT NOT NULL,
        [Identificacion] NVARCHAR(20) NOT NULL UNIQUE,
        [Direccion] NVARCHAR(200) NULL,
        [Telefono] NVARCHAR(20) NULL,
        [Contrasena] NVARCHAR(255) NOT NULL,
        [Estado] BIT NOT NULL DEFAULT 1
    );
END
GO

-- Tabla Cuentas
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Cuentas]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Cuentas] (
        [CuentaId] INT IDENTITY(1,1) PRIMARY KEY,
        [NumeroCuenta] NVARCHAR(20) NOT NULL UNIQUE,
        [TipoCuenta] NVARCHAR(50) NOT NULL,
        [SaldoInicial] DECIMAL(18,2) NOT NULL,
        [Estado] BIT NOT NULL DEFAULT 1,
        [ClienteId] INT NOT NULL,
        CONSTRAINT FK_Cuentas_Clientes FOREIGN KEY ([ClienteId]) 
            REFERENCES [dbo].[Clientes]([ClienteId])
    );
END
GO

-- Tabla Movimientos
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Movimientos]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Movimientos] (
        [MovimientoId] INT IDENTITY(1,1) PRIMARY KEY,
        [Fecha] DATETIME NOT NULL,
        [TipoMovimiento] NVARCHAR(50) NOT NULL,
        [Valor] DECIMAL(18,2) NOT NULL,
        [Saldo] DECIMAL(18,2) NOT NULL,
        [CuentaId] INT NOT NULL,
        CONSTRAINT FK_Movimientos_Cuentas FOREIGN KEY ([CuentaId]) 
            REFERENCES [dbo].[Cuentas]([CuentaId])
    );
    
    CREATE INDEX IX_Movimientos_Fecha ON [dbo].[Movimientos]([Fecha]);
    CREATE INDEX IX_Movimientos_CuentaId ON [dbo].[Movimientos]([CuentaId]);
END
GO

-- =============================================
-- Insertar datos de ejemplo
-- =============================================

-- Limpiar datos existentes (solo para desarrollo)
DELETE FROM [dbo].[Movimientos];
DELETE FROM [dbo].[Cuentas];
DELETE FROM [dbo].[Clientes];

-- Resetear identity
DBCC CHECKIDENT ('[dbo].[Movimientos]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[Cuentas]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[Clientes]', RESEED, 0);
GO

-- Insertar Clientes
INSERT INTO [dbo].[Clientes] ([Nombre], [Genero], [Edad], [Identificacion], [Direccion], [Telefono], [Contrasena], [Estado])
VALUES 
    ('Jose Lema', 'Masculino', 35, '1234567890', 'Otavalo sn y principal', '098254785', '1234', 1),
    ('Marianela Montalvo', 'Femenino', 28, '0987654321', 'Amazonas y NNUU', '097548965', '5678', 1),
    ('Juan Osorio', 'Masculino', 42, '1122334455', '13 junio y Equinoccial', '098874587', '1245', 1);
GO

-- Insertar Cuentas
INSERT INTO [dbo].[Cuentas] ([NumeroCuenta], [TipoCuenta], [SaldoInicial], [Estado], [ClienteId])
VALUES 
    ('478758', 'Ahorros', 2000.00, 1, 1),
    ('225487', 'Corriente', 100.00, 1, 2),
    ('495878', 'Ahorros', 0.00, 1, 3),
    ('496825', 'Ahorros', 540.00, 1, 2);
GO

-- Insertar Movimientos de ejemplo
-- Cliente: Jose Lema - Cuenta 478758
INSERT INTO [dbo].[Movimientos] ([Fecha], [TipoMovimiento], [Valor], [Saldo], [CuentaId])
VALUES 
    ('2024-02-08 10:00:00', 'Debito', -575.00, 1425.00, 1);

-- Cliente: Marianela Montalvo - Cuenta 225487
INSERT INTO [dbo].[Movimientos] ([Fecha], [TipoMovimiento], [Valor], [Saldo], [CuentaId])
VALUES 
    ('2024-02-08 09:00:00', 'Credito', 600.00, 700.00, 2),
    ('2024-02-08 11:00:00', 'Debito', -150.00, 550.00, 2);

-- Cliente: Juan Osorio - Cuenta 495878
INSERT INTO [dbo].[Movimientos] ([Fecha], [TipoMovimiento], [Valor], [Saldo], [CuentaId])
VALUES 
    ('2024-02-08 08:00:00', 'Credito', 150.00, 150.00, 3),
    ('2024-02-08 12:00:00', 'Debito', -150.00, 0.00, 3);

-- Cliente: Marianela Montalvo - Cuenta 496825
INSERT INTO [dbo].[Movimientos] ([Fecha], [TipoMovimiento], [Valor], [Saldo], [CuentaId])
VALUES 
    ('2024-02-08 13:00:00', 'Debito', -540.00, 0.00, 4);
GO

-- Actualizar los saldos iniciales de las cuentas con el último saldo de los movimientos
UPDATE c
SET c.SaldoInicial = m.Saldo
FROM [dbo].[Cuentas] c
INNER JOIN (
    SELECT CuentaId, Saldo, 
           ROW_NUMBER() OVER (PARTITION BY CuentaId ORDER BY Fecha DESC, MovimientoId DESC) as rn
    FROM [dbo].[Movimientos]
) m ON c.CuentaId = m.CuentaId AND m.rn = 1;
GO

-- =============================================
-- Verificar datos insertados
-- =============================================

SELECT 'Clientes' as Tabla, COUNT(*) as Total FROM [dbo].[Clientes]
UNION ALL
SELECT 'Cuentas', COUNT(*) FROM [dbo].[Cuentas]
UNION ALL
SELECT 'Movimientos', COUNT(*) FROM [dbo].[Movimientos];
GO

-- Mostrar datos de ejemplo
SELECT 
    c.Nombre,
    cu.NumeroCuenta,
    cu.TipoCuenta,
    cu.SaldoInicial as SaldoActual,
    cu.Estado
FROM [dbo].[Clientes] c
INNER JOIN [dbo].[Cuentas] cu ON c.ClienteId = cu.ClienteId
ORDER BY c.ClienteId, cu.CuentaId;
GO

PRINT 'Base de datos creada e inicializada correctamente';
GO

