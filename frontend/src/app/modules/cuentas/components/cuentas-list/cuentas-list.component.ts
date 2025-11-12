import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { CuentaService } from '@core/services/cuenta.service';
import { NotificationService } from '@core/services/notification.service';
import { Cuenta } from '@core/models/cuenta.model';

@Component({
  selector: 'app-cuentas-list',
  templateUrl: './cuentas-list.component.html',
  styleUrls: ['./cuentas-list.component.scss']
})
export class CuentasListComponent implements OnInit, OnDestroy {
  cuentas: Cuenta[] = [];
  loading = false;
  searchText = '';
  private destroy$ = new Subject<void>();

  constructor(
    private cuentaService: CuentaService,
    private notificationService: NotificationService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.queryParams.pipe(takeUntil(this.destroy$)).subscribe(params => {
      const clienteId = params['clienteId'];
      if (clienteId) {
        this.loadByCliente(clienteId);
      } else {
        this.loadCuentas();
      }
    });
  }

  loadCuentas(): void {
    this.loading = true;
    this.cuentaService.getAll().pipe(takeUntil(this.destroy$)).subscribe({
      next: cuentas => {
        this.cuentas = cuentas;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  loadByCliente(clienteId: number): void {
    this.loading = true;
    this.cuentaService.getByClienteId(clienteId).pipe(takeUntil(this.destroy$)).subscribe({
      next: cuentas => {
        this.cuentas = cuentas;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  onEdit(cuenta: Cuenta): void {
    this.router.navigate(['/cuentas/editar', cuenta.cuentaId]);
  }

  onDelete(cuenta: Cuenta): void {
    if (confirm(`¿Eliminar la cuenta ${cuenta.numeroCuenta}?`)) {
      this.cuentaService.delete(cuenta.cuentaId).pipe(takeUntil(this.destroy$)).subscribe({
        next: () => {
          this.notificationService.success('Cuenta eliminada correctamente');
          this.loadCuentas();
        }
      });
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

