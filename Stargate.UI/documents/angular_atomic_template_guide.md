# Angular Atomic Template Creation Guide

A concise playbook for spinning up an Angular application that follows Atomic Design. It mirrors the component levels outlined in this React template (atoms, molecules, organisms, templates, pages) and reuses the dark mode style guidance already documented in `Documents/dark_mode_style_guide.md`.

## 1. Prerequisites
- Node 18+ and npm installed.
- Angular CLI globally installed: `npm install -g @angular/cli`.
- TypeScript is first-class here (Angular ships with it); ensure `tsconfig.json` stays enabled and strict.
- From `E:\GenericAngularTemplate` (or your target workspace), run commands unless otherwise noted.

## 2. Scaffold the App
1) Create the TypeScript-based project without standalone API (keeps NgModules for clear grouping):  
   - Angular CLI 14+: `ng new atomic-angular --routing --style=scss --no-standalone`  
   - Angular CLI 13 (current env): `ng new atomic-angular --routing --style=scss`
2) Move into the project: `cd atomic-angular`.
3) Add Angular Material if desired for baseline accessibility: `ng add @angular/material` (optional).

## 3. Atomic Folder Structure (under `src/app`)
- `atoms/`: smallest UI pieces (Button, Label, Input, Text, Icon).
- `molecules/`: simple compositions of atoms (FormField, TagList, CardHeader).
- `organisms/`: complex sections built from molecules (AuthForm, DataTableSection, HeaderBar).
- `templates/`: page-level layouts that place organisms (DefaultTemplate, DashboardTemplate).
- `pages/`: route-bound screens using templates (LoginPage, DashboardPage).
- `shared/`: cross-cutting services, models, and theme utilities (ApiService, ThemeService, typography).

Create folders:  
`mkdir src/app/{atoms,molecules,organisms,templates,pages,shared}`

## 4. Atoms (Angular equivalents of the React examples)

### Button (`atoms/button`)
`ng g c atoms/button --export`
```typescript
// src/app/atoms/button/button.component.ts
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-button',
  templateUrl: './button.component.html',
  styleUrls: ['./button.component.scss']
})
export class ButtonComponent {
  @Input() kind: 'primary' | 'secondary' | 'ghost' = 'primary';
  @Input() type: 'button' | 'submit' = 'button';
  @Input() disabled = false;
  @Output() pressed = new EventEmitter<void>();

  onClick(): void {
    if (!this.disabled) {
      this.pressed.emit();
    }
  }
}
```
```html
<!-- src/app/atoms/button/button.component.html -->
<button
  [attr.type]="type"
  class="btn" [ngClass]="'btn-' + kind"
  [disabled]="disabled"
  (click)="onClick()">
  <ng-content></ng-content>
</button>
```

### Label and Input (`atoms/label`, `atoms/text-input`)
`ng g c atoms/label --export`  
`ng g c atoms/text-input --export`
```typescript
// src/app/atoms/text-input/text-input.component.ts
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.scss']
})
export class TextInputComponent {
  @Input() type: string = 'text';
  @Input() name = '';
  @Input() placeholder = '';
  @Input() value = '';
  @Input() disabled = false;
  @Output() valueChange = new EventEmitter<string>();

  onInput(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.valueChange.emit(target.value);
  }
}
```
```html
<!-- src/app/atoms/text-input/text-input.component.html -->
<input
  [type]="type"
  [name]="name"
  [attr.placeholder]="placeholder"
  [value]="value"
  [disabled]="disabled"
  (input)="onInput($event)"
  class="form-control" />
```

## 5. Molecules (Angular equivalent of `FormInput`)

### Form Field (`molecules/form-field`)
`ng g c molecules/form-field --export`
```typescript
// src/app/molecules/form-field/form-field.component.ts
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-form-field',
  templateUrl: './form-field.component.html',
  styleUrls: ['./form-field.component.scss']
})
export class FormFieldComponent {
  @Input() label = '';
  @Input() type: string = 'text';
  @Input() name = '';
  @Input() value = '';
  @Input() placeholder = '';
  @Input() error: string | null = null;
  @Output() valueChange = new EventEmitter<string>();
}
```
```html
<!-- src/app/molecules/form-field/form-field.component.html -->
<div class="form-group">
  <label class="form-label">{{ label }}</label>
  <app-text-input
    [type]="type"
    [name]="name"
    [value]="value"
    [placeholder]="placeholder"
    (valueChange)="valueChange.emit($event)">
  </app-text-input>
  <div *ngIf="error" class="alert alert-danger mt-2">{{ error }}</div>
</div>
```

## 6. Organisms (larger sections)

### Auth Form (`organisms/auth-form`)
`ng g c organisms/auth-form`
```typescript
// src/app/organisms/auth-form/auth-form.component.ts
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-auth-form',
  templateUrl: './auth-form.component.html'
})
export class AuthFormComponent {
  @Input() title = 'Sign in';
  @Input() error: string | null = null;
  @Output() submitAuth = new EventEmitter<{ email: string; password: string }>();

  model = { email: '', password: '' };

  onSubmit(): void {
    this.submitAuth.emit({ ...this.model });
  }
}
```
```html
<!-- src/app/organisms/auth-form/auth-form.component.html -->
<div class="card p-4 shadow-sm">
  <h2 class="h5 mb-3">{{ title }}</h2>
  <form (ngSubmit)="onSubmit()">
    <app-form-field
      label="Email"
      name="email"
      type="email"
      [(value)]="model.email">
    </app-form-field>
    <app-form-field
      label="Password"
      name="password"
      type="password"
      [(value)]="model.password">
    </app-form-field>
    <app-button type="submit" kind="primary" class="w-100 mt-2">Continue</app-button>
    <div *ngIf="error" class="alert alert-danger mt-3">{{ error }}</div>
  </form>
</div>
```

