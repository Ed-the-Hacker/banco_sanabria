import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MovimientosListComponent } from './components/movimientos-list/movimientos-list.component';
import { MovimientoFormComponent } from './components/movimiento-form/movimiento-form.component';

const routes: Routes = [
  {
    path: '',
    component: MovimientosListComponent
  },
  {
    path: 'nuevo',
    component: MovimientoFormComponent
  },
  {
    path: 'cuenta/:cuentaId/nuevo',
    component: MovimientoFormComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MovimientosRoutingModule {}

