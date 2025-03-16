import { AfterContentInit, Component, ContentChildren, Input, QueryList } from '@angular/core';
import { NgForOf, NgIf } from '@angular/common';
import { NzButtonComponent } from 'ng-zorro-antd/button';

@Component({
  selector: 'ss-section',
  standalone: true,
  imports: [
    NgIf
  ],
  template: `
    <div *ngIf="isSelected" class="section">
      <div>
        <ng-content></ng-content>
      </div>
    </div>
  `,
  styles: `
    .section {
      display: flex;
      flex-direction: column;
      align-content: center;
      justify-content: center;
      width: 100%;
      height: 100%;
    }
  `
})
export class SectionComponent {
  @Input() title!: string;
  isSelected: boolean = false;
}


@Component({
  selector: 'ss-sections',
  standalone: true,
  imports: [
    NgForOf,
    NzButtonComponent
  ],
  template: `
    <div class="sections">
      <div class="section-content">
        <ng-content></ng-content>
      </div>
      <div class="section-controls">
        <ng-container *ngFor="let section of sections">
          <button nz-button
                  [nzType]="section.isSelected ? 'primary' : 'default'"
                  (click)="selectSection(section)">
            {{ section.title }}
          </button>
        </ng-container>
      </div>
    </div>
  `,
  styles: `
    .sections {
      width: 100%;
      margin-top: 1rem;
      margin-bottom: 1rem;
      border-radius: 0.5rem;
      border-style: dashed;
      border-width: 0.15rem;
      border-color: #cacaca;
      padding: 0.75rem 1rem;
      height: 14rem;
      background-color: #fafafa;
      display: flex;
      flex-direction: column;

      .section-content {
        flex: 1;
      }

      .section-controls {
        margin-top: 0.5rem;
        flex: 0 0 auto;
      }
    }
  `
})
export class SectionsComponent implements AfterContentInit {
  @ContentChildren(SectionComponent) sections!: QueryList<SectionComponent>;

  ngAfterContentInit(): void {
    if (this.sections.length > 0) {
      this.selectSection(this.sections.first);
    }
  }

  selectSection(section: SectionComponent): void {
    this.sections.forEach(s => s.isSelected = false);
    section.isSelected = true;
  }
}
