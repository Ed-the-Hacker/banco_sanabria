import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CuentasListComponent } from './components/cuentas-list/cuentas-list.component';
import { CuentaFormComponent } from './components/cuenta-form/cuenta-form.component';

const routes: Routes = [
  {
    path: '',
    component: CuentasListComponent
  },
  {
    path: 'nuevo',
    component: CuentaFormComponent
  },
  {
    path: 'editar/:id',
    component: CuentaFormComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CuentasRoutingModule {}

