using BancoSanabria.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BancoSanabria.Infrastructure.Data
{
    /// <summary>
    /// Contexto de base de datos para el sistema bancario
    /// </summary>
    public class BancoDbContext : DbContext
    {
        public BancoDbContext(DbContextOptions<BancoDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Cuenta> Cuentas { get; set; } = null!;
        public DbSet<Movimiento> Movimientos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.ClienteId);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Identificacion).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Contrasena).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Direccion).HasMaxLength(200);
                entity.Property(e => e.Telefono).HasMaxLength(20);
                entity.Property(e => e.Genero).HasMaxLength(10);
                
                entity.HasIndex(e => e.Identificacion).IsUnique();
            });

            // Configuración de Cuenta
            modelBuilder.Entity<Cuenta>(entity =>
            {
                entity.HasKey(e => e.CuentaId);
                entity.Property(e => e.NumeroCuenta).IsRequired().HasMaxLength(20);
                entity.Property(e => e.TipoCuenta).IsRequired().HasMaxLength(50);
                entity.Property(e => e.SaldoInicial).HasColumnType("decimal(18,2)");
                
                entity.HasIndex(e => e.NumeroCuenta).IsUnique();

                // Relación: Una cuenta pertenece a un cliente
                entity.HasOne(e => e.Cliente)
                    .WithMany(c => c.Cuentas)
                    .HasForeignKey(e => e.ClienteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Movimiento
            modelBuilder.Entity<Movimiento>(entity =>
            {
                entity.HasKey(e => e.MovimientoId);
                entity.Property(e => e.TipoMovimiento).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Valor).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Saldo).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Fecha).IsRequired();

                // Relación: Un movimiento pertenece a una cuenta
                entity.HasOne(e => e.Cuenta)
                    .WithMany(c => c.Movimientos)
                    .HasForeignKey(e => e.CuentaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Fecha);
            });
        }
    }
}

