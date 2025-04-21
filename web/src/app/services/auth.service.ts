import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {AuthRes, LoginReq, RegisterReq, UserRes} from '../types';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = environment.apiV1BaseUrl;
  private tokenKey = 'ss-auth-token';
  private tokenExpiryKey = 'ss-token-expiry';
  private userKey = 'ss-user';

  constructor(private http: HttpClient) {
  }

  async login(req: LoginReq): Promise<AuthRes> {
    return firstValueFrom(this.http.post<AuthRes>(`${this.baseUrl}/auth/login`, req));
  }

  async register(req: RegisterReq): Promise<AuthRes> {
    return firstValueFrom(this.http.post<AuthRes>(`${this.baseUrl}/auth/register`, req));
  }

  isAuthenticated(): boolean {
    return !!this.getToken() && !!this.getTokenExpiry() && this.getTokenExpiry()! > new Date();
  }

  persistAuth(authRes: AuthRes): void {
    localStorage.setItem(this.tokenKey, authRes.accessToken);
    localStorage.setItem(this.tokenExpiryKey, authRes.expiresAt);
    localStorage.setItem(this.userKey, JSON.stringify(authRes.user));
  }

  clearAuth(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.tokenExpiryKey);
    localStorage.removeItem(this.userKey);
  }

  getUser(): UserRes | null {
    const rawUser = localStorage.getItem(this.userKey);

    if (!rawUser) {
      return null;
    }

    return JSON.parse(rawUser) as UserRes;
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private getTokenExpiry(): Date | null {
    return localStorage.getItem(this.tokenExpiryKey) ? new Date(localStorage.getItem(this.tokenExpiryKey)!) : null;
  }
}
