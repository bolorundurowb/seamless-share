import { Component, inject, OnInit } from '@angular/core';
import { NavbarComponent } from '../components/navbar.component';
import { NzButtonComponent } from 'ng-zorro-antd/button';
import { NzInputDirective, NzInputGroupComponent } from 'ng-zorro-antd/input';
import { NzIconDirective } from 'ng-zorro-antd/icon';
import { Title } from '@angular/platform-browser';
import { ShareService } from '../services/share.service';
import { FileRes, LinkRes, ShareRes, TextRes } from '../types';
import { Router } from '@angular/router';
import { DatePipe, NgForOf, NgIf } from '@angular/common';
import { TitleCardComponent } from '../components/share-item.component';
import { isFile, isLink, isText } from '../utils';
import { NzMessageService } from 'ng-zorro-antd/message';
import { FilesizePipe } from '../components/file-size.pipe';

@Component({
  selector: 'ss-share',
  templateUrl: 'share.page.html',
  styleUrl: 'share.page.scss',
  imports: [
    NavbarComponent,
    NzButtonComponent,
    NzInputDirective,
    NzInputGroupComponent,
    NzIconDirective,
    NgForOf,
    TitleCardComponent,
    NgIf,
    DatePipe,
    FilesizePipe
  ],
  standalone: true,
})
export class SharePage implements OnInit {
  private readonly shareService = inject(ShareService);
  private readonly router = inject(Router);
  private readonly messageService = inject(NzMessageService);

  private share!: ShareRes;
  private documents: FileRes[] = [];
  private images: FileRes[] = [];
  private texts: TextRes[] = [];
  private links: LinkRes[] = [];

  // selectedItem?: LinkRes | FileRes | TextRes;
  selectedItem?: any;
  shareLink!: string;


  constructor(title: Title) {
    title.setTitle('Seamless Share | Your Share');
  }

  get items(): (FileRes | TextRes | LinkRes)[] {
    return [ ...this.documents, ...this.texts, ...this.links, ...this.images ].sort((a, b) => {
      return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
    });
  }

  async ngOnInit() {
    const routeSnapshot = this.router.routerState.snapshot;
    const shareCode = routeSnapshot.root.firstChild?.params['shareCode'];

    if (!shareCode) {
      await this.router.navigate([ '/' ]);
    }

    this.share = await this.shareService.getShareByCode(shareCode);
    this.documents = await this.shareService.getDocumentShares(this.share.id);
    this.texts = await this.shareService.getTextShares(this.share.id);
    this.links = await this.shareService.getLinkShares(this.share.id);
    this.images = await this.shareService.getImageShares(this.share.id);
  }

  select(item: FileRes | TextRes | LinkRes) {
    this.selectedItem = item;
  }

  isSelectedItemAFile(): boolean {
    return isFile(this.selectedItem);
  }

  isLink(): boolean {
    return isLink(this.selectedItem);
  }

  isText(): boolean {
    return isText(this.selectedItem);
  }

  isDocument(): boolean {
    return isFile(this.selectedItem) && !this.selectedItem.metadata.mimeType.startsWith('image/');
  }

  isImage(): boolean {
    return isFile(this.selectedItem) && this.selectedItem.metadata.mimeType.startsWith('image/');
  }

  async downloadFIle() {
    if (this.isSelectedItemAFile()) {
      const messageId = this.messageService.loading('Downloading file...', { nzDuration: 0 }).messageId;

      try {
        const url = this.selectedItem.url;
        const fileName = this.selectedItem.metadata.name;

        const response = await fetch(url);
        const blob = await response.blob();

        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = fileName;
        link.click();
        URL.revokeObjectURL(link.href);

        this.messageService.success('File downloaded successfully');
      } catch (err) {
        console.error(err);
        this.messageService.error('Failed to download file');
      } finally {
        this.messageService.remove(messageId);
      }
    } else {
      this.messageService.error('Invalid operation');
    }
  }

  async copyToClipboard() {
    if (this.isLink() || this.isText()) {
      const text = this.selectedItem.url || this.selectedItem.content;
      const success = await this.copyTextToClipboard(text);

      if (success) {
        this.messageService.success('Copied to clipboard');
      } else {
        this.messageService.error('Failed to copy to clipboard');
      }
    } else {
      this.messageService.error('Invalid operation');
    }
  }

  async copyTextToClipboard(text: string): Promise<boolean> {
    if (navigator.clipboard) {
      try {
        await navigator.clipboard.writeText(text);
        return true;
      } catch (err) {
        console.error(err);
        return false;
      }
    }

    return this.copyTextToClipboardLegacy(text);
  }

  copyTextToClipboardLegacy(text: string): boolean {
    const textarea = document.createElement('textarea');
    textarea.value = text;
    textarea.style.position = 'fixed';
    document.body.appendChild(textarea);
    textarea.select();

    try {
      const successful = document.execCommand('copy');
      document.body.removeChild(textarea);
      return successful;
    } catch (err) {
      console.error('Failed to copy text: ', err);
      document.body.removeChild(textarea);
      return false;
    }
  }
}
