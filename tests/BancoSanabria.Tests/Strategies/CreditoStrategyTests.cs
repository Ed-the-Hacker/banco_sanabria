using BancoSanabria.Application.Strategies;
using FluentAssertions;
using Moq;
using Xunit;

namespace BancoSanabria.Tests.Strategies
{
    public class CreditoStrategyTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMovimientoRepository> _movimientoRepositoryMock;
        private readonly CreditoStrategy _creditoStrategy;

        public CreditoStrategyTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _movimientoRepositoryMock = new Mock<IMovimientoRepository>();
            
            _unitOfWorkMock.Setup(u => u.Movimientos).Returns(_movimientoRepositoryMock.Object);
            
            _creditoStrategy = new CreditoStrategy(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task ValidarMovimientoAsync_CreditoSiempreValido_NoDeberiaLanzarExcepcion()
        {
            // Arrange
            var cuenta = new Cuenta
            {
                CuentaId = 1,
                NumeroCuenta = "123456",
                SaldoInicial = 0m
            };

            // Act & Assert
            await _creditoStrategy.ValidarMovimientoAsync(cuenta, 1000m, DateTime.Now);
            
            // Si no lanza excepción, la prueba pasa
        }

        [Fact]
        public async Task CalcularNuevoSaldoAsync_DeberiaSumarValorAlSaldo()
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
            var nuevoSaldo = await _creditoStrategy.CalcularNuevoSaldoAsync(cuenta, 500m);

            // Assert
            nuevoSaldo.Should().Be(2000m); // 1500 + 500
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
            var nuevoSaldo = await _creditoStrategy.CalcularNuevoSaldoAsync(cuenta, 500m);

            // Assert
            nuevoSaldo.Should().Be(1500m); // 1000 + 500
        }

        [Fact]
        public void TipoMovimiento_DeberiaSerCredito()
        {
            // Assert
            _creditoStrategy.TipoMovimiento.Should().Be("Credito");
        }
    }
}

