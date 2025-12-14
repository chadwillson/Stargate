import { Component, Input } from '@angular/core';
import { AstronautDutyRecord } from '../../shared/models';

type SortColumn = 'id' | 'rank' | 'dutyTitle' | 'dutyStartDate' | 'dutyEndDate';

@Component({
  selector: 'app-astronaut-duty-table',
  templateUrl: './astronaut-duty-table.component.html',
  styleUrls: ['./astronaut-duty-table.component.scss'],
  standalone: false
})
export class AstronautDutyTableComponent {
  @Input() duties: AstronautDutyRecord[] = [];
  @Input() loading = false;

  sortColumn: SortColumn | null = null;
  sortAscending: boolean = true;

  sort(column: SortColumn): void {
    if (this.sortColumn === column) {
      this.sortAscending = !this.sortAscending;
    } else {
      this.sortColumn = column;
      this.sortAscending = true;
    }

    this.duties.sort((a, b) => {
      let aValue: any = a[column];
      let bValue: any = b[column];

      // Handle null/undefined values
      if (aValue === null || aValue === undefined) aValue = '';
      if (bValue === null || bValue === undefined) bValue = '';

      // Handle dates
      if (column === 'dutyStartDate' || column === 'dutyEndDate') {
        aValue = aValue ? new Date(aValue).getTime() : 0;
        bValue = bValue ? new Date(bValue).getTime() : 0;
      }

      // Handle numbers
      if (column === 'id') {
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
