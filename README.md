# Tennis Statistics Application

A comprehensive tennis statistics application featuring a Vue.js 3 frontend with Vuetify Material Design and a C# .NET 8 backend using Clean Architecture. The application tracks players, tournaments, matches, and rankings for both WTA and ATP associations.

## 🎾 Features

- **Player Management**: View player profiles, statistics, and ranking history
- **Tournament Tracking**: Browse tournaments by surface, category, and dates
- **Match Results**: Follow match scores and detailed statistics
- **Rankings**: Current WTA/ATP rankings with points and rank changes
- **Data Sync**: Automatic synchronization with balldontlie.io Tennis API

## 🛠️ Technology Stack

### Frontend
- **Vue.js 3** with Composition API and `<script setup>` syntax
- **Vuetify 3** for Material Design components
- **TypeScript** for type safety
- **Pinia** for state management
- **Vue Router** for navigation
- **Axios** for API communication
- **Vite** for fast development and building

### Backend
- **.NET 8** with ASP.NET Core Web API
- **Clean Architecture** (Domain, Application, Infrastructure, API layers)
- **Entity Framework Core** with PostgreSQL
- **MediatR** for CQRS pattern
- **AutoMapper** for object mapping
- **FluentValidation** for request validation

### Database
- **PostgreSQL** for persistent storage

## 📁 Project Structure

```
tennis-statistics/
├── docs/                           # Documentation
├── src/
│   ├── backend/
│   │   ├── TennisStats.Domain/     # Entities, Enums, Value Objects
│   │   ├── TennisStats.Application/# Use Cases, DTOs, Interfaces
│   │   ├── TennisStats.Infrastructure/ # EF Core, Repositories, External APIs
│   │   ├── TennisStats.API/        # Controllers, Middleware
│   │   └── TennisStats.sln         # Solution file
│   └── frontend/
│       └── tennis-stats-app/       # Vue.js application
└── README.md
```

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [PostgreSQL 15+](https://www.postgresql.org/download/)
- [Visual Studio Code](https://code.visualstudio.com/) (recommended)

### Database Setup

1. Install PostgreSQL and create a database:
   ```sql
   CREATE DATABASE TennisStats;
   ```

2. Update the connection string in `src/backend/TennisStats.API/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=TennisStats;Username=your_username;Password=your_password"
     }
   }
   ```

### Backend Setup

1. Navigate to the backend directory:
   ```bash
   cd src/backend
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Apply database migrations:
   ```bash
   dotnet ef database update --project TennisStats.Infrastructure --startup-project TennisStats.API
   ```

4. Run the API:
   ```bash
   cd TennisStats.API
   dotnet run
   ```

The API will be available at `https://localhost:7001` (or `http://localhost:5000`).

### Frontend Setup

1. Navigate to the frontend directory:
   ```bash
   cd src/frontend/tennis-stats-app
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm run dev
   ```

The application will be available at `http://localhost:5173`.

## 📡 API Endpoints

### Players
- `GET /api/players` - Get paginated list of players
- `GET /api/players/{id}` - Get player details
- `GET /api/players/search?q={query}` - Search players
- `GET /api/players/top/{association}/{count}` - Get top players by association

### Rankings
- `GET /api/rankings/current/{association}` - Get current rankings
- `GET /api/rankings/history/{playerId}` - Get player ranking history

### Sync
- `POST /api/sync/players/{association}` - Sync players from external API
- `POST /api/sync/rankings/{association}` - Sync rankings from external API

## 🏗️ Architecture

The backend follows Clean Architecture principles:

```
┌─────────────────────────────────────────────┐
│                   API                        │
│  (Controllers, Middleware, Configuration)    │
└─────────────────────┬───────────────────────┘
                      │
┌─────────────────────▼───────────────────────┐
│               Application                    │
│    (Use Cases, DTOs, Interfaces, CQRS)      │
└─────────────────────┬───────────────────────┘
                      │
┌─────────────────────▼───────────────────────┐
│              Infrastructure                  │
│  (EF Core, Repositories, External Services) │
└─────────────────────┬───────────────────────┘
                      │
┌─────────────────────▼───────────────────────┐
│                  Domain                      │
│      (Entities, Enums, Value Objects)       │
└─────────────────────────────────────────────┘
```

### Domain Layer
Contains business entities and logic:
- `Player`, `Tournament`, `Match`, `Set`, `Game`
- `Ranking`, `Season`, `MatchStatistics`, `PlayerStatistics`
- Enums: `Association`, `Hand`, `BackhandType`, `Surface`, `MatchStatus`, `Round`

### Application Layer
Contains business logic and use cases:
- MediatR queries and handlers (CQRS pattern)
- DTOs for data transfer
- Interface definitions for repositories and services
- AutoMapper profiles

### Infrastructure Layer
Contains external concerns:
- Entity Framework Core DbContext and configurations
- Repository implementations
- External API service (balldontlie.io)
- Unit of Work pattern

### API Layer
Contains API concerns:
- Controllers with REST endpoints
- Dependency injection configuration
- Swagger/OpenAPI documentation
- CORS configuration

## 🔧 Configuration

### API Configuration

The API is configured via `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=TennisStats;Username=postgres;Password=password"
  },
  "ExternalApi": {
    "BallDontLie": {
      "BaseUrl": "https://api.balldontlie.io/v1",
      "ApiKey": "your_api_key"
    }
  }
}
```

### Frontend Configuration

The frontend uses Vite for configuration in `vite.config.ts`:

```typescript
export default defineConfig({
  server: {
    proxy: {
      '/api': {
        target: 'https://localhost:7001',
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
```

## 🧪 Development

### Running Tests

```bash
# Backend tests
cd src/backend
dotnet test

# Frontend tests
cd src/frontend/tennis-stats-app
npm run test
```

### Building for Production

```bash
# Backend
cd src/backend
dotnet publish -c Release

# Frontend
cd src/frontend/tennis-stats-app
npm run build
```

## 📊 Data Models

### Player
- Personal info: name, country, date of birth
- Physical attributes: height, weight
- Playing style: hand preference, backhand type
- Career info: turned pro year
- Current ranking and points

### Tournament
- Name, location, category
- Surface type (Hard, Clay, Grass, Carpet)
- Prize money and points
- Start/end dates

### Match
- Players, scores, sets
- Tournament and round
- Status (Scheduled, InProgress, Completed)
- Match statistics

### Ranking
- Rank position and points
- Rank change from previous week
- Ranking date

## 🌐 External API Integration

The application integrates with [balldontlie.io Tennis API](https://www.balldontlie.io) for:
- WTA and ATP player data
- Tournament information
- Match results
- Rankings

Data is fetched from the external API and cached locally in the PostgreSQL database for better performance and offline access.

## 📝 License

This project is licensed under the MIT License.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request
