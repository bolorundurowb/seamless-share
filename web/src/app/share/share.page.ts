import { Component, ElementRef, inject, OnInit, ViewChild } from '@angular/core';
import { NavbarComponent } from '../components/navbar.component';
import { NzButtonComponent } from 'ng-zorro-antd/button';
import { NzInputDirective, NzInputGroupComponent, NzTextareaCountComponent } from 'ng-zorro-antd/input';
import { NzIconDirective } from 'ng-zorro-antd/icon';
import { Title } from '@angular/platform-browser';
import { ShareService } from '../services/share.service';
import { FileRes, LinkRes, ShareRes, TextRes } from '../types';
import { Router } from '@angular/router';
import { DatePipe, NgForOf, NgIf } from '@angular/common';
import { TitleCardComponent } from '../components/share-item.component';
import { extractErrorMessaging, isFile, isLink, isText, isUrlValid } from '../utils';
import { NzMessageService } from 'ng-zorro-antd/message';
import { FilesizePipe } from '../components/file-size.pipe';
import { NzPopconfirmDirective } from 'ng-zorro-antd/popconfirm';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { FormsModule } from '@angular/forms';
import { ImagePasteSelectComponent } from '../components/image-paste-select.component';
import { SectionComponent, SectionsComponent } from '../components/section.components';
import {TextShareComponent} from '../components/text-share.component';

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
    FilesizePipe,
    NzPopconfirmDirective,
    NzModalModule,
    FormsModule,
    ImagePasteSelectComponent,
    SectionComponent,
    SectionsComponent,
    TextShareComponent
  ],
  standalone: true,
})
export class SharePage implements OnInit {
  private readonly shareService = inject(ShareService);
  private readonly router = inject(Router);
  private readonly messageService = inject(NzMessageService);

  @ViewChild('sharedImage') sharedImageRef!: ElementRef<HTMLImageElement>;

  private share!: ShareRes;
  private documents: FileRes[] = [];
  private images: FileRes[] = [];
  private texts: TextRes[] = [];
  private links: LinkRes[] = [];

  // selectedItem?: LinkRes | FileRes | TextRes;
  selectedItem?: any;
  shareLink!: string;
  isAdderModalVisible = false;
  isMobileContentModalVisible = false;

  sharedUrl?: string;
  isSharedUrlValid = false;
  sharedText?: string;
  isSharedTextValid = false;
  sharedDocument?: File;
  isSharedDocumentValid = false;
  sharedImage?: File;
  isSharedImageValid = false;


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

    try {
      this.share = await this.shareService.getShareByCode(shareCode);
      this.documents = await this.shareService.getDocumentShares(this.share.id);
      this.texts = await this.shareService.getTextShares(this.share.id);
      this.links = await this.shareService.getLinkShares(this.share.id);
      this.images = await this.shareService.getImageShares(this.share.id);
    } catch (err: any) {
      const statusCode = err.status;
      if (statusCode === 404) {
        this.messageService.warning('You do not have access to this share');
        await this.router.navigate([ 'auth', 'login' ]);
        return;
      }

      this.messageService.error(extractErrorMessaging(err) ?? 'Failed to load share');
      throw err;
    }

