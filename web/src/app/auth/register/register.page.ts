import { Component, inject } from '@angular/core';
import { NonNullableFormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { Router } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { RegisterReq } from '../../types';

@Component({
  selector: 'ss-register',
  standalone: false,
  template: `
    <h2 class="title">
      Sign Up
    </h2>
    <form nz-form [formGroup]="validateForm" (ngSubmit)="submitForm()">
      <nz-form-item>
        <nz-form-control nzErrorTip="Please input your first name!">
          <nz-input-group>
            <input type="text" nz-input formControlName="firstName" placeholder="First/Given Name"/>
          </nz-input-group>
        </nz-form-control>
      </nz-form-item>
      <nz-form-item>
        <nz-form-control nzErrorTip="Please input your last name!">
          <nz-input-group>
            <input type="text" nz-input formControlName="lastName" placeholder="Last Name/Surname"/>
          </nz-input-group>
        </nz-form-control>
      </nz-form-item>
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

      <button nz-button class="register-form-button" [nzType]="'primary'">Register</button>
      Or have an existing account?
      <a routerLink="/auth/login">login!</a>
    </form>
  `,
  styles: `
    .register-form-button {
      width: 100%;
      margin-top: 0.25rem;
      margin-bottom: 0.5rem;
    }

    .title {
      text-align: center;
      font-weight: 500;
      margin-bottom: 1.2rem;
    }
  `
})
export class RegisterPage {
  private formBuilder = inject(NonNullableFormBuilder);
  private authService = inject(AuthService);
  private readonly notificationService = inject(NzNotificationService);
  private readonly router = inject(Router);

  validateForm = this.formBuilder.group({
    emailAddress: this.formBuilder.control('', [ Validators.required, Validators.email ]),
    password: this.formBuilder.control('', [ Validators.required ]),
    firstName: this.formBuilder.control('', []),
    lastName: this.formBuilder.control('', []),
  });

  constructor(title: Title) {
    title.setTitle('Seamless Share | Register');
  }

  async submitForm(): Promise<void> {
    if (!this.validateForm.valid) {
      Object.values(this.validateForm.controls)
        .forEach(control => {
          if (control.invalid) {
            control.markAsDirty();
            control.updateValueAndValidity({ onlySelf: true });
          }
        });
      return;
    }

    try {
      const res = await this.authService.register(this.validateForm.value as RegisterReq);
      this.notificationService.create(
        'success',
        'Success',
        'You have successfully created an account'
      );
      this.authService.persistAuth(res);
      this.validateForm.reset();

      // navigate to the home page
      await this.router.navigate([ '' ]);
    } catch (e: any) {
      this.notificationService.create(
        'error',
        'Error',
        e.error?.message ?? 'Failed to create an account'
      );
    }
  }
}
