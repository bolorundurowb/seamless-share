import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DatePipe } from '@angular/common';
import { AppSource, FileRes, LinkRes, TextRes } from '../types';
import { generatePreview, isLink, isText } from '../utils';

@Component({
  selector: 'ss-share-item-card',
  standalone: true,
  imports: [ CommonModule, DatePipe ],
  template: `
    <div class="card" [class.selected]="selected">
      <div class="title">
        {{ titleIcon }}
        {{ title }}
      </div>
      <div class="date">
        📅 <span>Created At:</span>
        {{ item.createdAt | date: 'medium' }}
      </div>
      <div class="date">
        ⏳ <span>Expires At:</span>
        {{ item.expiresAt | date: 'medium' }}
      </div>

      <div class="metadata">
        <span class="pill type">{{ type }}</span>
        <span class="pill status">{{ status }}</span>
        <ng-container *ngIf="item.source">
          <span class="pill" [ngClass]="sourceStyle">
            {{item.source}}
          </span>
        </ng-container>
      </div>
    </div>`,
  styles: [ `
    .card {
      border-bottom: 1px solid #e0e0e0;
      padding: 0.65rem;
      cursor: pointer;
      gap: 1rem;
      align-items: center;

      .title {
        font-weight: 500;
        color: #333;
        font-size: 1rem;
        line-height: 1.5;
        //overflow: hidden;
        //text-overflow: ellipsis;
        margin-bottom: 0.75rem;
      }

      .date {
        font-size: 0.85rem;
        color: #626B73;
        margin-top: 0.25rem;
        margin-bottom: 0.25rem;

        span {
          font-weight: 600;
        }
      }

      .metadata {
        display: flex;
        align-items: center;
        gap: 0.5rem;
        color: #666;
        font-size: 0.9rem;
        grid-column: 1 / -1;
        grid-row: 3;
        margin-top: 0.75rem;

        .pill {
          text-transform: lowercase;
          display: inline-flex;
          align-items: center;
          justify-content: center;
          padding: 0.25rem 0.75rem;
          border-radius: 9999px;
          font-size: 0.875rem;
          font-weight: 500;
          line-height: 1;
          white-space: nowrap;
          cursor: default;
          user-select: none;

          &.type {
            background-color: #f7fee7;
            color: #65a30d;
          }

          &.status {
            background-color: #e0f2fe;
            color: #0369a1;
          }

          &.unknown {
            background-color: #fff7ed;
            color: #c2410c;
          }

          &.web {
            background-color: #ccfbf1;
            color: #0d9488;
          }

          &.android {
            background-color: #ffe4e6;
            color: #9f1239;
          }

          &.ios {
            background-color: #ede9fe;
            color: #5b21b6;
          }
        }
      }

      &.selected {
        background-color: #F2ECFD;
        border-left: 0.2rem solid #9272BB;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        transform: translateY(-2px);
      }
    }
  ` ]
})
export class TitleCardComponent implements OnInit {
  @Input() item!: FileRes | TextRes | LinkRes;
  @Input() selected: boolean = false;

  title!: string;
  titleIcon!: string;
  status: string = 'Active';
  type!: string;
  sourceStyle: any = {};

  get isImage(): boolean {
    const file = this.item as FileRes;
    return file.metadata?.mimeType.startsWith('image/');
  }

  get isDocument(): boolean {
    const file = this.item as FileRes;
    return file.metadata && !file.metadata.mimeType.startsWith('image/');
  }

  get isLink(): boolean {
    return isLink(this.item);
  }

  get isText(): boolean {
    return isText(this.item);
  }

  ngOnInit() {
    this.type = this.getType();
    this.title = this.getTitle();
    this.titleIcon = this.getTitleIcon();

    if (this.item.source) {
      this.sourceStyle[this.item.source.toLowerCase()] = true;
    }
  }

  getType(): string {
    if (this.isLink) {
      return 'Link';
    }

    if (this.isImage) {
      return 'Image';
    }

    if (this.isDocument) {
      return 'Document';
    }

    if (this.isText) {
      return 'Text';
    }

    throw new Error('Unknown item type');
  }

  getTitle(): string {
    if (this.isLink) {
      const link = this.item as LinkRes;
      return link.url;
    }

    if (this.isDocument || this.isImage) {
      const file = this.item as FileRes;
      return file.metadata.name;
    }

    if (this.isText) {
      const text = this.item as TextRes;
      const preview = generatePreview(text.content);
      return preview.substring(0, 25);
      // return truncated.length < text.content.length ? `${truncated}...` : truncated;
    }

    throw new Error('Unknown item type');
  }

  getTitleIcon(): string {
    if (this.isLink) {
      return '🔗';
    }

    if (this.isImage) {
      return '🖼️';
    }

    if (this.isDocument) {
      return '📜';
    }

    if (this.isText) {
      return '🅰️';
    }

    throw new Error('Unknown item type');
  }

  protected readonly AppSource = AppSource;
}
