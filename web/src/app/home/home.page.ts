import {Component, OnInit} from '@angular/core';
import {Title} from '@angular/platform-browser';
import {NzInputDirective, NzInputGroupComponent, NzTextareaCountComponent} from 'ng-zorro-antd/input';
import {FormsModule} from '@angular/forms';
import {NzButtonComponent} from 'ng-zorro-antd/button';
import {AuthService} from '../services/auth.service';
import {ShareService} from '../services/share.service';
import {NgIf} from '@angular/common';
import {UserRes} from '../types';
import {SectionComponent, SectionsComponent} from '../components/section.components';
import {NzIconDirective} from 'ng-zorro-antd/icon';
import {Router} from '@angular/router';

@Component({
  selector: 'ss-home',
  standalone: true,
  templateUrl: './home.page.html',
  styleUrl: './home.page.scss',
  imports: [
    FormsModule,
    NzButtonComponent,
    NzInputDirective,
    NzTextareaCountComponent,
    NgIf,
    SectionsComponent,
    SectionComponent,
    NzInputGroupComponent,
    NzIconDirective
  ]
})
export class HomePage implements OnInit {
  isAuthenticated = false;
  user: UserRes | null = null;

  shareId?: string;
  shareCode?: string;

  sharedUrl?: string;
  isSharedUrlValid = false;

  sharedText?: string;
  isSharedTextValid = false;

  sharedFile?: File;
  isSharedFileValid = false;


  constructor(title: Title, private authService: AuthService, private shareService: ShareService,
              private router: Router) {
    title.setTitle('Seamless Share | Home');
  }

  async ngOnInit() {
    this.isAuthenticated = this.authService.isAuthenticated();
    this.user = this.authService.getUser();

    if (this.isAuthenticated) {
      const ownedShare = await this.shareService.getOwnedShare();
      this.shareId = ownedShare.id;
      this.shareCode = ownedShare.code;
    }
  }

  async createLinkShare() {
    if (!this.shareCode) {
      await this.createShare();
    }

    await this.shareService.addTextToShare(this.shareId!, {content: this.sharedUrl!});
    await this.goToShare();
  }

  async createTextShare() {
    if (!this.shareCode) {
      await this.createShare();
    }

    await this.shareService.addTextToShare(this.shareId!, {content: this.sharedText!});
    await this.goToShare();
  }

  validateSharedUrl() {
    const urlPattern = /^(https?:\/\/)?([\w\d-]+)\.([a-z]{2,})([\/\w\d.-]*)*\/?$/;
    this.isSharedUrlValid = urlPattern.test(this.sharedUrl || '');
  }

  validateSharedText() {
    this.isSharedTextValid = !!this.sharedText && this.sharedText.length > 0 && this.sharedText.length <= 5000;
  }

  fileChanged(event: any) {
    this.sharedFile = event.target.files[0];
  }

  private async createShare() {
    const share = await this.shareService.createShare();
    this.shareId = share.id;
    this.shareCode = share.code;
  }

  private async goToShare() {
    await this.router.navigate(['shares', this.shareCode]);
  }
}
