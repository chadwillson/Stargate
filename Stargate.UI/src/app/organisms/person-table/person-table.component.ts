import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PersonRequest, PersonResponse } from '../../shared/models';

type SortColumn = 'personId' | 'name' | 'currentRank' | 'currentDutyTitle' | 'careerStartDate' | 'careerEndDate';

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
  sortColumn: SortColumn | null = null;
  sortAscending: boolean = true;

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

  hasAstronautRecords(person: PersonResponse): boolean {
    // A person has astronaut records if they have any of these fields set
    return !!(person.currentRank || person.currentDutyTitle || person.careerStartDate);
  }

  sort(column: SortColumn): void {
    if (this.sortColumn === column) {
      this.sortAscending = !this.sortAscending;
    } else {
      this.sortColumn = column;
      this.sortAscending = true;
    }

    this.people.sort((a, b) => {
      let aValue: any = a[column];
      let bValue: any = b[column];

      // Handle null/undefined values
      if (aValue === null || aValue === undefined) aValue = '';
      if (bValue === null || bValue === undefined) bValue = '';

      // Handle dates
      if (column === 'careerStartDate' || column === 'careerEndDate') {
        aValue = aValue ? new Date(aValue).getTime() : 0;
        bValue = bValue ? new Date(bValue).getTime() : 0;
      }

      // Handle numbers
      if (column === 'personId') {
        aValue = Number(aValue);
        bValue = Number(bValue);
      }

      // Compare
      let comparison = 0;
      if (aValue > bValue) {
        comparison = 1;
      } else if (aValue < bValue) {
        comparison = -1;
      }

      return this.sortAscending ? comparison : -comparison;
    });
  }

  isSorted(column: SortColumn): boolean {
    return this.sortColumn === column;
  }
}
