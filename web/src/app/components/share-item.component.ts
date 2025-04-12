import { Component, Input } from '@angular/core';
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
        <h1 class="title">{{ title }}</h1>
        <h2 *ngIf="subtitle" class="subtitle">{{ subtitle }}</h2>

        <div class="metadata">
          <span class="status">{{ status }}</span>
          <span class="category">{{ category }}</span>
          <span class="date">{{ date | date }}</span>
        </div>
      </div>
    </div>
  `,
  styles: [ `
    .card {
      border: 1px solid #e0e0e0;
      border-radius: 4px;
      padding: 24px;
      margin: 16px;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
      background-color: white;

      &-content {
        display: flex;
        flex-direction: column;
        gap: 8px;

        .title {
          margin: 0;
          font-size: 24px;
          font-weight: 500;
          color: #333;
        }

        .subtitle {
          margin: 0;
          font-size: 16px;
          font-weight: 400;
          color: #666;
        }

        .metadata {
          display: flex;
          align-items: center;
          gap: 16px;
          color: #666;
          font-size: 14px;

          .status {
            font-weight: 500;
            color: #333;
          }

          .category,
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
    }
  ` ]
})
export class TitleCardComponent {
  @Input() item!: FileRes | TextRes | LinkRes;

  title: string = 'Title';
  subtitle?: string;
  status: string = 'Draft';
  category: string = 'Web';
  date: Date = new Date();
}
