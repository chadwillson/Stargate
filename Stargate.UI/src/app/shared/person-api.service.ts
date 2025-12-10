import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PersonRequest, PersonResponse, PersonListResponse } from './models';

@Injectable({ providedIn: 'root' })
export class PersonApiService {
  private baseUrl = `${environment.apiUrl}/Person`;

  constructor(private http: HttpClient) {}

  /**
   * GET - Retrieve all people
   * GET /api/Person
   */
  getPeople(): Observable<PersonListResponse> {
    return this.http.get<PersonListResponse>(this.baseUrl);
  }

  /**
   * GET - Retrieve person by name
   * GET /api/Person/{name}
   */
  getPersonByName(name: string): Observable<PersonResponse> {
    return this.http.get<PersonResponse>(`${this.baseUrl}/${encodeURIComponent(name)}`);
  }

  /**
   * POST - Create a new person
   * POST /api/Person
   */
  createPerson(person: PersonRequest): Observable<PersonResponse> {
    return this.http.post<PersonResponse>(this.baseUrl, person);
  }

  /**
   * PUT - Update an existing person
   * PUT /api/Person/{name}
   */
  updatePerson(name: string, person: PersonRequest): Observable<PersonResponse> {
    return this.http.put<PersonResponse>(`${this.baseUrl}/${encodeURIComponent(name)}`, person);
  }
}
