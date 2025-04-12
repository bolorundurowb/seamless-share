import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DatePipe } from '@angular/common';
import { FileRes, LinkRes, TextRes } from '../types';

@Component({
  selector: 'ss-share-item-card',
  standalone: true,
  imports: [ CommonModule, DatePipe ],
  template: `
    <div class="card">
      <div class="card-content">
        <div class="header">{{ title }}</div>

        <div class="metadata">
          <span class="pill type">{{ type }}</span>
          <span class="pill status">{{ status }}</span>
          <span class="pill source">{{ source }}</span>
        </div>

        <div class="date">{{ createdAt | date: 'medium' }}</div>
      </div>
    </div>
  `,
  styles: [ `
    .card {
      border-bottom: 1px solid #e0e0e0;
      margin: 1rem;
      background-color: white;

      &-content {
        display: flex;
        flex-direction: column;
        gap: 8px;

        .header {
          margin: 0;
          font-size: 24px;
          font-weight: 500;
          color: #333;
        }

        .metadata {
          display: flex;
          align-items: center;
          gap: 16px;
          color: #666;
          font-size: 14px;

          .pill {
            font-weight: 500;
            color: #333;

            &.type {

            }

            &.status {

            }

            &.source {

            }
          }
        }

        .date {
          position: relative;
          padding-left: 16px;

          &::before {
            content: "•";
            position: absolute;
            left: 0;
            color: #e0e0e0;
          }
        }
      }
    }
  ` ]
})
export class TitleCardComponent implements OnInit {
  @Input() item!: FileRes | TextRes | LinkRes;

  title: string = 'Title';
  status: string = 'Draft';
  type: string = 'Web';
  source?: string;
  createdAt: Date = new Date();

  ngOnInit() {
    this.createdAt = new Date(this.item.createdAt);
    this.type = this.getType();
    this.source = this.source?.toString() ?? 'Unknown';
  }

  getType(): string {
    if ('content' in this.item && this.item.content) {
      return 'Text';
    }

    if ('url' in this.item && this.item.url) {
      return 'Link';
    }

    return 'Document';
  }
}
