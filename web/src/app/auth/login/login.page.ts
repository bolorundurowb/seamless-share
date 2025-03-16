import { Component, inject } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { NonNullableFormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { LoginReq } from '../../types';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { Router } from '@angular/router';

@Component({
  selector: 'ss-login',
  standalone: false,
  template: `
    <h2 class="title">
      Log In
    </h2>
    <form nz-form [formGroup]="validateForm" (ngSubmit)="submitForm()">
      <nz-form-item>
        <nz-form-control nzErrorTip="Please input your email address!">
          <nz-input-group>
            <input type="email" nz-input formControlName="emailAddress" placeholder="Email Address"/>
          </nz-input-group>
        </nz-form-control>
      </nz-form-item>
      <nz-form-item>
        <nz-form-control nzErrorTip="Please input your Password!">
          <nz-input-group>
            <input type="password" nz-input formControlName="password" placeholder="Password"/>
          </nz-input-group>
        </nz-form-control>
      </nz-form-item>

      <div class="login-form-forgot">
        <a>forgot password?</a>
      </div>

      <button nz-button class="login-form-button" [nzType]="'primary'">Log in</button>
      Or
      <a routerLink="/auth/register">register now!</a>
    </form>
  `,
  styles: `
    .login-form-button {
      width: 100%;
      margin-top: 0.25rem;
      margin-bottom: 0.25rem;
    }

    .login-form-forgot {
      text-align: end;
      margin-bottom: 0.75rem;
    }

    .title {
      text-align: center;
      font-weight: 500;
      margin-bottom: 1.2rem;
    }
  `
})
export class LoginPage {
  private formBuilder = inject(NonNullableFormBuilder);
  private authService = inject(AuthService);
  private readonly notificationService = inject(NzNotificationService);
  private readonly router = inject(Router);

  validateForm = this.formBuilder.group({
    emailAddress: this.formBuilder.control('', [ Validators.required, Validators.email ]),
    password: this.formBuilder.control('', [ Validators.required ]),
  });

  constructor(title: Title) {
    title.setTitle('Seamless Share | Login');
  }

  async submitForm(): Promise<void> {
    if (!this.validateForm.valid) {
      Object.values(this.validateForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      return;
    }

    try {
      const res = await this.authService.login(this.validateForm.value as LoginReq);
      this.notificationService.create(
        'success',
        'Success',
        'You have successfully logged in'
      );
      this.authService.persistAuth(res);
      this.validateForm.reset();

      // navigate to the home page
      await this.router.navigate([ '' ]);
    } catch (e: any) {
      this.notificationService.create(
        'error',
        'Error',
        e.error?.message ?? 'Failed to log in'
      );
    }
  }
}
