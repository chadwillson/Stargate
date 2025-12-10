import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Item } from '../../shared/api.service';

@Component({
  selector: 'app-data-table-section',
  templateUrl: './data-table-section.component.html',
  styleUrls: ['./data-table-section.component.scss'],
  standalone: false
})
export class DataTableSectionComponent {
  @Input() rows: Item[] = [];
  @Input() loading = false;
  @Input() editingId: number | null = null;
  @Output() deleteItem = new EventEmitter<number>();
  @Output() updateItem = new EventEmitter<{ id: number; updates: Partial<Item> }>();
  @Output() startEdit = new EventEmitter<number>();
  @Output() cancelEdit = new EventEmitter<void>();

  editForm: { [key: number]: { name: string; value: string } } = {};

  onStartEdit(row: Item): void {
    this.editForm[row.id] = {
      name: row.name,
      value: row.value
    };
    this.startEdit.emit(row.id);
  }

  onSaveEdit(id: number): void {
    const updates = this.editForm[id];
    if (updates) {
      this.updateItem.emit({ id, updates });
      delete this.editForm[id];
    }
  }

  onCancelEdit(id: number): void {
    delete this.editForm[id];
    this.cancelEdit.emit();
  }

  onDelete(id: number): void {
    this.deleteItem.emit(id);
  }
}
