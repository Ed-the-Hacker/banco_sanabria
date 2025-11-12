import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { MovimientosRoutingModule } from './movimientos-routing.module';
import { MovimientosListComponent } from './components/movimientos-list/movimientos-list.component';
import { MovimientoFormComponent } from './components/movimiento-form/movimiento-form.component';

@NgModule({
  declarations: [
    MovimientosListComponent,
    MovimientoFormComponent
  ],
  imports: [
    SharedModule,
    MovimientosRoutingModule
  ]
})
export class MovimientosModule {}

