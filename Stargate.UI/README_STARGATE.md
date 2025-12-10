# Stargate UI

Angular web application for managing the Stargate system, including Person and Astronaut Duty management.

## Features

### Person Management
- View all people in the system
- Create new person records
- Update existing person information
- View person details including rank, duty title, and career dates

### Astronaut Duty Management
- Search astronaut duties by name
- View duty assignments with rank, title, and dates
- Create new astronaut duty assignments

## Project Structure

Built with Angular atomic design pattern:

```
src/app/
├── atoms/           # Basic UI elements (buttons, inputs)
├── molecules/       # Composite components
├── organisms/       # Complex components (tables, forms)
│   ├── person-table/
│   └── astronaut-duty-table/
├── pages/          # Page components
│   ├── dashboard-page/          # Person management
│   └── astronaut-duty-page/     # Astronaut duty management
├── templates/      # Page layouts
└── shared/         # Services and models
    ├── models/
    ├── person-api.service.ts
    └── astronaut-duty-api.service.ts
```

## Getting Started

### Prerequisites
- Node.js (v18+)
- npm (v9+)
- Angular CLI (v19.0.0)

### Installation

1. Install dependencies:
```bash
npm install
```

2. Configure API endpoint:
   - Development: `src/environments/environment.ts`
   - Production: `src/environments/environment.prod.ts`

   Default development API: `http://localhost:5173/api`

### Running the Application

Development server:
```bash
npm start
```

Navigate to `http://localhost:4200`

### Building for Production

```bash
npm run build
```

Build artifacts will be in the `dist/stargate-ui` directory.

## API Integration

The application connects to the Stargate API with the following endpoints:

### Person API (`/api/Person`)
- `GET /api/Person` - Get all people
- `GET /api/Person/{name}` - Get person by name
- `POST /api/Person` - Create new person
- `PUT /api/Person/{name}` - Update person

### Astronaut Duty API (`/api/AstronautDuty`)
- `GET /api/AstronautDuty/{name}` - Get astronaut duties by name
- `POST /api/AstronautDuty` - Create new astronaut duty

## Development

### Running Tests

Unit tests:
```bash
npm test
```

### Code Style

This project follows Angular style guide and uses:
- TypeScript strict mode
- SCSS for styling
- Standalone: false (module-based components)

## Visual Studio Integration

This project includes a Visual Studio JavaScript project file (`Stargate.UI.esproj`) and is integrated into the main Stargate solution.

## Contributing

When adding new features:
1. Create models in `shared/models/`
2. Create services in `shared/`
3. Build components following atomic design pattern
4. Update routing in `app-routing.module.ts`
5. Add navigation links in `app.component.html`

## Technology Stack

- Angular 19.0.0
- RxJS 7.8.0
- TypeScript 5.6.0
- SCSS for styling
