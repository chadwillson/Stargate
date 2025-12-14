import { Component, OnInit } from '@angular/core';
import { AstronautDutyApiService } from '../../shared/astronaut-duty-api.service';
import { PersonWithDuties, CreateAstronautDutyRequest } from '../../shared/models';

@Component({
  selector: 'app-astronaut-duty-page',
  templateUrl: './astronaut-duty-page.component.html',
  styleUrls: ['./astronaut-duty-page.component.scss'],
  standalone: false
})
export class AstronautDutyPageComponent implements OnInit {
  people: PersonWithDuties[] = [];
  loading = false;
  error: string | null = null;
  searchName = '';
  editingPersonId: number | null = null;
  expandedPersonId: number | null = null;
  newDuty: Partial<CreateAstronautDutyRequest> = {};

  constructor(private dutyApi: AstronautDutyApiService) {}

  ngOnInit(): void {
    // Astronaut duties require a name to search, so we don't load on init
  }

  toggleExpand(personId: number): void {
    if (this.expandedPersonId === personId) {
      this.expandedPersonId = null;
    } else {
      this.expandedPersonId = personId;
      this.editingPersonId = null; // Close editing form if open
    }
  }

  searchPeople(): void {
    if (!this.searchName) {
      this.error = 'Please enter a name to search';
      return;
    }

    this.loading = true;
    this.error = null;
    this.editingPersonId = null;
    
    this.dutyApi.getPeopleWithDutiesByName(this.searchName).subscribe({
      next: (people) => {
        this.people = people;
        this.loading = false;
      },
      error: (err) => {
        this.error = err.message || 'Failed to load people';
        this.loading = false;
        console.error(err);
      }
    });
  }

  startEditingDuty(personId: number, personName: string): void {
    this.editingPersonId = personId;
    this.expandedPersonId = null; // Close expanded view if open
    this.newDuty = {
      name: personName,
      rank: '',
      dutyTitle: '',
      dutyStartDate: new Date().toISOString().split('T')[0]
    };
    this.error = null;
  }

  cancelEditing(): void {
    this.editingPersonId = null;
    this.newDuty = {};
    this.error = null;
  }

  onCreateDuty(personName: string, formData: Partial<CreateAstronautDutyRequest>): void {
    if (!formData.rank || !formData.dutyTitle || !formData.dutyStartDate) {
      this.error = 'Please fill in all required fields';
      return;
    }

    this.loading = true;
    const dutyRequest: CreateAstronautDutyRequest = {
      name: personName,
      rank: formData.rank,
      dutyTitle: formData.dutyTitle,
      dutyStartDate: formData.dutyStartDate
    };

    this.dutyApi.createAstronautDuty(dutyRequest).subscribe({
      next: () => {
        // Refresh the search to show updated duties
        this.searchPeople();
        this.editingPersonId = null;
        this.newDuty = {};
        this.error = null;
      },
      error: (err) => {
        this.error = err.message || 'Failed to create astronaut duty';
        this.loading = false;
        console.error(err);
      }
    });
  }
}
