import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthRoutingModule } from './auth-routing.module';
import { LoginPage } from './login/login.page';
import { RegisterPage } from './register/register.page';
import { AuthPage } from './auth.page';
import { NzFormControlComponent, NzFormDirective, NzFormItemComponent } from 'ng-zorro-antd/form';
import { ReactiveFormsModule } from '@angular/forms';
import { NzInputDirective, NzInputGroupComponent } from 'ng-zorro-antd/input';
import { NzColDirective, NzRowDirective } from 'ng-zorro-antd/grid';
import { NzButtonComponent } from 'ng-zorro-antd/button';

@NgModule({
  declarations: [ AuthPage, LoginPage, RegisterPage ],
  imports: [ CommonModule, AuthRoutingModule, NzFormDirective, NzFormItemComponent, ReactiveFormsModule, NzFormControlComponent, NzInputGroupComponent, NzRowDirective, NzColDirective, NzButtonComponent, NzInputDirective ],
})
export class AuthModule {
}
