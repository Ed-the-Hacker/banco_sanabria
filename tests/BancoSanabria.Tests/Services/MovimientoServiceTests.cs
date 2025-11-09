using BancoSanabria.Application.DTOs;
using BancoSanabria.Application.Services;
using BancoSanabria.Application.Strategies;
using BancoSanabria.Domain.Entities;
using BancoSanabria.Domain.Exceptions;
using BancoSanabria.Infrastructure.Repositories;
using BancoSanabria.Infrastructure.UnitOfWork;
using FluentAssertions;
using Moq;
using Xunit;

namespace BancoSanabria.Tests.Services
{
    public class MovimientoServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICuentaRepository> _cuentaRepositoryMock;
        private readonly Mock<IMovimientoRepository> _movimientoRepositoryMock;
        private readonly MovimientoStrategyFactory _strategyFactory;
        private readonly MovimientoService _movimientoService;

        public MovimientoServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _cuentaRepositoryMock = new Mock<ICuentaRepository>();
            _movimientoRepositoryMock = new Mock<IMovimientoRepository>();

            _unitOfWorkMock.Setup(u => u.Cuentas).Returns(_cuentaRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.Movimientos).Returns(_movimientoRepositoryMock.Object);

            // Configurar estrategias
            var strategies = new List<IMovimientoStrategy>
            {
                new CreditoStrategy(_unitOfWorkMock.Object),
                new DebitoStrategy(_unitOfWorkMock.Object)
            };

