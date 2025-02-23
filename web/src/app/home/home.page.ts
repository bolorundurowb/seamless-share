import {Component} from '@angular/core';
import {Title} from '@angular/platform-browser';
import {NzTabComponent, NzTabSetComponent} from 'ng-zorro-antd/tabs';
import {NzInputDirective, NzInputGroupComponent} from 'ng-zorro-antd/input';
import {FormsModule} from '@angular/forms';
import {NzButtonComponent} from 'ng-zorro-antd/button';

@Component({
  selector: 'ss-home',
  standalone: true,
  templateUrl: './home.page.html',
  imports: [
    NzTabSetComponent,
    NzTabComponent,
    NzInputGroupComponent,
    FormsModule,
    NzButtonComponent,
    NzInputDirective
  ]
})
export class HomePage {
  sharedUrl?: string;
  isSharedUrlValid = false;

  constructor(title: Title) {
    title.setTitle('Seamless Share | Home');
  }

  validateSharedUrl() {
    const urlPattern = /^(https?:\/\/)?([\w\d-]+)\.([a-z]{2,})([\/\w\d.-]*)*\/?$/;
    this.isSharedUrlValid = urlPattern.test(this.sharedUrl || '');
  }
}
