import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Login } from '../models/login.model';
import { RegisterUser } from '../models/register-user.model';
import { AuthResponse } from '../models/auth-response.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private api = environment.apiUrl;

  constructor(private http: HttpClient) {}

  register(data: RegisterUser): Observable<any> {
    // ensure role default
    const payload = { ...data, role: data.role ?? 'User' };
    return this.http.post(`${this.api}/auth/register`, payload, { responseType: 'text' as 'json' });
  }

  login(data: Login): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.api}/auth/login`, data).pipe(
      tap((response: AuthResponse) => this.setSession(response))
    );
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('email');
    localStorage.removeItem('role');
    localStorage.removeItem('expiration');
  }

  setSession(response: AuthResponse): void {
    localStorage.setItem('token', response.token);
    localStorage.setItem('email', response.email);
    localStorage.setItem('role', response.role);
    localStorage.setItem('expiration', response.expiration);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getRole(): string | null {
    return localStorage.getItem('role');
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;
    const exp = localStorage.getItem('expiration');
    if (!exp) return true;
    try {
      const expDate = new Date(exp);
      return expDate > new Date();
    } catch {
      return true;
    }
  }
}