### Data Table Section (Angular equivalent of the React table example)
`ng g c organisms/data-table-section`
```typescript
// src/app/organisms/data-table-section/data-table-section.component.ts
import { Component, Input } from '@angular/core';

interface Row { id: number | string; name: string; value: string | number; }

@Component({
  selector: 'app-data-table-section',
  templateUrl: './data-table-section.component.html'
})
export class DataTableSectionComponent {
  @Input() rows: Row[] = [];
}
```
```html
<!-- src/app/organisms/data-table-section/data-table-section.component.html -->
<table class="table table-striped table-hover">
  <thead>
    <tr>
      <th>ID</th>
      <th>Name</th>
      <th>Value</th>
    </tr>
  </thead>
  <tbody>
    <tr *ngFor="let row of rows">
      <td>{{ row.id }}</td>
      <td>{{ row.name }}</td>
      <td>{{ row.value }}</td>
    </tr>
  </tbody>
</table>
```

## 7. Templates and Pages

### Default Template (`templates/default-template`)
`ng g c templates/default-template`
```typescript
// src/app/templates/default-template/default-template.component.ts
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-default-template',
  templateUrl: './default-template.component.html'
})
export class DefaultTemplateComponent {
  @Input() pageTitle = '';
}
```
```html
<!-- src/app/templates/default-template/default-template.component.html -->
<div class="page">
  <header class="page__header d-flex align-items-center justify-content-between">
    <h1 class="h4 mb-0">{{ pageTitle }}</h1>
    <ng-content select="[page-actions]"></ng-content>
  </header>
  <main class="page__body">
    <ng-content></ng-content>
  </main>
</div>
```

### Page Example (`pages/dashboard-page`)
`ng g c pages/dashboard-page`
```typescript
// src/app/pages/dashboard-page/dashboard-page.component.ts
import { Component } from '@angular/core';
import { ApiService } from '../../shared/api.service';

@Component({
  selector: 'app-dashboard-page',
  templateUrl: './dashboard-page.component.html'
})
export class DashboardPageComponent {
  rows = [];

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.api.getItems().subscribe(rows => this.rows = rows);
  }
}
```
```html
<!-- src/app/pages/dashboard-page/dashboard-page.component.html -->
<app-default-template pageTitle="Dashboard">
  <ng-container page-actions>
    <app-button kind="secondary">New Item</app-button>
  </ng-container>
  <app-data-table-section [rows]="rows"></app-data-table-section>
</app-default-template>
```

## 8. Shared Services: CRUD API (Angular `HttpClient` equivalent)
`ng g s shared/api`
```typescript
// src/app/shared/api.service.ts
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface Item { id: number; name: string; value: string; }

@Injectable({ providedIn: 'root' })
export class ApiService {
  private baseUrl = 'https://api.example.com/items';

  constructor(private http: HttpClient) {}

  getItems(): Observable<Item[]> {
    return this.http.get<Item[]>(this.baseUrl);
  }

  createItem(item: Partial<Item>): Observable<Item> {
    return this.http.post<Item>(this.baseUrl, item);
  }

  updateItem(id: number, item: Partial<Item>): Observable<Item> {
    return this.http.put<Item>(`${this.baseUrl}/${id}`, item);
  }

  deleteItem(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
```

## 9. Module Wiring
Register atomic components in feature modules for clean imports:
- Create `atoms.module.ts`, `molecules.module.ts`, `organisms.module.ts`, `templates.module.ts`, and export their declarations.
- Import these modules into `app.module.ts` and into any feature modules that need them.
- Example for atoms:
```typescript
// src/app/atoms/atoms.module.ts
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ButtonComponent } from './button/button.component';
import { LabelComponent } from './label/label.component';
import { TextInputComponent } from './text-input/text-input.component';

@NgModule({
  declarations: [ButtonComponent, LabelComponent, TextInputComponent],
  imports: [CommonModule],
  exports: [ButtonComponent, LabelComponent, TextInputComponent]
})
export class AtomsModule {}
```

## 10. Theming and Dark Mode (reuse the existing style guide)
- Create `src/styles/_theme.scss` with CSS variables aligned to `dark_mode_style_guide.md`:
```scss
:root {
  --bg-base: #121212;
  --bg-surface: #1b1b1b;
  --text-primary: #e6e6e6;
  --text-secondary: #b0b0b0;
  --accent: #4f8cff;
  --danger: #ff6b6b;
  --border: #2a2a2a;
}
body {
  background: var(--bg-base);
  color: var(--text-primary);
  font-family: "Inter", "Segoe UI", system-ui, sans-serif;
}
.btn { transition: background-color 160ms ease, color 160ms ease; }
```
- Import the theme in `src/styles.scss`: `@use 'styles/theme';`
- Respect `prefers-color-scheme` with a light token set if needed; keep contrast ratios per the guide.

## 11. Testing and Quality
- Unit test atoms and molecules with `ng test --include src/app/atoms/**/*.spec.ts`.
- Add accessibility checks (aria labels on inputs/buttons).
- Run format/lint: `ng lint` and `ng test` before packaging.

## 12. Quick Build and Serve
- Dev server: `ng serve -o`.
- Production build: `ng build --configuration production`.

## 13. Checklist (Atomic + Angular)
- [ ] Atoms exported via `AtomsModule`.
- [ ] Molecules wrap atoms and expose clean `@Input()`/`@Output()`.
- [ ] Organisms orchestrate molecules without business logic leakage.
- [ ] Templates define layout; pages supply data/state.
- [ ] Shared services handle API calls; components stay presentation-focused.
- [ ] Dark mode tokens in place; contrast verified against the style guide.
