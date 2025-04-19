import { Component, inject, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { NzInputDirective, NzInputGroupComponent, NzTextareaCountComponent } from 'ng-zorro-antd/input';
import { FormsModule } from '@angular/forms';
import { NzButtonComponent } from 'ng-zorro-antd/button';
import { AuthService } from '../services/auth.service';
import { ShareService } from '../services/share.service';
import { FileRes, ShareRes } from '../types';
import { SectionComponent, SectionsComponent } from '../components/section.components';
import { NzIconDirective } from 'ng-zorro-antd/icon';
import { Router } from '@angular/router';
import { ImagePasteSelectComponent } from '../components/image-paste-select.component';
import { NavbarComponent } from '../components/navbar.component';
import { isUrlValid } from '../utils';
import { NzMessageService } from 'ng-zorro-antd/message';

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
    SectionsComponent,
    SectionComponent,
    NzInputGroupComponent,
    NzIconDirective,
    ImagePasteSelectComponent,
    NavbarComponent
  ]
})
export class HomePage implements OnInit {
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);
  private readonly shareService = inject(ShareService);
  private readonly messageService = inject(NzMessageService);

  sharedUrl?: string;
  isSharedUrlValid = false;

  sharedText?: string;
  isSharedTextValid = false;

  sharedFile?: File;
  isSharedFileValid = false;


  constructor(title: Title) {
    title.setTitle('Seamless Share | Home');
  }

  async ngOnInit() {
    const isAuthenticated = this.authService.isAuthenticated();

    if (isAuthenticated) {
      const ownedShare = await this.shareService.getOwnedShare();
      await this.goToShare(ownedShare.code);
    }
  }

  async createLinkShare() {
    const share = await this.createShare();
    await this.shareService.addTextToShare(share.id, { content: this.sharedUrl! });
    await this.goToShare(share.code);
  }

  async createTextShare() {
    const share = await this.createShare();
    await this.shareService.addTextToShare(share.id, { content: this.sharedText! });
    await this.goToShare(share.code);
  }

  async createDocumentShare() {
    const messageId = this.messageService.loading('Uploading document...', { nzDuration: 0 }).messageId;

    try {
      const share = await this.createShare();
     await this.shareService.createDocumentShare(share.id, this.sharedFile!);
      await this.goToShare(share.code);
    } catch (err) {
      console.error(err);
      this.messageService.error('Failed to upload document');
    } finally {
      this.messageService.remove(messageId);
    }
  }

  validateSharedUrl() {
    this.isSharedUrlValid = isUrlValid(this.sharedUrl);
  }

  validateSharedText() {
    this.isSharedTextValid = !!this.sharedText && this.sharedText.length > 0 && this.sharedText.length <= 5000;
  }

  validateSharedFile() {
    this.isSharedFileValid = !!this.sharedFile && this.sharedFile.size > 0 && this.sharedFile.size <= 10_000_000;
  }

  fileChanged(event: any) {
    this.sharedFile = event.target.files[0];
  }

  private async createShare(): Promise<ShareRes> {
    return await this.shareService.createShare();
    this.validateSharedFile();
  }

  private async goToShare(shareCode: string) {
    await this.router.navigate([ 'shares', shareCode ]);
  }
}
