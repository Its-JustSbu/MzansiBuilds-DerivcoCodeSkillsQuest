import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class Api {
  http = inject(HttpClient);

  API_URI = "https://localhost:7203/api";

  get(route: string): Observable<any> {
    return this.http.get(`${this.API_URI}/${route}`);
  }

  post(route: string, data: any): Observable<any> {
    return this.http.post(`${this.API_URI}/${route}`, data);
  }

  put(route: string, data: any): Observable<any> {
    return this.http.put(`${this.API_URI}/${route}`, data);
  }

  delete(route: string, data: any = {}): Observable<any> {
    return this.http.delete(`${this.API_URI}/${route}`, data);
  }
}
