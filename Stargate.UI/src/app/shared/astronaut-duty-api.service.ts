import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { AstronautDuty, CreateAstronautDutyRequest, BaseResponse, PersonWithDuties } from './models';

interface AstronautDutyResponse {
  id: number;
  personId: number;
  rank: string;
  dutyTitle: string;
  dutyStartDate: string;
  dutyEndDate?: string;
}

interface PersonAstronautResponse {
  personId: number;
  name: string;
  currentRank: string;
  currentDutyTitle: string;
  careerStartDate?: string;
  careerEndDate?: string;
}

interface AstronautDutiesByNameResponse {
  person?: PersonAstronautResponse;
  astronautDuties: AstronautDutyResponse[];
}

interface AstronautDutiesListResponse extends BaseResponse {
  duties: AstronautDutiesByNameResponse[];
}

@Injectable({ providedIn: 'root' })
export class AstronautDutyApiService {
  private baseUrl = `${environment.apiUrl}/AstronautDuty`;

  constructor(private http: HttpClient) {}

  /**
   * GET - Search for people by name (case-insensitive partial match) and return their duties
   * GET /api/AstronautDuty/{name}
   */
  getPeopleWithDutiesByName(name: string): Observable<PersonWithDuties[]> {
    return this.http.get<AstronautDutiesListResponse>(`${this.baseUrl}/${encodeURIComponent(name)}`).pipe(
      map(response => {
        // Return people with their duties
        return response.duties.map(duty => ({
          person: {
            id: duty.person?.personId || 0,
            name: duty.person?.name || name,
            currentRank: duty.person?.currentRank || '',
            currentDutyTitle: duty.person?.currentDutyTitle || '',
            careerStartDate: duty.person?.careerStartDate,
            careerEndDate: duty.person?.careerEndDate
          },
          astronautDuties: duty.astronautDuties.map(ad => ({
            id: ad.id,
            personId: ad.personId,
            rank: ad.rank,
            dutyTitle: ad.dutyTitle,
            dutyStartDate: ad.dutyStartDate,
            dutyEndDate: ad.dutyEndDate
          }))
        }));
      }),
      catchError(error => {
        // Handle HTTP error responses (404, 500, etc.)
        if (error.error && typeof error.error === 'object' && 'message' in error.error) {
          // The backend returned a structured error response
          throw new Error(error.error.message || 'Failed to retrieve people');
        }
        throw new Error(error.message || 'Failed to retrieve people');
      })
    );
  }

  /**
   * POST - Create a new astronaut duty
   * POST /api/AstronautDuty
   */
  createAstronautDuty(duty: CreateAstronautDutyRequest): Observable<AstronautDuty> {
    return this.http.post<AstronautDuty>(this.baseUrl, duty).pipe(
      catchError(error => {
        // Handle HTTP error responses (404, 500, etc.)
        if (error.error && typeof error.error === 'object' && 'message' in error.error) {
          // The backend returned a structured error response
          throw new Error(error.error.message || 'Failed to create astronaut duty');
        }
        throw new Error(error.message || 'Failed to create astronaut duty');
      })
    );
  }
}
