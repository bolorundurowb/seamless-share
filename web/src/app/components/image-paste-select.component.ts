import { Component, ElementRef, HostListener, ViewChild } from '@angular/core';
import { NgIf } from '@angular/common';

@Component({
  selector: 'ss-image-paste-select',
  standalone: true,
  imports: [
    NgIf
  ],
  template: `
    <div class="container">
      <!-- Display the file name if available -->
      <div *ngIf="fileName" class="file-name">
        {{ fileName }}
      </div>

      <!-- Image display area -->
      <div class="image-container" (click)="onContainerClick()">
        <img *ngIf="imageSrc" [src]="imageSrc" alt="Selected Image" class="image-preview"/>
        <div *ngIf="!imageSrc" class="placeholder">
          Click to select an image or paste one here.
        </div>
      </div>

      <!-- Hidden file input -->
      <input
        type="file"
        #fileInput
        (change)="onFileSelected($event)"
        accept="image/*"
        style="display: none;"
      />
    </div>
  `,
  styles: `
    .container {
      display: flex;
      align-items: center;
      gap: 16px;
    }

    .file-name {
      font-size: 14px;
      color: #333;
    }

    .image-container {
      width: 99.9%;
      height: 9rem;
      border: 2px dashed #ccc;
      border-radius: 0.5rem;
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      overflow: hidden;

      .image-preview {
        max-width: 100%;
        max-height: 100%;
        object-fit: contain;
      }

      .placeholder {
        color: #888;
        text-align: center;
      }
    }
  `
})
export class ImagePasteSelectComponent {
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  imageSrc: string | ArrayBuffer | null = null;
  fileName: string | null = null;

  // Handle file selection
  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      this.fileName = file.name;
      this.readFile(file);
    }
  }

  // Handle click on the container
  onContainerClick(): void {
    this.fileInput.nativeElement.click();
  }

  // Read the file and display the image
  private readFile(file: File): void {
    const reader = new FileReader();
    reader.onload = (e) => {
      this.imageSrc = e.target?.result as string;
    };
    reader.readAsDataURL(file);
  }

  // Handle paste event
  @HostListener('window:paste', [ '$event' ])
  onPaste(event: ClipboardEvent): void {
    const items = event.clipboardData?.items;
    if (items) {
      for (let i = 0; i < items.length; i++) {
        const item = items[i];
        if (item.type.indexOf('image') !== -1) {
          const blob = item.getAsFile();
          if (blob) {
            this.fileName = null; // No file name for pasted images
            this.readFile(blob);
          }
        }
      }
    }
  }
}
