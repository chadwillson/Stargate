import { Component, EventEmitter, Input, OnChanges, SimpleChanges, Output } from '@angular/core';
import { UserResponse, RoleResponse, UserRequest, UpdateUserRequest } from '../../shared/user-api.service';
import { SelectOption } from '../../atoms/select/select.component';

@Component({
  selector: 'app-user-form-modal',
  standalone: false,
  templateUrl: './user-form-modal.component.html',
  styleUrl: './user-form-modal.component.scss'
})
export class UserFormModalComponent implements OnChanges {
  @Input() isOpen = false;
  @Input() user: UserResponse | null = null; // If null, create mode; if set, edit mode
  @Input() roles: RoleResponse[] = [];
  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<UserRequest | UpdateUserRequest>();

  formData: any = {
    username: '',
    email: '',
    password: '',
    firstName: '',
    lastName: '',
    roleId: 1,
    isActive: true
  };

  isEditMode = false;

  get roleOptions(): SelectOption[] {
    return this.roles.map(role => ({
      value: role.id,
      label: role.name
    }));
  }

  ngOnChanges(changes: SimpleChanges): void {
    // Re-initialize form when isOpen changes to true or when user changes
    if ((changes['isOpen'] && this.isOpen) || changes['user'] || changes['roles']) {
      this.initializeForm();
    }
  }

  private initializeForm(): void {
    this.isEditMode = !!this.user;

    if (this.user) {
      // Edit mode - populate form with user data
      this.formData = {
        email: this.user.email,
        firstName: this.user.firstName || '',
        lastName: this.user.lastName || '',
        roleId: this.user.roleId,
        isActive: this.user.isActive,
        newPassword: ''
      };
    } else {
      // Create mode - reset form
      this.formData = {
        username: '',
        email: '',
        password: '',
        firstName: '',
        lastName: '',
        roleId: this.roles.length > 0 ? this.roles[0].id : 1,
        isActive: true
      };
    }
  }

  onClose(): void {
    this.close.emit();
  }

  onSave(): void {
    if (this.isEditMode) {
      // Edit mode - send UpdateUserRequest
      const updateData: UpdateUserRequest = {
        email: this.formData.email,
        firstName: this.formData.firstName,
        lastName: this.formData.lastName,
        roleId: this.formData.roleId,
        isActive: this.formData.isActive
      };

      // Only include password if it was changed
      if (this.formData.newPassword) {
        updateData.newPassword = this.formData.newPassword;
      }

      this.save.emit(updateData);
    } else {
      // Create mode - send UserRequest
      const createData: UserRequest = {
        username: this.formData.username,
        email: this.formData.email,
        password: this.formData.password,
        firstName: this.formData.firstName,
        lastName: this.formData.lastName,
        roleId: this.formData.roleId
      };

      this.save.emit(createData);
    }
  }
}
