import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environment';
import { AuthRes, LoginReq, RegisterReq } from '../types';
import { firstValueFrom } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = environment.apiV1BaseUrl;

  constructor(private http: HttpClient) {
  }

  async login(req: LoginReq): Promise<AuthRes> {
    return firstValueFrom(this.http.post<AuthRes>(`${this.baseUrl}/auth/login`, req));
  }

  async register(req: RegisterReq): Promise<AuthRes> {
    return firstValueFrom(this.http.post<AuthRes>(`${this.baseUrl}/auth/register`, req));
  }
}
