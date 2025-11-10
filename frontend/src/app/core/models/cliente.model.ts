export interface Cliente {
  clienteId: number;
  nombre: string;
  genero: string;
  edad: number;
  identificacion: string;
  direccion: string;
  telefono: string;
  contrasena: string;
  estado: boolean;
}

export interface ClienteCreate {
  nombre: string;
  genero: string;
  edad: number;
  identificacion: string;
  direccion: string;
  telefono: string;
  contrasena: string;
  estado: boolean;
}

export interface ClienteUpdate {
  nombre?: string;
  genero?: string;
  edad?: number;
  direccion?: string;
  telefono?: string;
  contrasena?: string;
  estado?: boolean;
}

