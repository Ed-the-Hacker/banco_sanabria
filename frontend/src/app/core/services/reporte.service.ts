import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';
import { Reporte, ReporteRequest } from '../models/reporte.model';

@Injectable({
  providedIn: 'root'
})
export class ReporteService {
  private readonly apiUrl = `${environment.apiUrl}/reportes`;

  constructor(private http: HttpClient) {}

  generarReporte(request: ReporteRequest): Observable<Reporte> {
    const params = new HttpParams()
      .set('fechaInicio', this.formatDate(request.fechaInicio))
      .set('fechaFin', this.formatDate(request.fechaFin))
      .set('clienteId', request.clienteId.toString());

    return this.http.get<Reporte>(this.apiUrl, { params });
  }

  descargarPdf(pdfBase64: string, nombreArchivo: string = 'reporte.pdf'): void {
    const byteCharacters = atob(pdfBase64);
    const byteNumbers = new Array(byteCharacters.length);
    
    for (let i = 0; i < byteCharacters.length; i++) {
      byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: 'application/pdf' });
    
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = nombreArchivo;
    link.click();
    
    URL.revokeObjectURL(link.href);
  }

  private formatDate(date: string | Date): string {
    if (typeof date === 'string') {
      return date.split('T')[0];
    }
    return date.toISOString().split('T')[0];
  }
}

