import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PersonRequest, PersonResponse } from '../../shared/models';

@Component({
  selector: 'app-person-table',
  templateUrl: './person-table.component.html',
  styleUrls: ['./person-table.component.scss'],
  standalone: false
})
export class PersonTableComponent {
  @Input() people: PersonResponse[] = [];
  @Input() loading = false;
  @Input() editingName: string | null = null;

  @Output() updatePerson = new EventEmitter<{ name: string; updates: PersonRequest }>();
  @Output() startEdit = new EventEmitter<string>();
  @Output() cancelEdit = new EventEmitter<void>();

  editForm: Partial<PersonRequest> = {};

  onEdit(person: PersonResponse): void {
    this.editForm = { id: person.personId, name: person.name };
    this.startEdit.emit(person.name);
  }

  onSave(originalName: string): void {
    if (this.editForm.name) {
      const updates: PersonRequest = {
        id: this.editForm.id || 0,
        name: this.editForm.name
      };
      this.updatePerson.emit({ name: originalName, updates });
    }
  }

  onCancel(): void {
    this.editForm = {};
    this.cancelEdit.emit();
  }
}
