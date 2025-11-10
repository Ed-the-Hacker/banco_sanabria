import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ClienteService } from '@core/services/cliente.service';
import { CuentaService } from '@core/services/cuenta.service';
import { MovimientoService } from '@core/services/movimiento.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  stats = {
    clientes: 0,
    cuentas: 0,
    movimientos: 0
  };
  loading = true;

  constructor(
    private clienteService: ClienteService,
    private cuentaService: CuentaService,
    private movimientoService: MovimientoService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    Promise.all([
      this.clienteService.getAll().toPromise(),
      this.cuentaService.getAll().toPromise(),
      this.movimientoService.getAll().toPromise()
    ]).then(([clientes, cuentas, movimientos]) => {
      this.stats = {
        clientes: clientes?.length || 0,
        cuentas: cuentas?.length || 0,
        movimientos: movimientos?.length || 0
      };
      this.loading = false;
    }).catch(() => {
      this.loading = false;
    });
  }

  navigateTo(path: string): void {
    this.router.navigate([path]);
  }
}

