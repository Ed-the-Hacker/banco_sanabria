export interface Cuenta {
  cuentaId: number;
  numeroCuenta: string;
  tipoCuenta: string;
  saldoInicial: number;
  estado: boolean;
  clienteId: number;
  nombreCliente?: string;
}

export interface CuentaCreate {
  numeroCuenta: string;
  tipoCuenta: string;
  saldoInicial: number;
  estado: boolean;
  clienteId: number;
}

export interface CuentaUpdate {
  tipoCuenta?: string;
  saldoInicial?: number;
  estado?: boolean;
}

