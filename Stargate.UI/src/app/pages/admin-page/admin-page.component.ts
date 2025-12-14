import { Component, OnInit } from '@angular/core';
import { UserApiService, UserResponse, UserRequest, UpdateUserRequest, RoleResponse } from '../../shared/user-api.service';

@Component({
  selector: 'app-admin-page',
  templateUrl: './admin-page.component.html',
  styleUrls: ['./admin-page.component.scss'],
  standalone: false
})
export class AdminPageComponent implements OnInit {
  users: UserResponse[] = [];
  roles: RoleResponse[] = [];
  loading = false;
  error: string | null = null;

  // Modal state
  isModalOpen = false;
  selectedUser: UserResponse | null = null;

  constructor(private userApiService: UserApiService) {}

  ngOnInit(): void {
    this.loadUsers();
    this.loadRoles();
  }

  loadUsers(): void {
    this.loading = true;
    this.error = null;

    this.userApiService.getAllUsers().subscribe({
      next: (response) => {
        if (response.success) {
          this.users = response.users;
        } else {
          this.error = response.message || 'Failed to load users';
        }
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading users:', err);
        this.error = 'Failed to load users. Please try again.';
        this.loading = false;
      }
    });
  }

  loadRoles(): void {
    this.userApiService.getAllRoles().subscribe({
      next: (response) => {
        if (response.success) {
          this.roles = response.roles;
        }
      },
      error: (err) => {
        console.error('Error loading roles:', err);
      }
    });
  }

  onCreateUser(): void {
    this.selectedUser = null;
    this.isModalOpen = true;
  }

  onEditUser(user: UserResponse): void {
    this.selectedUser = user;
    this.isModalOpen = true;
  }

  onDeleteUser(user: UserResponse): void {
    if (!confirm(`Are you sure you want to delete user "${user.username}"?`)) {
      return;
    }

    this.loading = true;
    this.userApiService.deleteUser(user.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.loadUsers(); // Reload the list
        } else {
          alert(response.message || 'Failed to delete user');
          this.loading = false;
        }
      },
      error: (err) => {
        console.error('Error deleting user:', err);
        alert('Failed to delete user. Please try again.');
        this.loading = false;
      }
    });
  }

  onRefreshUsers(): void {
    this.loadUsers();
  }

  onModalClose(): void {
    this.isModalOpen = false;
    this.selectedUser = null;
  }

  onModalSave(data: UserRequest | UpdateUserRequest): void {
    this.loading = true;

    if (this.selectedUser) {
      // Edit mode
      this.userApiService.updateUser(this.selectedUser.id, data as UpdateUserRequest).subscribe({
        next: (response) => {
          if (response.success) {
            this.isModalOpen = false;
            this.selectedUser = null;
            this.loadUsers();
          } else {
            alert(response.message || 'Failed to update user');
            this.loading = false;
          }
        },
        error: (err) => {
          console.error('Error updating user:', err);
          alert('Failed to update user. Please try again.');
          this.loading = false;
        }
      });
    } else {
      // Create mode
      this.userApiService.createUser(data as UserRequest).subscribe({
        next: (response) => {
          if (response.success) {
            this.isModalOpen = false;
            this.loadUsers();
          } else {
            alert(response.message || 'Failed to create user');
            this.loading = false;
          }
        },
        error: (err) => {
          console.error('Error creating user:', err);
          alert('Failed to create user. Please try again.');
          this.loading = false;
        }
      });
    }
  }
}
