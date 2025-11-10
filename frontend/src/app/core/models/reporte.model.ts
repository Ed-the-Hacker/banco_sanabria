export interface ReporteRequest {
  fechaInicio: string | Date;
  fechaFin: string | Date;
  clienteId: number;
}

export interface Reporte {
  cliente: ClienteReporte;
  cuentas: CuentaReporte[];
  pdfBase64: string;
}

export interface ClienteReporte {
  nombre: string;
  identificacion: string;
  direccion: string;
  telefono: string;
}

export interface CuentaReporte {
  numeroCuenta: string;
  tipoCuenta: string;
  saldoInicial: number;
  saldoDisponible: number;
  estado: boolean;
  movimientos: MovimientoReporte[];
  totalCreditos: number;
  totalDebitos: number;
}

export interface MovimientoReporte {
  fecha: Date | string;
  tipoMovimiento: string;
  valor: number;
  saldo: number;
}