            _strategyFactory = new MovimientoStrategyFactory(strategies);
            _movimientoService = new MovimientoService(_unitOfWorkMock.Object, _strategyFactory);
        }

        [Fact]
        public async Task CreateAsync_DebitoConSaldoCero_DeberiaLanzarExcepcion()
        {
            // Arrange
            var cuenta = new Cuenta
            {
                CuentaId = 1,
                NumeroCuenta = "123456",
                SaldoInicial = 1000m,
                Estado = true
            };

            var ultimoMovimiento = new Movimiento
            {
                MovimientoId = 1,
                CuentaId = 1,
                Saldo = 0m, // Saldo en cero
                TipoMovimiento = "Credito",
                Valor = 500m
            };

            var movimientoDto = new MovimientoCreateDto
            {
                CuentaId = 1,
                TipoMovimiento = "Debito",
                Valor = 100m,
                Fecha = DateTime.Now
            };

            _cuentaRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(cuenta);

            _movimientoRepositoryMock
                .Setup(r => r.GetUltimoMovimientoAsync(It.IsAny<int>()))
                .ReturnsAsync(ultimoMovimiento);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(
                () => _movimientoService.CreateAsync(movimientoDto)
            );

            exception.Message.Should().Be("Saldo no disponible");
        }

        [Fact]
        public async Task CreateAsync_DebitoConSaldoInsuficiente_DeberiaLanzarExcepcion()
        {
            // Arrange
            var cuenta = new Cuenta
            {
                CuentaId = 1,
                NumeroCuenta = "123456",
                SaldoInicial = 1000m,
                Estado = true
            };

            var ultimoMovimiento = new Movimiento
            {
                MovimientoId = 1,
                CuentaId = 1,
                Saldo = 50m, // Saldo menor al valor del débito
                TipoMovimiento = "Credito",
                Valor = 50m
            };

            var movimientoDto = new MovimientoCreateDto
            {
                CuentaId = 1,
                TipoMovimiento = "Debito",
                Valor = 100m, // Intenta retirar más de lo disponible
                Fecha = DateTime.Now
            };

            _cuentaRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(cuenta);

            _movimientoRepositoryMock
                .Setup(r => r.GetUltimoMovimientoAsync(It.IsAny<int>()))
                .ReturnsAsync(ultimoMovimiento);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(
                () => _movimientoService.CreateAsync(movimientoDto)
            );

            exception.Message.Should().Be("Saldo no disponible");
        }

        [Fact]
        public async Task CreateAsync_DebitoExcedeLimiteDiario_DeberiaLanzarExcepcion()
        {
            // Arrange
            var cuenta = new Cuenta
            {
                CuentaId = 1,
                NumeroCuenta = "123456",
                SaldoInicial = 5000m,
                Estado = true
            };

            var ultimoMovimiento = new Movimiento
            {
                MovimientoId = 1,
                CuentaId = 1,
                Saldo = 4000m,
                TipoMovimiento = "Credito",
                Valor = 4000m
            };

            var movimientoDto = new MovimientoCreateDto
            {
                CuentaId = 1,
                TipoMovimiento = "Debito",
                Valor = 600m, // Este retiro superaría el límite diario de 1000
                Fecha = DateTime.Now
            };

            _cuentaRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(cuenta);

            _movimientoRepositoryMock
                .Setup(r => r.GetUltimoMovimientoAsync(It.IsAny<int>()))
                .ReturnsAsync(ultimoMovimiento);

            // Ya se han retirado 500 hoy
            _movimientoRepositoryMock
                .Setup(r => r.GetSumaDebitosDelDiaAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync(500m);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(
                () => _movimientoService.CreateAsync(movimientoDto)
            );

            exception.Message.Should().Be("Cupo diario Excedido");
        }

        [Fact]
        public async Task CreateAsync_CreditoValido_DeberiaCrearMovimientoCorrectamente()
        {
            // Arrange
            var cuenta = new Cuenta
            {
                CuentaId = 1,
                NumeroCuenta = "123456",
                SaldoInicial = 1000m,
                Estado = true,
                Cliente = new Cliente { ClienteId = 1, Nombre = "Test" }
            };

            var ultimoMovimiento = new Movimiento
            {
                MovimientoId = 1,
                CuentaId = 1,
                Saldo = 1500m,
                TipoMovimiento = "Credito",
                Valor = 500m
            };

            var movimientoDto = new MovimientoCreateDto
            {
                CuentaId = 1,
                TipoMovimiento = "Credito",
                Valor = 500m,
                Fecha = DateTime.Now
            };

            _cuentaRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(cuenta);

            _movimientoRepositoryMock
                .Setup(r => r.GetUltimoMovimientoAsync(It.IsAny<int>()))
                .ReturnsAsync(ultimoMovimiento);

            _movimientoRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Movimiento>()))
                .ReturnsAsync((Movimiento m) => m);

            // Act
            var resultado = await _movimientoService.CreateAsync(movimientoDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Valor.Should().Be(500m); // Valor positivo para crédito
            resultado.Saldo.Should().Be(2000m); // 1500 + 500
            resultado.TipoMovimiento.Should().Be("Credito");

            _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DebitoValido_DeberiaCrearMovimientoCorrectamente()
        {
            // Arrange
            var cuenta = new Cuenta
            {
                CuentaId = 1,
                NumeroCuenta = "123456",
                SaldoInicial = 1000m,
                Estado = true,
                Cliente = new Cliente { ClienteId = 1, Nombre = "Test" }
            };

            var ultimoMovimiento = new Movimiento
            {
                MovimientoId = 1,
                CuentaId = 1,
                Saldo = 1500m,
                TipoMovimiento = "Credito",
                Valor = 500m
            };

            var movimientoDto = new MovimientoCreateDto
            {
                CuentaId = 1,
                TipoMovimiento = "Debito",
                Valor = 300m,
                Fecha = DateTime.Now
            };

            _cuentaRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(cuenta);

            _movimientoRepositoryMock
                .Setup(r => r.GetUltimoMovimientoAsync(It.IsAny<int>()))
                .ReturnsAsync(ultimoMovimiento);

            _movimientoRepositoryMock
                .Setup(r => r.GetSumaDebitosDelDiaAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync(0m);

            _movimientoRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Movimiento>()))
                .ReturnsAsync((Movimiento m) => m);

            // Act
            var resultado = await _movimientoService.CreateAsync(movimientoDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Valor.Should().Be(-300m); // Valor negativo para débito
            resultado.Saldo.Should().Be(1200m); // 1500 - 300
            resultado.TipoMovimiento.Should().Be("Debito");

            _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_CuentaInactiva_DeberiaLanzarExcepcion()
        {
            // Arrange
            var cuenta = new Cuenta
            {
                CuentaId = 1,
                NumeroCuenta = "123456",
                SaldoInicial = 1000m,
                Estado = false // Cuenta inactiva
            };

            var movimientoDto = new MovimientoCreateDto
            {
                CuentaId = 1,
                TipoMovimiento = "Credito",
                Valor = 500m,
                Fecha = DateTime.Now
            };

            _cuentaRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(cuenta);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(
                () => _movimientoService.CreateAsync(movimientoDto)
            );

            exception.Message.Should().Be("La cuenta está inactiva");
        }

        [Fact]
        public async Task CreateAsync_TipoMovimientoInvalido_DeberiaLanzarExcepcion()
        {
            // Arrange
            var cuenta = new Cuenta
            {
                CuentaId = 1,
                NumeroCuenta = "123456",
                SaldoInicial = 1000m,
                Estado = true
            };

            var movimientoDto = new MovimientoCreateDto
            {
                CuentaId = 1,
                TipoMovimiento = "TipoInvalido", // Tipo no válido
                Valor = 500m,
                Fecha = DateTime.Now
            };

            _cuentaRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(cuenta);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(
                () => _movimientoService.CreateAsync(movimientoDto)
            );

            exception.Message.Should().Contain("Tipo de movimiento");
            exception.Message.Should().Contain("no válido");
        }
    }
}

