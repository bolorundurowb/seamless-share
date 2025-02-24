import { Component } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { NzTabComponent, NzTabSetComponent } from 'ng-zorro-antd/tabs';
import { NzInputDirective, NzInputGroupComponent, NzTextareaCountComponent } from 'ng-zorro-antd/input';
import { FormsModule } from '@angular/forms';
import { NzButtonComponent } from 'ng-zorro-antd/button';
import { AuthService } from '../services/auth.service';
import { ShareService } from '../services/share.service';

@Component({
  selector: 'ss-home',
  standalone: true,
  templateUrl: './home.page.html',
  styleUrl: './home.page.scss',
  imports: [
    NzTabSetComponent,
    NzTabComponent,
    NzInputGroupComponent,
    FormsModule,
    NzButtonComponent,
    NzInputDirective,
    NzTextareaCountComponent
  ]
})
export class HomePage {
  sharedUrl?: string;
  isSharedUrlValid = false;

  sharedText?: string;

  constructor(title: Title, private authService: AuthService, private shareService: ShareService) {
    title.setTitle('Seamless Share | Home');
  }

  validateSharedUrl() {
    const urlPattern = /^(https?:\/\/)?([\w\d-]+)\.([a-z]{2,})([\/\w\d.-]*)*\/?$/;
    this.isSharedUrlValid = urlPattern.test(this.sharedUrl || '');
  }
}
