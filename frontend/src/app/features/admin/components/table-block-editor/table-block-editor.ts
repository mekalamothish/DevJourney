import { ChangeDetectionStrategy, Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import type { TableBlock } from '../../../../core/models/blog-post';

@Component({
  selector: 'dj-table-block-editor',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './table-block-editor.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TableBlockEditor {
  @Input() block!: TableBlock;
  @Output() update = new EventEmitter<TableBlock>();

  headers: string[] = [];
  rows: string[][] = [];

  ngOnInit() {
    this.headers = [...(this.block.headers || ['Column 1', 'Column 2'])];
    this.rows = this.block.rows && this.block.rows.length > 0 
      ? this.block.rows.map(r => [...r])
      : [['', ''], ['', '']];
  }

  updateHeader(index: number, value: string) {
    this.headers[index] = value;
    this.emitUpdate();
  }

  updateCell(rowIndex: number, cellIndex: number, value: string) {
    this.rows[rowIndex][cellIndex] = value;
    this.emitUpdate();
  }

  addRow() {
    const newRow = new Array(this.headers.length).fill('');
    this.rows.push(newRow);
    this.emitUpdate();
  }

  removeRow(index: number) {
    if (this.rows.length > 1) {
      this.rows.splice(index, 1);
      this.emitUpdate();
    }
  }

  addColumn() {
    this.headers.push(`Column ${this.headers.length + 1}`);
    this.rows.forEach(row => row.push(''));
    this.emitUpdate();
  }

  removeColumn(index: number) {
    if (this.headers.length > 1) {
      this.headers.splice(index, 1);
      this.rows.forEach(row => row.splice(index, 1));
      this.emitUpdate();
    }
  }

  clearTable() {
    if (confirm('Clear all table data?')) {
      this.headers = ['Column 1', 'Column 2'];
      this.rows = [['', ''], ['', '']];
      this.emitUpdate();
    }
  }

  private emitUpdate() {
    this.update.emit({
      ...this.block,
      headers: this.headers,
      rows: this.rows
    });
  }
}
