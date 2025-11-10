import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ClienteService } from '@core/services/cliente.service';
import { NotificationService } from '@core/services/notification.service';
import { Cliente } from '@core/models/cliente.model';

@Component({
  selector: 'app-clientes-list',
  templateUrl: './clientes-list.component.html',
  styleUrls: ['./clientes-list.component.scss']
})
export class ClientesListComponent implements OnInit {
  clientes: Cliente[] = [];
  loading = false;
  searchText = '';

  constructor(
    private clienteService: ClienteService,
    private notificationService: NotificationService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadClientes();
  }

  loadClientes(): void {
    this.loading = true;
    this.clienteService.getAll().subscribe({
      next: (clientes) => {
        this.clientes = clientes;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  onEdit(cliente: Cliente): void {
    this.router.navigate(['/clientes/editar', cliente.clienteId]);
  }

  onDelete(cliente: Cliente): void {
    if (confirm(`¿Está seguro de eliminar al cliente ${cliente.nombre}?`)) {
      this.clienteService.delete(cliente.clienteId).subscribe({
        next: () => {
          this.notificationService.success('Cliente eliminado exitosamente');
          this.loadClientes();
        }
      });
    }
  }

  onViewCuentas(cliente: Cliente): void {
    this.router.navigate(['/cuentas'], { queryParams: { clienteId: cliente.clienteId } });
  }
}

