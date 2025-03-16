import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthRoutingModule } from './auth-routing.module';
import { LoginPage } from './login/login.page';
import { RegisterPage } from './register/register.page';
import { AuthPage } from './auth.page';
import { NzFormModule } from 'ng-zorro-antd/form';
import { ReactiveFormsModule } from '@angular/forms';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonComponent } from 'ng-zorro-antd/button';

@NgModule({
  declarations: [ AuthPage, LoginPage, RegisterPage ],
  imports: [ CommonModule, AuthRoutingModule, NzFormModule, ReactiveFormsModule, NzInputModule, NzButtonComponent ],
})
export class AuthModule {
}
