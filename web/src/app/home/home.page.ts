import {Component, OnInit} from '@angular/core';
import {Title} from '@angular/platform-browser';
import {NzTabComponent, NzTabSetComponent} from 'ng-zorro-antd/tabs';
import {NzInputDirective, NzInputGroupComponent, NzTextareaCountComponent} from 'ng-zorro-antd/input';
import {FormsModule} from '@angular/forms';
import {NzButtonComponent} from 'ng-zorro-antd/button';
import {AuthService} from '../services/auth.service';
import {ShareService} from '../services/share.service';
import {NgIf} from '@angular/common';
import {UserRes} from '../types';
import {SectionComponent, SectionsComponent} from '../components/section.component';

@Component({
  selector: 'ss-home',
  standalone: true,
  templateUrl: './home.page.html',
  styleUrl: './home.page.scss',
  imports: [
    NzInputGroupComponent,
    FormsModule,
    NzButtonComponent,
    NzInputDirective,
    NzTextareaCountComponent,
    NgIf,
    SectionsComponent,
    SectionComponent
  ]
})
export class HomePage implements OnInit {
  isAuthenticated = false;
  user: UserRes | null = null;
  shareCode?: string;

  sharedUrl?: string;
  isSharedUrlValid = false;

  sharedText?: string;


  constructor(title: Title, private authService: AuthService, private shareService: ShareService) {
    title.setTitle('Seamless Share | Home');
  }

  async ngOnInit() {
    this.isAuthenticated = this.authService.isAuthenticated();
    this.user = this.authService.getUser();

    if (this.isAuthenticated) {
      const ownedShare = await this.shareService.getOwnedShare();
      this.shareCode = ownedShare.code;
    }
  }

  validateSharedUrl() {
    const urlPattern = /^(https?:\/\/)?([\w\d-]+)\.([a-z]{2,})([\/\w\d.-]*)*\/?$/;
    this.isSharedUrlValid = urlPattern.test(this.sharedUrl || '');
  }
}
