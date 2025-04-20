import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { ShareService } from '../services/share.service';
import { UserRes } from '../types';
import { Router, RouterLink } from '@angular/router';
import { NgIf } from '@angular/common';
import { NzButtonComponent } from 'ng-zorro-antd/button';
import { TextIconComponent } from './text-icon.component';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzIconDirective } from 'ng-zorro-antd/icon';
import { NzMenuDirective, NzMenuDividerDirective, NzMenuItemComponent } from 'ng-zorro-antd/menu';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'ss-navbar',
  standalone: true,
  imports: [
    NgIf,
    NzButtonComponent,
    TextIconComponent,
    NzIconDirective,
    NzDropDownModule,
    NzMenuDirective,
    NzMenuItemComponent,
    NzMenuDividerDirective,
    RouterLink,
  ],
  template: `
    <div class="navbar">
      <div class="left">
        <a routerLink="/"><img src="/images/logo.jpg"/></a>
        Seamless Share
      </div>

      <div class="right">
        <ng-container *ngIf="isAuthenticated">
          <div class="user-info">
            <a nz-dropdown [nzDropdownMenu]="menu" nzTrigger="hover" nzPlacement="bottomRight">
              <ss-text-icon name="{{ user!.firstName }} {{ user!.lastName }}"></ss-text-icon>
              <span nz-icon nzType="down" style="font-size: 1rem; font-weight: bolder"></span>
            </a>
            <nz-dropdown-menu #menu="nzDropdownMenu">
              <ul nz-menu>
                <li nz-menu-item (click)="goToMyShare()">My Share</li>
                <li nz-menu-divider></li>
                <li nz-menu-item (click)="logout()">Log Out</li>
              </ul>
            </nz-dropdown-menu>
          </div>
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
      justify-content: space-between;
      padding: 10px 20px;

      .left {
        display: flex;
        align-items: center;
        gap: 1rem;
        font-size: 1.5rem;
        font-weight: bold;
        color: #333;

        img {
          width: 2.5rem;
        }
      }

      .right {
        display: flex;
        align-items: center;
        gap: 1rem;
      }

      .user-info {
        display: flex;
        align-items: center;
        gap: 0.5rem;

        a {
          display: flex;
          align-items: center;
          gap: 0.1rem;
        }
      }
    }
  `
})
export class NavbarComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);
  private readonly shareService = inject(ShareService);
  private readonly messageService = inject(NzMessageService);

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

  async goToMyShare() {
    try {
      const share = await this.shareService.getOwnedShare();
      await this.router.navigate([ 'shares', share.code ]);
    } catch (e) {
      console.error(e);
      this.messageService.error('Failed to load share');
    }
  }

  async logout() {
    this.authService.clearAuth();
    await this.router.navigate([ '/' ]);
  }
}
