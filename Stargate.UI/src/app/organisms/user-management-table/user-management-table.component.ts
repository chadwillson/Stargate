import { Component, EventEmitter, Input, Output } from '@angular/core';
import { UserResponse } from '../../shared/user-api.service';

type SortColumn = 'username' | 'email' | 'firstName' | 'lastName' | 'roleName' | 'isActive' | 'lastLoginAt';

@Component({
  selector: 'app-user-management-table',
  templateUrl: './user-management-table.component.html',
  styleUrls: ['./user-management-table.component.scss'],
  standalone: false
})
export class UserManagementTableComponent {
  @Input() users: UserResponse[] = [];
  @Input() loading = false;
  @Output() editUser = new EventEmitter<UserResponse>();
  @Output() deleteUser = new EventEmitter<UserResponse>();
  @Output() refreshUsers = new EventEmitter<void>();

  sortColumn: SortColumn | null = null;
  sortAscending: boolean = true;

  onEdit(user: UserResponse): void {
    this.editUser.emit(user);
  }

  onDelete(user: UserResponse): void {
    if (confirm(`Are you sure you want to delete user "${user.username}"?`)) {
      this.deleteUser.emit(user);
    }
  }

  onRefresh(): void {
    this.refreshUsers.emit();
  }

  getStatusBadgeClass(isActive: boolean): string {
    return isActive ? 'badge bg-success' : 'badge bg-secondary';
  }

  getStatusText(isActive: boolean): string {
    return isActive ? 'Active' : 'Inactive';
  }

  formatDate(date: Date | undefined): string {
    if (!date) return 'Never';
    return new Date(date).toLocaleDateString();
  }

  sort(column: SortColumn): void {
    if (this.sortColumn === column) {
      this.sortAscending = !this.sortAscending;
    } else {
      this.sortColumn = column;
      this.sortAscending = true;
    }

    this.users.sort((a, b) => {
      let aValue: any = column === 'firstName' || column === 'lastName'
        ? (a.firstName || '') + ' ' + (a.lastName || '')
        : a[column];
      let bValue: any = column === 'firstName' || column === 'lastName'
        ? (b.firstName || '') + ' ' + (b.lastName || '')
        : b[column];

      // Handle null/undefined values
      if (aValue === null || aValue === undefined) aValue = '';
      if (bValue === null || bValue === undefined) bValue = '';

      // Handle dates
      if (column === 'lastLoginAt') {
        aValue = aValue ? new Date(aValue).getTime() : 0;
        bValue = bValue ? new Date(bValue).getTime() : 0;
      }

      // Handle booleans
      if (column === 'isActive') {
        aValue = aValue ? 1 : 0;
        bValue = bValue ? 1 : 0;
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
