import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ClienteService } from '@core/services/cliente.service';
import { ReporteService } from '@core/services/reporte.service';
import { NotificationService } from '@core/services/notification.service';
import { Cliente } from '@core/models/cliente.model';
import { Reporte, ReporteRequest, CuentaReporte } from '@core/models/reporte.model';

@Component({
  selector: 'app-reportes',
  templateUrl: './reportes.component.html',
  styleUrls: ['./reportes.component.scss']
})
export class ReportesComponent {
  reporteForm: FormGroup;
  clientes: Cliente[] = [];
  reporte?: Reporte;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private clienteService: ClienteService,
    private reporteService: ReporteService,
    private notificationService: NotificationService
  ) {
    const hoy = new Date();
    const inicioMes = new Date(hoy.getFullYear(), hoy.getMonth(), 1);

    this.reporteForm = this.fb.group({
      clienteId: ['', Validators.required],
      fechaInicio: [inicioMes.toISOString().substring(0, 10), Validators.required],
      fechaFin: [hoy.toISOString().substring(0, 10), Validators.required]
    });

    this.clienteService.getAll().subscribe(clientes => {
      this.clientes = clientes.filter(c => c.estado);
    });
  }

  generarReporte(): void {
    if (this.reporteForm.invalid) {
      this.reporteForm.markAllAsTouched();
      this.notificationService.warning('Completa los campos requeridos');
      return;
    }

    const formValue = this.reporteForm.value;
    const request: ReporteRequest = {
      clienteId: Number(formValue.clienteId),
      fechaInicio: formValue.fechaInicio,
      fechaFin: formValue.fechaFin
    };

    this.loading = true;
    this.reporteService.generarReporte(request).subscribe({
      next: reporte => {
        this.reporte = reporte;
        this.loading = false;
        this.notificationService.success('Reporte generado correctamente');
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  descargarPdf(): void {
    if (!this.reporte?.pdfBase64) {
      this.notificationService.warning('Debes generar un reporte primero');
      return;
    }

    const fileName = `reporte-${this.reporte?.cliente.identificacion}-${this.reporteForm.value.fechaInicio}-${this.reporteForm.value.fechaFin}.pdf`;
    this.reporteService.descargarPdf(this.reporte.pdfBase64, fileName);
  }

  trackByCuenta(_index: number, cuenta: CuentaReporte): string {
    return cuenta.numeroCuenta;
  }
}