    // create the share link
    this.shareLink = `${location.origin}/shares/${this.share.code}`;
  }

  select(item: FileRes | TextRes | LinkRes) {
    this.selectedItem = item;

    // Show mobile modal on tablet/mobile devices
    if (window.innerWidth <= 768) {
      this.isMobileContentModalVisible = true;
    }
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

  async showAdderModal() {
    this.isAdderModalVisible = true;
  }

  async dismissAdderModal() {
    this.isAdderModalVisible = false;
    this.resetState();
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
        this.messageService.error(extractErrorMessaging(err) ?? 'Failed to download file');
      } finally {
        this.messageService.remove(messageId);
      }
    } else {
      this.messageService.error('Invalid operation');
    }
  }

  async copyShareLink() {
    const success = await this.copyTextToClipboard(this.shareLink);

    if (success) {
      this.messageService.success('Share link copied to clipboard');
    } else {
      this.messageService.error('Failed to copy share link to clipboard');
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
    } else if (this.isImage()) {
      const success = await this.copyRenderedImageToClipboard(this.sharedImageRef.nativeElement);

      if (success) {
        this.messageService.success('Image copied to clipboard');
      } else {
        this.messageService.error('Failed to copy image to clipboard');
      }
    } else {
      this.messageService.error('Invalid operation');
    }
  }

  async deleteItem() {
    const messageId = this.messageService.loading('Deleting item...', { nzDuration: 0 }).messageId;
    const shareId = this.share.id;
    const shareItemId = this.selectedItem.id;

    try {
      if (this.isLink()) {
        await this.shareService.deleteLinkFromShare(shareId, shareItemId);
        this.links = this.links.filter((link) => link.id !== shareItemId);
      } else if (this.isDocument()) {
        await this.shareService.deleteDocumentShare(shareId, shareItemId);
        this.documents = this.documents.filter((doc) => doc.id !== shareItemId);
      } else if (this.isImage()) {
        await this.shareService.deleteImageShare(shareId, shareItemId);
        this.images = this.images.filter((img) => img.id !== shareItemId);
      } else if (this.isText()) {
        await this.shareService.deleteTextFromShare(shareId, shareItemId);
        this.texts = this.texts.filter((text) => text.id !== shareItemId);
      } else {
        throw new Error('Unknown item type');
      }

      this.selectedItem = undefined;
      this.messageService.success('Item deleted successfully');

      // Close mobile modal if open
      if (window.innerWidth <= 768) {
        this.isMobileContentModalVisible = false;
      }
    } catch (err) {
      console.error(err);
      this.messageService.error(extractErrorMessaging(err) ?? 'Failed to delete item');
    } finally {
      this.messageService.remove(messageId);
    }
  }

  async copyRenderedImageToClipboard(imgElement: HTMLImageElement): Promise<boolean> {
    const canvas = document.createElement('canvas');
    canvas.width = imgElement.naturalWidth;
    canvas.height = imgElement.naturalHeight;

    const ctx = canvas.getContext('2d');
    if (!ctx) return false;

    ctx.drawImage(imgElement, 0, 0);

    return new Promise<boolean>((resolve) => {
      canvas.toBlob(async (blob) => {
        if (!blob) return resolve(false);

        try {
          const item = new ClipboardItem({ [blob.type]: blob });
          await navigator.clipboard.write([ item ]);
          resolve(true);
        } catch {
          resolve(false);
        }
      }, 'image/png');
    });
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

  async createLinkShare() {
    const link = await this.shareService.addTextToShare(this.share.id, { content: this.sharedUrl! }) as LinkRes;
    this.links = [ link, ...this.links ];

    this.messageService.success('Link added to share');
    this.resetState();
  }

  async createTextShare() {
    const text = await this.shareService.addTextToShare(this.share.id, { content: this.sharedText! }) as TextRes;
    this.texts = [ text, ...this.texts ];

    this.messageService.success('Text added to share');
    this.resetState();
  }

  async createImageShare() {
    const messageId = this.messageService.loading('Uploading image...', { nzDuration: 0 }).messageId;

    try {
      const image = await this.shareService.createImageShare(this.share.id, this.sharedImage!) as FileRes;
      this.images = [ image, ...this.images ];

      this.messageService.success('Image added to share');
      this.resetState();
    } catch (err) {
      console.error(err);
      this.messageService.error(extractErrorMessaging(err) ?? 'Failed to upload image');
    } finally {
      this.messageService.remove(messageId);
    }
  }

  async createDocumentShare() {
    const messageId = this.messageService.loading('Uploading document...', { nzDuration: 0 }).messageId;

    try {
      const document = await this.shareService.createDocumentShare(this.share.id, this.sharedDocument!) as FileRes;
      this.documents = [ document, ...this.documents ];

      this.messageService.success('Document added to share');
      this.resetState();
    } catch (err) {
      console.error(err);
      this.messageService.error(extractErrorMessaging(err) ?? 'Failed to upload document');
    } finally {
      this.messageService.remove(messageId);
    }
  }

  validateSharedUrl() {
    this.isSharedUrlValid = isUrlValid(this.sharedUrl);
  }

  validateSharedText(text: string) {
    this.sharedText = text;
    this.isSharedTextValid = !!this.sharedText && this.sharedText.length > 0 && this.sharedText.length <= 5000;
  }

  validateSharedImage() {
    this.isSharedImageValid = !!this.sharedImage && this.sharedImage.size > 0 && this.sharedImage.size <= 10_000_000;
  }

  imageSelected(event: File) {
    this.sharedImage = event;
    this.validateSharedImage();
  }

  validateSharedDocument() {
    this.isSharedDocumentValid = !!this.sharedDocument && this.sharedDocument.size > 0 && this.sharedDocument.size <= 10_000_000;
  }

  fileChanged(event: any) {
    this.sharedDocument = event.target.files[0];
    this.validateSharedDocument();
  }

  resetState() {
    this.sharedUrl = undefined;
    this.sharedText = undefined;
    this.sharedDocument = undefined;
    this.sharedImage = undefined;
    this.isSharedUrlValid = false;
    this.isSharedTextValid = false;
    this.isSharedDocumentValid = false;
    this.isSharedImageValid = false;
  }

  closeMobileModal() {
    this.isMobileContentModalVisible = false;
  }
}
