import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LayoutComponent } from './layout/layout.component';

const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadChildren: () => import('./modules/dashboard/dashboard.module').then(m => m.DashboardModule)
      },
      {
        path: 'clientes',
        loadChildren: () => import('./modules/clientes/clientes.module').then(m => m.ClientesModule)
      },
      {
        path: 'cuentas',
        loadChildren: () => import('./modules/cuentas/cuentas.module').then(m => m.CuentasModule)
      },
      {
        path: 'movimientos',
        loadChildren: () => import('./modules/movimientos/movimientos.module').then(m => m.MovimientosModule)
      },
      {
        path: 'reportes',
        loadChildren: () => import('./modules/reportes/reportes.module').then(m => m.ReportesModule)
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}

