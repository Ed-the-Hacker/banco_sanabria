import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ClienteService } from '@core/services/cliente.service';
import { NotificationService } from '@core/services/notification.service';

@Component({
  selector: 'app-cliente-form',
  templateUrl: './cliente-form.component.html',
  styleUrls: ['./cliente-form.component.scss']
})
export class ClienteFormComponent implements OnInit {
  clienteForm: FormGroup;
  isEditMode = false;
  clienteId?: number;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private clienteService: ClienteService,
    private notificationService: NotificationService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.clienteForm = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(100)]],
      genero: ['', Validators.required],
      edad: ['', [Validators.required, Validators.min(18), Validators.max(120)]],
      identificacion: ['', [Validators.required, Validators.maxLength(20)]],
      direccion: ['', Validators.maxLength(200)],
      telefono: ['', Validators.maxLength(20)],
      contrasena: ['', [Validators.required, Validators.minLength(4)]],
      estado: [true]
    });
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.isEditMode = true;
        this.clienteId = +params['id'];
        this.loadCliente();
        // En modo edición, la contraseña es opcional
        this.clienteForm.get('contrasena')?.clearValidators();
        this.clienteForm.get('contrasena')?.updateValueAndValidity();
      }
    });
  }

  loadCliente(): void {
    if (!this.clienteId) return;

    this.loading = true;
    this.clienteService.getById(this.clienteId).subscribe({
      next: (cliente) => {
        this.clienteForm.patchValue({
          nombre: cliente.nombre,
          genero: cliente.genero,
          edad: cliente.edad,
          identificacion: cliente.identificacion,
          direccion: cliente.direccion,
          telefono: cliente.telefono,
          estado: cliente.estado
        });
        // Deshabilitar identificación en modo edición
        this.clienteForm.get('identificacion')?.disable();
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.router.navigate(['/clientes']);
      }
    });
  }

  onSubmit(): void {
    if (this.clienteForm.invalid) {
      this.clienteForm.markAllAsTouched();
      this.notificationService.warning('Por favor complete todos los campos requeridos');
      return;
    }

    this.loading = true;
    const formValue = this.clienteForm.getRawValue();

    // Si está en modo edición y no se cambió la contraseña, no enviarla
    if (this.isEditMode && !formValue.contrasena) {
      delete formValue.contrasena;
    }

    const request$ = this.isEditMode && this.clienteId
      ? this.clienteService.update(this.clienteId, formValue)
      : this.clienteService.create(formValue);

    request$.subscribe({
      next: () => {
        this.notificationService.success(
          this.isEditMode ? 'Cliente actualizado exitosamente' : 'Cliente creado exitosamente'
        );
        this.router.navigate(['/clientes']);
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/clientes']);
  }

  // Getters para validaciones en el template
  get nombre() { return this.clienteForm.get('nombre'); }
  get genero() { return this.clienteForm.get('genero'); }
  get edad() { return this.clienteForm.get('edad'); }
  get identificacion() { return this.clienteForm.get('identificacion'); }
  get direccion() { return this.clienteForm.get('direccion'); }
  get telefono() { return this.clienteForm.get('telefono'); }
  get contrasena() { return this.clienteForm.get('contrasena'); }
  get estado() { return this.clienteForm.get('estado'); }
}

