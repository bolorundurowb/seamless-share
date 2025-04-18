import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DatePipe } from '@angular/common';
import { AppSource, FileRes, LinkRes, TextRes } from '../types';
import { isLink, isText } from '../utils';

@Component({
  selector: 'ss-share-item-card',
  standalone: true,
  imports: [ CommonModule, DatePipe ],
  template: `
    <div class="card" [class.selected]="selected">
      <div class="card-icon">
        <img [src]="'/images/' + type?.toLowerCase() + '.png'"/>
      </div>
      <div class="card-content">
        <div class="header">{{ title }}</div>

        <div class="date">{{ createdAt | date: 'medium' }}</div>
      </div>

      <div class="metadata">
        <span class="pill type">{{ type }}</span>
        <span class="pill status">{{ status }}</span>
        <ng-container *ngIf="source">
          <ng-container *ngIf="source === AppSource.Web">
              <span class="pill web">
                {{ source }}
              </span>
          </ng-container>
          <ng-container *ngIf="source === AppSource.Android">
              <span class="pill android">
                {{ source }}
              </span>
          </ng-container>
          <ng-container *ngIf="source === AppSource.iOS">
              <span class="pill ios">
                {{ source }}
              </span>
          </ng-container>
          <ng-container *ngIf="source === AppSource.Unknown">
              <span class="pill unknown">
                {{ source }}
              </span>
          </ng-container>
        </ng-container>
      </div>
    </div>
  `,
  styles: [ `
    .card {
      border-bottom: 1px solid #e0e0e0;
      padding: 0.65rem;
      cursor: pointer;
      display: grid;
      grid-template-columns: 4rem calc(100% - 4rem);
      grid-template-rows: 3rem 1rem 1.2rem;
      gap: 1rem;
      align-items: center;

      &.selected {
        background-color: #F2ECFD;
        border-left: 0.2rem solid #9272BB;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        transform: translateY(-2px);
      }

      &-icon {
        grid-column: 1;
        grid-row: 1 / span 2;
        display: flex;
        align-items: center;
        justify-content: center;
        height: 100%;
        box-sizing: border-box;

        img {
          width: 100%;
          height: auto;
          max-height: 100%;
          object-fit: contain;
          border-radius: 0.25rem;
        }
      }

      &-content {
        grid-column: 2;
        grid-row: 1 / span 2;
        display: flex;
        flex-direction: column;
        gap: 0.5rem;

        .header {
          font-weight: 500;
          color: #333;
          font-size: 1rem;
          line-height: 1.5;
          height: calc(2 * 1rem * 1.5);
          overflow: hidden;
        }

        .date {
          font-size: 0.85rem;
          color: #626B73;
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
    }
  ` ]
})
export class TitleCardComponent implements OnInit {
  @Input() item!: FileRes | TextRes | LinkRes;
  @Input() selected: boolean = false;

  title!: string;
  status: string = 'Active';
  type!: string;
  source?: AppSource;
  createdAt: Date = new Date();

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
    this.createdAt = new Date(this.item.createdAt);
    this.type = this.getType();
    this.source = this.item.source;
    this.title = this.getTitle();
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
      const truncated = text.content.substring(0, 50);
      return truncated.length < text.content.length ? `${truncated}...` : truncated;
    }

    throw new Error('Unknown item type');
  }

  protected readonly AppSource = AppSource;
}
