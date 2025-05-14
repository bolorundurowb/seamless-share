import {Component, EventEmitter, OnDestroy, OnInit, Output} from '@angular/core';
import {Editor, NgxEditorComponent, NgxEditorMenuComponent, Toolbar} from 'ngx-editor';
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'ss-text-share',
  template: `
    <div class="NgxEditor__Wrapper">
      <ngx-editor-menu
        [editor]="editor"
        [toolbar]="toolbar"
      ></ngx-editor-menu>
      <ngx-editor
        [editor]="editor"
        [(ngModel)]="html"
        [disabled]="false"
        [maxlength]="7500"
        [placeholder]="'Type here...'"
        (ngModelChange)="contentChanged()"
      ></ngx-editor>
    </div>
  `,
  styles: `
    $default-height: 10rem;
    $default-max-height: 12.5rem;

    .NgxEditor__Wrapper {
      text-align: start;
      height: $default-height;
      max-height: $default-max-height;
    }

    ::ng-deep .NgxEditor {
      height: calc($default-height - 39px);
      max-height: calc($default-max-height - 39px);
      overflow-y: auto;
    }
  `,
  imports: [
    NgxEditorMenuComponent,
    NgxEditorComponent,
    FormsModule
  ],
  standalone: true
})
export class TextShareComponent implements OnInit, OnDestroy {
  editor!: Editor;
  html = '';
  @Output() htmlChange = new EventEmitter<string>();

  toolbar: Toolbar = [
    ['bold', 'italic'],
    ['underline', 'strike'],
    ['code', 'blockquote'],
    ['ordered_list', 'bullet_list'],
    [{ heading: ['h1', 'h2', 'h3', 'h4', 'h5', 'h6'] }],
    ['link', 'image'],
    ['text_color', 'background_color'],
    ['align_left', 'align_center', 'align_right', 'align_justify'],
  ];



  ngOnInit(): void {
    this.editor = new Editor();
  }

  ngOnDestroy(): void {
    this.editor.destroy();
  }

  contentChanged() {
    this.htmlChange.emit(this.html);
  }
}
