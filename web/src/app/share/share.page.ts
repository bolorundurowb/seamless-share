import { Component } from '@angular/core';
import { NavbarComponent } from '../components/navbar.component';
import { NzButtonComponent } from 'ng-zorro-antd/button';
import { NzInputDirective, NzInputGroupComponent } from 'ng-zorro-antd/input';
import { NzIconDirective, provideNzIconsPatch } from 'ng-zorro-antd/icon';

import { PlusOutline } from '@ant-design/icons-angular/icons';

@Component({
  selector: 'ss-share',
  templateUrl: 'share.page.html',
  styleUrl: 'share.page.scss',
  imports: [
    NavbarComponent,
    NzButtonComponent,
    NzInputDirective,
    NzInputGroupComponent,
    NzIconDirective
  ],
  standalone: true,
  providers: [
    provideNzIconsPatch([PlusOutline])
  ]
})
export class SharePage {
}
