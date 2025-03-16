import { Component, inject } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { NonNullableFormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'ss-login',
  standalone: false,
  template: `
    <h2 class="title">
      Log In
    </h2>
    <form nz-form [formGroup]="validateForm" class="login-form" (ngSubmit)="submitForm()">
      <nz-form-item>
        <nz-form-control nzErrorTip="Please input your username!">
          <nz-input-group nzPrefixIcon="user">
            <input type="text" nz-input formControlName="username" placeholder="Username"/>
          </nz-input-group>
        </nz-form-control>
      </nz-form-item>
      <nz-form-item>
        <nz-form-control nzErrorTip="Please input your Password!">
          <nz-input-group nzPrefixIcon="lock">
            <input type="password" nz-input formControlName="password" placeholder="Password"/>
          </nz-input-group>
        </nz-form-control>
      </nz-form-item>

      <div class="login-form-forgot">
        <a >forgot password?</a>
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
  validateForm = this.formBuilder.group({
    username: this.formBuilder.control('', [ Validators.required ]),
    password: this.formBuilder.control('', [ Validators.required ]),
  });

  constructor(title: Title) {
    title.setTitle('Seamless Share | Login');
  }

  submitForm(): void {
    if (this.validateForm.valid) {
      console.log('submit', this.validateForm.value);
    } else {
      Object.values(this.validateForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
    }
  }
}
