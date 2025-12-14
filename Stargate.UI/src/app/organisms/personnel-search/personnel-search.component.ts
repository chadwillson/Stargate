import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-personnel-search',
  standalone: false,
  templateUrl: './personnel-search.component.html',
  styleUrl: './personnel-search.component.scss'
})
export class PersonnelSearchComponent {
  @Input() searchValue = '';
  @Input() loading = false;
  @Output() searchValueChange = new EventEmitter<string>();
  @Output() search = new EventEmitter<void>();

  onSearchValueChange(value: string): void {
    this.searchValue = value;
    this.searchValueChange.emit(value);
  }

  onSearch(): void {
    this.search.emit();
  }

  onKeyEnter(): void {
    if (!this.loading && this.searchValue) {
      this.onSearch();
    }
  }
}
