import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { CuentasRoutingModule } from './cuentas-routing.module';
import { CuentasListComponent } from './components/cuentas-list/cuentas-list.component';
import { CuentaFormComponent } from './components/cuenta-form/cuenta-form.component';

@NgModule({
  declarations: [
    CuentasListComponent,
    CuentaFormComponent
  ],
  imports: [
    SharedModule,
    CuentasRoutingModule
  ]
})
export class CuentasModule {}

