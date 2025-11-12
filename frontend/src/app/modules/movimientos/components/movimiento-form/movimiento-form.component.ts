import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { MovimientoService } from '@core/services/movimiento.service';
import { CuentaService } from '@core/services/cuenta.service';
import { NotificationService } from '@core/services/notification.service';
import { Cuenta } from '@core/models/cuenta.model';

@Component({
  selector: 'app-movimiento-form',
  templateUrl: './movimiento-form.component.html',
  styleUrls: ['./movimiento-form.component.scss']
})
export class MovimientoFormComponent implements OnInit, OnDestroy {
  movimientoForm: FormGroup;
  cuentas: Cuenta[] = [];
  loading = false;
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private movimientoService: MovimientoService,
    private cuentaService: CuentaService,
    private notificationService: NotificationService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    const today = new Date().toISOString().substring(0, 16);
    this.movimientoForm = this.fb.group({
      fecha: [today, Validators.required],
      tipoMovimiento: ['Credito', Validators.required],
      valor: [null, [Validators.required, Validators.min(0.01)]],
      cuentaId: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.cuentaService.getAll().pipe(takeUntil(this.destroy$)).subscribe(cuentas => {
      this.cuentas = cuentas.filter(c => c.estado);
    });

    this.route.params.pipe(takeUntil(this.destroy$)).subscribe(params => {
      const cuentaId = params['cuentaId'];
      if (cuentaId) {
        this.movimientoForm.patchValue({ cuentaId: Number(cuentaId) });
      }
    });
  }

  onSubmit(): void {
    if (this.movimientoForm.invalid) {
      this.movimientoForm.markAllAsTouched();
      this.notificationService.warning('Completa los campos requeridos');
      return;
    }

    this.loading = true;
    const raw = this.movimientoForm.value;

    const payload = {
      fecha: new Date(raw.fecha).toISOString(),
      tipoMovimiento: raw.tipoMovimiento,
      valor: Number(raw.valor),
      cuentaId: Number(raw.cuentaId)
    };

    this.movimientoService.create(payload).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.notificationService.success('Movimiento registrado correctamente');
        this.router.navigate(['/movimientos'], {
          queryParams: this.route.snapshot.params['cuentaId']
            ? { cuentaId: this.route.snapshot.params['cuentaId'] }
            : undefined
        });
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/movimientos'], {
      queryParams: this.route.snapshot.params['cuentaId']
        ? { cuentaId: this.route.snapshot.params['cuentaId'] }
        : undefined
    });
  }

  get fecha() {
    return this.movimientoForm.get('fecha');
  }

  get tipoMovimiento() {
    return this.movimientoForm.get('tipoMovimiento');
  }

  get valor() {
    return this.movimientoForm.get('valor');
  }

  get cuentaIdControl() {
    return this.movimientoForm.get('cuentaId');
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

