import {Component, Input} from '@angular/core';

@Component({
  selector: 'ss-sections',
  standalone: true,
  template: `
  <div>Hello</div>
  `
})
export class SectionsComponent {

}

@Component({
  selector: 'ss-section',
  standalone: true,
  template: `
  <div>Hello</div>
  `
})
export class SectionComponent {
@Input() title?: string;
}
