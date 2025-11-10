export interface Movimiento {
  movimientoId: number;
  fecha: Date | string;
  tipoMovimiento: string;
  valor: number;
  saldo: number;
  cuentaId: number;
  numeroCuenta?: string;
}

export interface MovimientoCreate {
  fecha: Date | string;
  tipoMovimiento: string;
  valor: number;
  cuentaId: number;
}

