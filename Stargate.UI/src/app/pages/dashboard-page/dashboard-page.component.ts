import { Component, OnInit } from '@angular/core';
import { PersonApiService } from '../../shared/person-api.service';
import { PersonRequest, PersonResponse } from '../../shared/models';

@Component({
  selector: 'app-dashboard-page',
  templateUrl: './dashboard-page.component.html',
  styleUrls: ['./dashboard-page.component.scss'],
  standalone: false
})
export class DashboardPageComponent implements OnInit {
  rows: PersonResponse[] = [];
  loading = false;
  error: string | null = null;
  isCreating = false;
  newPerson: Partial<PersonRequest> = { name: '', id: 0 };
  editingName: string | null = null;

  constructor(private personApi: PersonApiService) {}

  ngOnInit(): void {
    this.loadPeople();
  }

  loadPeople(): void {
    this.loading = true;
    this.error = null;
    this.personApi.getPeople().subscribe({
      next: (response) => {
        this.rows = response.people || [];
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load people';
        this.loading = false;
        console.error(err);
      }
    });
  }

  onCreatePerson(): void {
    if (!this.newPerson.name) {
      return;
    }

    this.loading = true;
    const personRequest: PersonRequest = {
      id: this.newPerson.id || 0,
      name: this.newPerson.name
    };

    this.personApi.createPerson(personRequest).subscribe({
      next: (person) => {
        this.rows = [...this.rows, person];
        this.newPerson = { name: '', id: 0 };
        this.isCreating = false;
        this.loading = false;
        this.error = null;
      },
      error: (err) => {
        this.error = 'Failed to create person';
        this.loading = false;
        console.error(err);
      }
    });
  }

  onUpdatePerson(name: string, updates: PersonRequest): void {
    this.loading = true;
    this.personApi.updatePerson(name, updates).subscribe({
      next: (updated) => {
        this.rows = this.rows.map(r => r.name === name ? updated : r);
        this.editingName = null;
        this.loading = false;
        this.error = null;
      },
      error: (err) => {
        this.error = 'Failed to update person';
        this.loading = false;
        console.error(err);
      }
    });
  }

  toggleCreating(): void {
    this.isCreating = !this.isCreating;
    if (!this.isCreating) {
      this.newPerson = { name: '', id: 0 };
    }
  }

  startEditing(name: string): void {
    this.editingName = name;
  }

  cancelEdit(): void {
    this.editingName = null;
  }
}
