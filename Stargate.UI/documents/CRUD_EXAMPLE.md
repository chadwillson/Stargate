# CRUD API Example - Angular

This application demonstrates a complete CRUD (Create, Read, Update, Delete) implementation using Angular with a clean atomic design pattern.

## Features

### ✅ Full CRUD Operations
- **Create**: Add new items with name and value
- **Read**: Display all items in a data table
- **Update**: Edit existing items inline
- **Delete**: Remove items with confirmation

### 🎨 UI Features
- Loading states during API calls
- Error handling and display
- Inline editing in the data table
- Confirmation dialogs for destructive actions
- Responsive form inputs

## File Structure

```
src/app/
├── shared/
│   └── api.service.ts              # API service with CRUD methods
├── pages/
│   └── dashboard-page/             # Main page with CRUD UI
│       ├── dashboard-page.component.ts
│       ├── dashboard-page.component.html
│       └── dashboard-page.component.scss
└── organisms/
    └── data-table-section/         # Reusable table component
        ├── data-table-section.component.ts
        ├── data-table-section.component.html
        └── data-table-section.component.scss
```

## API Service Usage

### GET - Retrieve Items
```typescript
this.api.getItems().subscribe({
  next: (items) => console.log('Items:', items),
  error: (err) => console.error('Error:', err)
});
```

### POST - Create Item
```typescript
const newItem = { name: 'New Item', value: 'Pending' };
this.api.createItem(newItem).subscribe({
  next: (created) => console.log('Created:', created),
  error: (err) => console.error('Error:', err)
});
```

### PUT - Update Item
```typescript
const updates = { name: 'Updated Name', value: 'Complete' };
this.api.updateItem(1, updates).subscribe({
  next: (updated) => console.log('Updated:', updated),
  error: (err) => console.error('Error:', err)
});
```

### DELETE - Remove Item
```typescript
this.api.deleteItem(1).subscribe({
  next: () => console.log('Deleted successfully'),
  error: (err) => console.error('Error:', err)
});
```

## Mock Data vs Real API

The API service is currently configured to use **mock data** for offline development:

```typescript
private useMockData = true; // In api.service.ts
```

### Switching to a Real API

1. **Update the API service** (`src/app/shared/api.service.ts`):
```typescript
private baseUrl = 'https://your-api.com/api/items';
private useMockData = false;
```

2. **Configure environment variables** (`src/environments/environment.ts`):
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://your-api.com/api'
};
```

3. **Update the service constructor**:
```typescript
constructor(private http: HttpClient) {
  this.baseUrl = environment.apiUrl + '/items';
  this.useMockData = false;
}
```

4. **Ensure CORS is configured** on your backend to allow requests from `http://localhost:4200`

## Component Features

### Dashboard Page Component
- Manages CRUD state (loading, error, editing)
- Handles form submission for creating items
- Coordinates update and delete operations
- Provides user feedback

### Data Table Section Component
- Displays items in a table format
- Supports inline editing with save/cancel
- Emits events for parent component handling
- Shows loading and empty states

## Error Handling

The implementation includes comprehensive error handling:

```typescript
this.api.deleteItem(id).subscribe({
  next: () => {
    this.rows = this.rows.filter(r => r.id !== id);
    this.loading = false;
  },
  error: (err) => {
    this.error = 'Failed to delete item';
    this.loading = false;
    console.error(err);
  }
});
```

## Best Practices Demonstrated

1. **RxJS Observables**: Proper subscription with error handling
2. **TypeScript Types**: Strong typing with `Item` interface
3. **Component Communication**: Using `@Input` and `@Output` decorators
4. **State Management**: Managing loading, error, and editing states
5. **User Experience**: Loading indicators, confirmations, and error messages
6. **Mock Data**: Simulated network delays for realistic testing

## Testing the CRUD Operations

1. Start the dev server: `npm start`
2. Navigate to `http://localhost:4200`
3. Try the following:
   - Click "New Item" to create an item
   - Click "Edit" on any row to modify it
   - Click "Delete" to remove an item
   - Observe loading states during operations

## Next Steps

To extend this example:

1. **Add pagination** for large datasets
2. **Implement search/filter** functionality
3. **Add sorting** by column
4. **Include validation** for form inputs
5. **Add toast notifications** for success/error messages
6. **Implement optimistic updates** for better UX
7. **Add unit tests** for components and service

## Resources

- [Angular HttpClient Documentation](https://angular.io/guide/http)
- [RxJS Observable Guide](https://rxjs.dev/guide/observable)
- [Angular Forms Guide](https://angular.io/guide/forms-overview)
