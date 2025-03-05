import {AfterContentInit, Component, ContentChildren, Input, QueryList} from '@angular/core';
import {NgForOf, NgIf} from '@angular/common';

@Component({
  selector: 'ss-section',
  standalone: true,
  imports: [
    NgIf
  ],
  template: `
    <div *ngIf="isSelected" class="section">
      <ng-content></ng-content>
    </div>
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
    NgForOf
  ],
  template: `
    <div class="sections-container">
      <ng-content></ng-content>
    </div>
    <div class="buttons-container">
      <button *ngFor="let section of sections" (click)="selectSection(section)">
        {{ section.title }}
      </button>
    </div>
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
