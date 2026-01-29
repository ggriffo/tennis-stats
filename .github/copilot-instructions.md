# Tennis Statistics Application - Copilot Instructions

## Project Overview
A tennis statistics application with Vue.js 3 frontend (Vuetify Material Design) and C# .NET 8 backend using Clean Architecture.

## Technology Stack

### Frontend
- Vue.js 3 with Composition API
- Vuetify 3 (Material Design)
- TypeScript
- Pinia for state management
- Vue Router
- Axios for API calls

### Backend
- .NET 8
- Clean Architecture (Domain, Application, Infrastructure, API)
- Entity Framework Core with PostgreSQL
- MediatR for CQRS
- AutoMapper
- FluentValidation

### Database
- PostgreSQL

## Project Structure

```
tennis-statistics/
├── src/
│   ├── backend/
│   │   ├── TennisStats.Domain/           # Entities, Enums, Value Objects
│   │   ├── TennisStats.Application/      # Use Cases, DTOs, Interfaces
│   │   ├── TennisStats.Infrastructure/   # EF Core, External APIs, Repositories
│   │   └── TennisStats.API/              # Controllers, Middleware
│   └── frontend/
│       └── tennis-stats-app/             # Vue.js application
└── docs/
```

## Coding Guidelines

### C# Backend
- Use async/await for all database and API operations
- Follow CQRS pattern with MediatR
- Use repository pattern for data access
- All entities should have audit fields (CreatedAt, UpdatedAt)
- Use FluentValidation for request validation
- External API data should always be cached in local database

### Vue.js Frontend
- Use Composition API with `<script setup>` syntax
- Use TypeScript for type safety
- Components should be in PascalCase
- Use Pinia stores for shared state
- Follow Vuetify design patterns

## Data Models
- Players (supports WTA and ATP)
- Tournaments
- Matches
- Sets
- Statistics
- Rankings
- Seasons

## External API Integration
- Source: balldontlie.io Tennis API
- Strategy: Read from API, store locally, serve from local database
