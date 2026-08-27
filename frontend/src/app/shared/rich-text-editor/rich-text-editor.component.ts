// Usage example:
// <app-rich-text-editor [(ngModel)]="model.content[0].text"></app-rich-text-editor>
// or in template-driven forms: <app-rich-text-editor name="body" [(ngModel)]="body"></app-rich-text-editor>

import { Component, Input, forwardRef, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { EditorModule, TINYMCE_SCRIPT_SRC } from '@tinymce/tinymce-angular';

@Component({
  selector: 'app-rich-text-editor',
  standalone: true,
  imports: [CommonModule, FormsModule, EditorModule],
  template: `
    <editor
      [init]="mergedInit"
      [(ngModel)]="valueModel"
      [disabled]="disabled"
    ></editor>
  `,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => RichTextEditorComponent),
      multi: true,
    },
    { provide: TINYMCE_SCRIPT_SRC, useValue: '/assets/tinymce/tinymce.min.js' },
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RichTextEditorComponent implements ControlValueAccessor {
  // Allow passing a tinymce init config to override defaults
  @Input() config: any;

  private _value: string | undefined = '';
  disabled = false;

  private onChange: any = () => {};
  private onTouched: any = () => {};

  // Use the local tinymce script by providing TINYMCE_SCRIPT_SRC in providers (below)

  defaultInit = {
    base_url: '/assets/tinymce', // self-hosted assets (see postinstall copy)
    suffix: '.min',
    apiKey: '', // ensure no cloud requests
    menubar: false,
    plugins: [
      'lists',
      'link',
      'image',
      'table',
      'code',
      'wordcount',
      'searchreplace',
      'formatpainter',
    ],
    toolbar:
      'undo redo | bold italic | bullist numlist | alignleft aligncenter alignright | link image table | code | wordcount',
    // Explicitly point skins/content_css to local copies to avoid network requests
    skin: 'oxide',
    skin_url: '/assets/tinymce/skins/ui/oxide',
    content_css: '/assets/tinymce/skins/content/default/content.min.css',
  } as any;

  get mergedInit() {
    return { ...this.defaultInit, ...(this.config || {}) };
  }

  // ControlValueAccessor interface
  writeValue(obj: any): void {
    this._value = obj ?? '';
    this.onChange(this._value);
  }
  registerOnChange(fn: any): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }
  setDisabledState?(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  // Bridge for [(ngModel)] used by <editor>
  get valueModel() {
    return this._value;
  }
  set valueModel(v: any) {
    this._value = v;
    this.onChange(v);
  }
}
