import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AstronautDutyRecord, CreateAstronautDutyRequest } from '../../shared/models';

@Component({
  selector: 'app-astronaut-duty-form',
  standalone: false,
  templateUrl: './astronaut-duty-form.component.html',
  styleUrl: './astronaut-duty-form.component.scss'
})
export class AstronautDutyFormComponent {
  @Input() personName = '';
  @Input() existingDuties: AstronautDutyRecord[] = [];
  @Input() loading = false;
  @Output() save = new EventEmitter<Partial<CreateAstronautDutyRequest>>();
  @Output() cancel = new EventEmitter<void>();

  formData = {
    rank: '',
    dutyTitle: '',
    dutyStartDate: new Date().toISOString().split('T')[0]
  };

  onSave(): void {
    if (this.formData.rank && this.formData.dutyTitle && this.formData.dutyStartDate) {
      this.save.emit(this.formData);
    }
  }

  onCancel(): void {
    this.cancel.emit();
  }

  get isValid(): boolean {
    return !!(this.formData.rank && this.formData.dutyTitle && this.formData.dutyStartDate);
  }
}
