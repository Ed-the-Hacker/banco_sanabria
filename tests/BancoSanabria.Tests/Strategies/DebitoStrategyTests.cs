using BancoSanabria.Application.Strategies;
using FluentAssertions;
using Moq;
using Xunit;

namespace BancoSanabria.Tests.Strategies
{
    public class DebitoStrategyTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMovimientoRepository> _movimientoRepositoryMock;
        private readonly DebitoStrategy _debitoStrategy;

        public DebitoStrategyTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _movimientoRepositoryMock = new Mock<IMovimientoRepository>();
            
            _unitOfWorkMock.Setup(u => u.Movimientos).Returns(_movimientoRepositoryMock.Object);
            
            _debitoStrategy = new DebitoStrategy(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task ValidarMovimientoAsync_SaldoCero_DeberiaLanzarExcepcion()
        {
            // Arrange
            var cuenta = new Cuenta
            {
                CuentaId = 1,
                NumeroCuenta = "123456",
                SaldoInicial = 1000m
            };

            var ultimoMovimiento = new Movimiento
            {
                MovimientoId = 1,
                CuentaId = 1,
                Saldo = 0m // Saldo en cero
            };

            _movimientoRepositoryMock
                .Setup(r => r.GetUltimoMovimientoAsync(It.IsAny<int>()))
                .ReturnsAsync(ultimoMovimiento);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(
                () => _debitoStrategy.ValidarMovimientoAsync(cuenta, 100m, DateTime.Now)
            );

            exception.Message.Should().Be("Saldo no disponible");
        }

        [Fact]
        public async Task ValidarMovimientoAsync_SaldoInsuficiente_DeberiaLanzarExcepcion()
        {
            // Arrange
            var cuenta = new Cuenta
            {
                CuentaId = 1,
                NumeroCuenta = "123456",
                SaldoInicial = 1000m
            };

            var ultimoMovimiento = new Movimiento
            {
                MovimientoId = 1,
                CuentaId = 1,
                Saldo = 50m
            };

            _movimientoRepositoryMock
                .Setup(r => r.GetUltimoMovimientoAsync(It.IsAny<int>()))
                .ReturnsAsync(ultimoMovimiento);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(
                () => _debitoStrategy.ValidarMovimientoAsync(cuenta, 100m, DateTime.Now)
            );

            exception.Message.Should().Be("Saldo no disponible");
        }

        [Fact]
        public async Task ValidarMovimientoAsync_ExcedeLimiteDiario_DeberiaLanzarExcepcion()
        {
            // Arrange
            var cuenta = new Cuenta
            {
                CuentaId = 1,
                NumeroCuenta = "123456",
                SaldoInicial = 5000m
            };

            var ultimoMovimiento = new Movimiento
            {
                MovimientoId = 1,
                CuentaId = 1,
                Saldo = 4000m
            };

            _movimientoRepositoryMock
                .Setup(r => r.GetUltimoMovimientoAsync(It.IsAny<int>()))
                .ReturnsAsync(ultimoMovimiento);

            // Ya se retiraron 700 en el día
            _movimientoRepositoryMock
                .Setup(r => r.GetSumaDebitosDelDiaAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync(700m);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(
                () => _debitoStrategy.ValidarMovimientoAsync(cuenta, 400m, DateTime.Now)
            );

            exception.Message.Should().Be("Cupo diario Excedido");
        }

        [Fact]
        public async Task ValidarMovimientoAsync_DebitoValido_NoDeberiaLanzarExcepcion()
        {
            // Arrange
            var cuenta = new Cuenta
            {
                CuentaId = 1,
                NumeroCuenta = "123456",
                SaldoInicial = 1000m
            };

            var ultimoMovimiento = new Movimiento
            {
                MovimientoId = 1,
                CuentaId = 1,
                Saldo = 1500m
            };

            _movimientoRepositoryMock
                .Setup(r => r.GetUltimoMovimientoAsync(It.IsAny<int>()))
                .ReturnsAsync(ultimoMovimiento);

            _movimientoRepositoryMock
                .Setup(r => r.GetSumaDebitosDelDiaAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync(200m);

            // Act & Assert
            await _debitoStrategy.ValidarMovimientoAsync(cuenta, 300m, DateTime.Now);
            
            // Si no lanza excepción, la prueba pasa
        }

        [Fact]
        public async Task CalcularNuevoSaldoAsync_DeberiaRestarValorDelSaldo()
        {
            // Arrange
            var cuenta = new Cuenta
            {
                CuentaId = 1,
                NumeroCuenta = "123456",
                SaldoInicial = 1000m
            };

            var ultimoMovimiento = new Movimiento
            {
                MovimientoId = 1,
                CuentaId = 1,
                Saldo = 1500m
            };

            _movimientoRepositoryMock
                .Setup(r => r.GetUltimoMovimientoAsync(It.IsAny<int>()))
                .ReturnsAsync(ultimoMovimiento);

            // Act
            var nuevoSaldo = await _debitoStrategy.CalcularNuevoSaldoAsync(cuenta, 300m);

            // Assert
            nuevoSaldo.Should().Be(1200m); // 1500 - 300
        }

        [Fact]
        public async Task CalcularNuevoSaldoAsync_SinMovimientosAnteriores_DeberiaUsarSaldoInicial()
        {
            // Arrange
            var cuenta = new Cuenta
            {
                CuentaId = 1,
                NumeroCuenta = "123456",
                SaldoInicial = 1000m
            };

            _movimientoRepositoryMock
                .Setup(r => r.GetUltimoMovimientoAsync(It.IsAny<int>()))
                .ReturnsAsync((Movimiento?)null);

            // Act
            var nuevoSaldo = await _debitoStrategy.CalcularNuevoSaldoAsync(cuenta, 300m);

            // Assert
            nuevoSaldo.Should().Be(700m); // 1000 - 300
        }
    }
}

