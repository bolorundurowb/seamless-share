import {Component, ContentChild, ContentChildren, Input, QueryList, TemplateRef} from '@angular/core';
import {NgForOf, NgIf, NgTemplateOutlet} from '@angular/common';

@Component({
  selector: 'ss-section',
  standalone: true,
  template: `
    <ng-template>
      <ng-content></ng-content>
    </ng-template>
  `
})
export class SectionComponent {
  @Input() title!: string;
  @ContentChild(TemplateRef) template!: TemplateRef<any>;
}


@Component({
  selector: 'ss-sections',
  standalone: true,
  imports: [
    NgTemplateOutlet,
    NgForOf,
    NgIf
  ],
  template: `
    <div class="sections-container">
      <ng-container *ngFor="let section of sections; let i = index">
        <div *ngIf="selectedSectionIndex === i" class="section-content">
          <ng-container *ngTemplateOutlet="section.template"></ng-container>
        </div>
      </ng-container>
      <div class="buttons">
        <button *ngFor="let section of sections; let i = index"
                (click)="selectSection(i)"
                [class.active]="selectedSectionIndex === i">{{ section.title }}
        </button>
      </div>
    </div>
  `
})
export class SectionsComponent {
  @ContentChildren(SectionComponent) sections!: QueryList<SectionComponent>;
  selectedSectionIndex = 0;

  selectSection(index: number) {
    this.selectedSectionIndex = index;
  }
}
