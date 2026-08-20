import { ChangeDetectionStrategy, Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import type { TerminalBlock } from '../../../../core/models/blog-post';

@Component({
  selector: 'dj-terminal-block-editor',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './terminal-block-editor.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TerminalBlockEditor {
  @Input() block!: TerminalBlock;
  @Output() update = new EventEmitter<TerminalBlock>();

  lines: string[] = [];

  ngOnInit() {
    if (this.block.lines && Array.isArray(this.block.lines)) {
      this.lines = [...this.block.lines];
    } else {
      this.lines = [''];
    }
  }

  updateLine(index: number, value: string) {
    this.lines[index] = value;
    this.emitUpdate();
  }

  addLine(index?: number) {
    if (index !== undefined) {
      this.lines.splice(index + 1, 0, '');
    } else {
      this.lines.push('');
    }
    this.emitUpdate();
  }

  removeLine(index: number) {
    if (this.lines.length > 1) {
      this.lines.splice(index, 1);
      this.emitUpdate();
    }
  }

  clearLines() {
    if (confirm('Clear all terminal lines?')) {
      this.lines = [''];
      this.emitUpdate();
    }
  }

  private emitUpdate() {
    this.update.emit({
      ...this.block,
      lines: this.lines.filter(line => line !== '')
    });
  }
}
