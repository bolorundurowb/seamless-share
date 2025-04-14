import { Component, inject, OnInit } from '@angular/core';
import { NavbarComponent } from '../components/navbar.component';
import { NzButtonComponent } from 'ng-zorro-antd/button';
import { NzInputDirective, NzInputGroupComponent } from 'ng-zorro-antd/input';
import { NzIconDirective } from 'ng-zorro-antd/icon';
import { Title } from '@angular/platform-browser';
import { ShareService } from '../services/share.service';
import { FileRes, LinkRes, ShareRes, TextRes } from '../types';
import { Router } from '@angular/router';
import { NgForOf } from '@angular/common';
import { TitleCardComponent } from '../components/share-item.component';

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
    TitleCardComponent
  ],
  standalone: true,
})
export class SharePage implements OnInit {
  private readonly shareService = inject(ShareService);
  private readonly router = inject(Router);

  private share!: ShareRes;
  private documents: FileRes[] = [];
  private images: FileRes[] = [];
  private texts: TextRes[] = [];
  private links: LinkRes[] = [];

  selectedItem?: LinkRes | FileRes | TextRes;


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
}
