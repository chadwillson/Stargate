import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of, delay, throwError } from 'rxjs';

export interface Item { id: number; name: string; value: string; }

@Injectable({ providedIn: 'root' })
export class ApiService {
  // Swap with a real endpoint; using mock data to keep the template runnable offline.
  private baseUrl = 'https://api.example.com/items';
  private useMockData = true; // Set to false to use real API
  
  // Mock data store
  private mockItems: Item[] = [
    { id: 1, name: 'Alpha', value: 'Ready' },
    { id: 2, name: 'Beta', value: 'In Progress' },
    { id: 3, name: 'Gamma', value: 'Done' }
  ];
  private nextId = 4;

  constructor(private http: HttpClient) {}

  /**
   * GET - Retrieve all items
   * Example: this.api.getItems().subscribe(items => console.log(items));
   */
  getItems(): Observable<Item[]> {
    if (this.useMockData) {
      return of([...this.mockItems]).pipe(delay(300)); // Simulate network delay
    }
    return this.http.get<Item[]>(this.baseUrl);
  }

  /**
   * POST - Create a new item
   * Example: this.api.createItem({ name: 'Delta', value: 'Pending' }).subscribe(newItem => console.log(newItem));
   */
  createItem(item: Partial<Item>): Observable<Item> {
    if (this.useMockData) {
      const newItem: Item = {
        id: this.nextId++,
        name: item.name || 'Untitled',
        value: item.value || 'N/A'
      };
      this.mockItems.push(newItem);
      return of(newItem).pipe(delay(300));
    }
    return this.http.post<Item>(this.baseUrl, item);
  }

  /**
   * PUT - Update an existing item
   * Example: this.api.updateItem(1, { name: 'Updated Alpha' }).subscribe(updated => console.log(updated));
   */
  updateItem(id: number, item: Partial<Item>): Observable<Item> {
    if (this.useMockData) {
      const index = this.mockItems.findIndex(i => i.id === id);
      if (index === -1) {
        return throwError(() => new Error('Item not found')).pipe(delay(300));
      }
      const updated = { ...this.mockItems[index], ...item };
      this.mockItems[index] = updated;
      return of(updated).pipe(delay(300));
    }
    return this.http.put<Item>(`${this.baseUrl}/${id}`, item);
  }

  /**
   * DELETE - Remove an item
   * Example: this.api.deleteItem(1).subscribe(() => console.log('Deleted'));
   */
  deleteItem(id: number): Observable<void> {
    if (this.useMockData) {
      const index = this.mockItems.findIndex(i => i.id === id);
      if (index === -1) {
        return throwError(() => new Error('Item not found')).pipe(delay(300));
      }
      this.mockItems.splice(index, 1);
      return of(void 0).pipe(delay(300));
    }
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  /**
   * Example of switching to real API:
   * 
   * 1. Set useMockData = false
   * 2. Update baseUrl to your actual API endpoint
   * 3. Ensure CORS is configured on your backend
   * 4. Add error handling as needed
   * 
   * Example with real API:
   * constructor(private http: HttpClient) {
   *   this.baseUrl = environment.apiUrl + '/items';
   *   this.useMockData = false;
   * }
   */
}
