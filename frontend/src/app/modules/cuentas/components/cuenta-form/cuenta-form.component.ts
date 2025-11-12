import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { CuentaService } from '@core/services/cuenta.service';
import { ClienteService } from '@core/services/cliente.service';
import { NotificationService } from '@core/services/notification.service';
import { Cliente } from '@core/models/cliente.model';

@Component({
  selector: 'app-cuenta-form',
  templateUrl: './cuenta-form.component.html',
  styleUrls: ['./cuenta-form.component.scss']
})
export class CuentaFormComponent implements OnInit, OnDestroy {
  cuentaForm: FormGroup;
  isEditMode = false;
  cuentaId?: number;
  clientes: Cliente[] = [];
  loading = false;
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private cuentaService: CuentaService,
    private clienteService: ClienteService,
    private notificationService: NotificationService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.cuentaForm = this.fb.group({
      numeroCuenta: ['', [Validators.required, Validators.maxLength(20)]],
      tipoCuenta: ['', Validators.required],
      saldoInicial: [0, [Validators.required, Validators.min(0)]],
      estado: [true],
      clienteId: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.clienteService.getAll().pipe(takeUntil(this.destroy$)).subscribe(clientes => {
      this.clientes = clientes.filter(c => c.estado);
    });

    this.route.params.pipe(takeUntil(this.destroy$)).subscribe(params => {
      if (params['id']) {
        this.isEditMode = true;
        this.cuentaId = +params['id'];
        this.loadCuenta();
        this.cuentaForm.get('numeroCuenta')?.disable();
      }
    });
  }

  loadCuenta(): void {
    if (!this.cuentaId) {
      return;
    }

    this.loading = true;
    this.cuentaService.getById(this.cuentaId).pipe(takeUntil(this.destroy$)).subscribe({
      next: cuenta => {
        if (!cuenta) {
          this.router.navigate(['/cuentas']);
          return;
        }
        this.cuentaForm.patchValue({
          numeroCuenta: cuenta.numeroCuenta,
          tipoCuenta: cuenta.tipoCuenta,
          saldoInicial: cuenta.saldoInicial,
          estado: cuenta.estado,
          clienteId: cuenta.clienteId
        });
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.router.navigate(['/cuentas']);
      }
    });
  }

  onSubmit(): void {
    if (this.cuentaForm.invalid) {
      this.cuentaForm.markAllAsTouched();
      this.notificationService.warning('Completa los campos requeridos');
      return;
    }

    this.loading = true;
    const value = {
      ...this.cuentaForm.getRawValue(),
      clienteId: Number(this.cuentaForm.get('clienteId')?.value)
    };

    const request$ = this.isEditMode && this.cuentaId
      ? this.cuentaService.update(this.cuentaId, value)
      : this.cuentaService.create(value);

    request$.pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.notificationService.success(
          this.isEditMode ? 'Cuenta actualizada correctamente' : 'Cuenta creada correctamente'
        );
        this.router.navigate(['/cuentas']);
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/cuentas']);
  }

  get numeroCuenta() {
    return this.cuentaForm.get('numeroCuenta');
  }

  get tipoCuenta() {
    return this.cuentaForm.get('tipoCuenta');
  }

  get saldoInicial() {
    return this.cuentaForm.get('saldoInicial');
  }

  get clienteIdControl() {
    return this.cuentaForm.get('clienteId');
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

