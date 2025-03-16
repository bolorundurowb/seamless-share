import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { ShareService } from '../services/share.service';
import { UserRes } from '../types';
import { Router } from '@angular/router';
import { NgIf } from '@angular/common';
import { NzButtonComponent } from 'ng-zorro-antd/button';
import { TextIconComponent } from './text-icon.component';

@Component({
  selector: 'ss-navbar',
  standalone: true,
  imports: [
    NgIf,
    NzButtonComponent,
    TextIconComponent
  ],
  template: `
    <div class="navbar">
      <div class="actions">
        <ng-container *ngIf="isAuthenticated">
          <ss-text-icon name="{{ user!.firstName }} {{ user!.lastName }}"></ss-text-icon>
        </ng-container>
        <ng-container *ngIf="!isAuthenticated">
          <button nz-button nzType="primary" (click)="goToLogin()">Login</button>
          <button nz-button (click)="goToRegister()">Register</button>
        </ng-container>
      </div>
    </div>
  `,
  styles: `
    .navbar {
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 3.5rem;
      background-color: #ffffff;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
      z-index: 1000;
      display: flex;
      padding: 10px 20px;
      justify-content: end;

      .actions {
        display: flex;
        gap: 1rem;
        height: 100%;
        align-items: center;
      }
    }
  `
})
export class NavbarComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);
  private readonly shareService = inject(ShareService);

  isAuthenticated = false;
  user: UserRes | null = null;

  async ngOnInit() {
    this.isAuthenticated = this.authService.isAuthenticated();
    this.user = this.authService.getUser();
  }

  async goToLogin() {
    await this.router.navigate([ 'auth', 'login' ]);
  }

  async goToRegister() {
    await this.router.navigate([ 'auth', 'register' ]);
  }
}
