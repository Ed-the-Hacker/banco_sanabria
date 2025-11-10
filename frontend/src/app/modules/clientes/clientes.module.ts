import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { ClientesRoutingModule } from './clientes-routing.module';

import { ClientesListComponent } from './components/clientes-list/clientes-list.component';
import { ClienteFormComponent } from './components/cliente-form/cliente-form.component';

@NgModule({
  declarations: [
    ClientesListComponent,
    ClienteFormComponent
  ],
  imports: [
    SharedModule,
    ClientesRoutingModule
  ]
})
export class ClientesModule {}

