using BancoSanabria.Application.DTOs;
using BancoSanabria.Application.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BancoSanabria.Infrastructure.Services
{
    public class PdfGeneratorService : IPdfGeneratorService
    {
        public PdfGeneratorService()
        {
            // Configurar licencia de QuestPDF (Community)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<string> GenerarPdfReporteAsync(ReporteDto reporte, ReporteRequestDto request)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // Encabezado
                    page.Header()
                        .Column(column =>
                        {
                            column.Item().Text("Estado de Cuenta")
                                .FontSize(20)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            column.Item().Text($"Período: {request.FechaInicio:dd/MM/yyyy} - {request.FechaFin:dd/MM/yyyy}")
                                .FontSize(12);

                            column.Item().PaddingVertical(5).LineHorizontal(1);
                        });

                    // Contenido
                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            // Información del cliente
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Cliente:").Bold();
                                    col.Item().Text(reporte.Cliente.Nombre);
                                });
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Identificación:").Bold();
                                    col.Item().Text(reporte.Cliente.Identificacion);
                                });
                            });

                            column.Item().PaddingVertical(5);

                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Dirección:").Bold();
                                    col.Item().Text(reporte.Cliente.Direccion);
                                });
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Teléfono:").Bold();
                                    col.Item().Text(reporte.Cliente.Telefono);
                                });
                            });

                            column.Item().PaddingVertical(10);

                            // Cuentas y movimientos
                            foreach (var cuenta in reporte.Cuentas)
                            {
                                column.Item().Element(container => GenerarSeccionCuenta(container, cuenta));
                                column.Item().PaddingVertical(5);
                            }
                        });

                    // Pie de página
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                        });
                });
            });

            // Generar PDF en memoria
            var pdfBytes = document.GeneratePdf();
            
            // Convertir a Base64
            return await Task.FromResult(Convert.ToBase64String(pdfBytes));
        }

        private void GenerarSeccionCuenta(IContainer container, CuentaReporteDto cuenta)
        {
            container.Column(column =>
            {
                // Información de la cuenta
                column.Item().Background(Colors.Grey.Lighten3).Padding(5)
                    .Row(row =>
                    {
                        row.RelativeItem().Text($"Cuenta: {cuenta.NumeroCuenta}").Bold();
                        row.RelativeItem().Text($"Tipo: {cuenta.TipoCuenta}").Bold();
                        row.RelativeItem().Text($"Estado: {(cuenta.Estado ? "Activa" : "Inactiva")}").Bold();
                    });

                column.Item().PaddingVertical(5);

                // Resumen de saldos
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text($"Saldo Inicial: ${cuenta.SaldoInicial:N2}");
                    row.RelativeItem().Text($"Total Créditos: ${cuenta.TotalCreditos:N2}").FontColor(Colors.Green.Darken2);
                    row.RelativeItem().Text($"Total Débitos: ${cuenta.TotalDebitos:N2}").FontColor(Colors.Red.Darken2);
                    row.RelativeItem().Text($"Saldo Disponible: ${cuenta.SaldoDisponible:N2}").Bold();
                });

                column.Item().PaddingVertical(5);

                // Tabla de movimientos
                if (cuenta.Movimientos.Any())
                {
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Fecha
                            columns.RelativeColumn(2); // Tipo
                            columns.RelativeColumn(2); // Valor
                            columns.RelativeColumn(2); // Saldo
                        });

                        // Encabezado de tabla
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Fecha").Bold();
                            header.Cell().Element(CellStyle).Text("Tipo").Bold();
                            header.Cell().Element(CellStyle).Text("Valor").Bold();
                            header.Cell().Element(CellStyle).Text("Saldo").Bold();
                        });

                        // Filas de movimientos
                        foreach (var movimiento in cuenta.Movimientos)
                        {
                            table.Cell().Element(CellStyle).Text(movimiento.Fecha.ToString("dd/MM/yyyy HH:mm"));
                            table.Cell().Element(CellStyle).Text(movimiento.TipoMovimiento);
                            table.Cell().Element(CellStyle).Text($"${movimiento.Valor:N2}");
                            table.Cell().Element(CellStyle).Text($"${movimiento.Saldo:N2}");
                        }
                    });
                }
                else
                {
                    column.Item().Text("No hay movimientos en este período").Italic();
                }
            });
        }

        private IContainer CellStyle(IContainer container)
        {
            return container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
        }
    }
}

