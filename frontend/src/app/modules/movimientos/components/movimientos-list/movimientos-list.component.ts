import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { MovimientoService } from '@core/services/movimiento.service';
import { NotificationService } from '@core/services/notification.service';
import { Movimiento } from '@core/models/movimiento.model';

@Component({
  selector: 'app-movimientos-list',
  templateUrl: './movimientos-list.component.html',
  styleUrls: ['./movimientos-list.component.scss']
})
export class MovimientosListComponent implements OnInit, OnDestroy {
  movimientos: Movimiento[] = [];
  loading = false;
  searchText = '';
  cuentaIdFiltro?: number;
  private destroy$ = new Subject<void>();

  constructor(
    private movimientoService: MovimientoService,
    private notificationService: NotificationService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.queryParams.pipe(takeUntil(this.destroy$)).subscribe(params => {
      const cuentaId = params['cuentaId'];
      if (cuentaId) {
        this.cuentaIdFiltro = +cuentaId;
        this.loadByCuenta(+cuentaId);
      } else {
        this.cuentaIdFiltro = undefined;
        this.loadMovimientos();
      }
    });
  }

  loadMovimientos(): void {
    this.loading = true;
    this.movimientoService.getAll().pipe(takeUntil(this.destroy$)).subscribe({
      next: movimientos => {
        this.movimientos = movimientos;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  loadByCuenta(cuentaId: number): void {
    this.loading = true;
    this.movimientoService.getByCuentaId(cuentaId).pipe(takeUntil(this.destroy$)).subscribe({
      next: movimientos => {
        this.movimientos = movimientos;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  onNuevoMovimiento(): void {
    if (this.cuentaIdFiltro) {
      this.router.navigate(['/movimientos/cuenta', this.cuentaIdFiltro, 'nuevo']);
    } else {
      this.router.navigate(['/movimientos/nuevo']);
    }
  }

  onDelete(movimiento: Movimiento): void {
    if (confirm('Solo se puede eliminar el último movimiento. ¿Confirmas eliminarlo?')) {
      this.movimientoService.delete(movimiento.movimientoId).pipe(takeUntil(this.destroy$)).subscribe({
        next: () => {
          this.notificationService.success('Movimiento eliminado correctamente');
          this.cuentaIdFiltro ? this.loadByCuenta(this.cuentaIdFiltro) : this.loadMovimientos();
        }
      });
    }
  }

  trackById(_index: number, movimiento: Movimiento): number {
    return movimiento.movimientoId;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

