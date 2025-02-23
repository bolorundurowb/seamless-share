import {Component} from '@angular/core';
import {Title} from '@angular/platform-browser';

@Component({
  selector: 'ss-home',
  standalone: true,
  templateUrl: './home.page.html',
})
export class HomePage {
  constructor(title: Title) {
    title.setTitle('Seamless Share | Home');
  }
}
