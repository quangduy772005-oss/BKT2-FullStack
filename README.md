# PCM Pro - Pickleball Club Management System (Professional Edition)

## 🎯 Project Overview

**PCM Pro** is a comprehensive fullstack application for managing a professional pickleball club with advanced features including:

- ✅ Member & E-Wallet Management with real-time balance updates
- ✅ Smart Court Booking with Optimistic Locking (concurrency control)
- ✅ Recurring Booking with conflict detection
- ✅ Tournament System with Knockout bracket generation
- ✅ ELO Rating System based on match results
- ✅ Real-time notifications via SignalR
- ✅ Background jobs for automatic booking cancellation (Hangfire)
- ✅ Redis caching for performance optimization
- ✅ JWT Authentication & Role-based Authorization
- ✅ Clean Architecture with separated layers
- ✅ Docker containerization for easy deployment

---

## 📊 Tech Stack

### Backend
- **ASP.NET Core 8.0** - Web API framework
- **Entity Framework Core 8.0** - ORM with Code First migrations
- **SQL Server 2022** - Database
- **Redis 7.0** - Caching & leaderboards
- **SignalR 1.2.9** - Real-time communication
- **Hangfire 1.8.6** - Background job processing
- **JWT Bearer** - Authentication
- **FluentValidation 11.8.0** - Input validation
- **AutoMapper 12.0.1** - DTO mapping
- **MediatR 12.2.0** - CQRS pattern support

### Frontend
- **Vue.js 3** - UI framework
- **Vite** - Build tool
- **TypeScript** - Type safety
- **Axios** - HTTP client
- **Pinia** - State management

### DevOps
- **Docker** - Containerization
- **Docker Compose** - Multi-container orchestration
- **Nginx** - Web server for frontend

---

## 🏗️ Architecture

### Clean Architecture (4 Layers)

```
┌─────────────────────────────────────────────┐
│         API Layer (Presentation)            │  Controllers, Middleware, Hubs
├─────────────────────────────────────────────┤
│    Application Layer (Use Cases)            │  Services, DTOs, Validators, Mappings
├─────────────────────────────────────────────┤
│   Infrastructure Layer (External Services)  │  EF Core, Redis, Email, Hangfire
├─────────────────────────────────────────────┤
│     Core Layer (Domain Business Logic)      │  Entities, Enums, Interfaces
└─────────────────────────────────────────────┘
```

### Key Design Patterns
- **Repository Pattern** with **Unit of Work** for data consistency
- **Dependency Injection** for loose coupling
- **CQRS** with MediatR for command/query separation
- **AutoMapper** for DTO transformation
- **FluentValidation** for input validation
- **Optimistic Locking** with `RowVersion` for concurrency control

---

## 📦 Project Structure

```
PCM/
├── PCM.Core/                    # Domain Layer
│   ├── Entities/               # Business entities
│   ├── Enums/                  # Business enumerations
│   └── Interfaces/             # Service contracts
│
├── PCM.Application/            # Application Layer
│   ├── DTOs/                   # Data Transfer Objects
│   ├── Services/               # Business logic services
│   ├── Validators/             # Validation rules
│   ├── Mappings/               # AutoMapper profiles
│   └── DependencyInjection.cs
│
├── PCM.Infrastructure/         # Infrastructure Layer
│   ├── Persistence/            # DbContext, Repositories, UnitOfWork
│   ├── Services/               # Redis, Email, Hangfire
│   ├── Hubs/                   # SignalR hubs
│   └── DependencyInjection.cs
│
├── PCM.Api/                    # API Layer
│   ├── Controllers/            # API endpoints
│   ├── Program.cs              # Startup configuration
│   └── appsettings.json        # Configuration
│
├── pcm-client/                 # Frontend (Vue.js)
│   ├── src/
│   │   ├── components/         # Vue components
│   │   ├── views/              # Page components
│   │   ├── stores/             # Pinia stores
│   │   ├── services/           # API services
│   │   └── router/             # Route definitions
│   ├── vite.config.ts
│   └── package.json
│
├── docker-compose.yml          # Multi-container setup
├── ARCHITECTURE.md             # Architecture documentation
└── README.md                   # This file
```

---

## 🚀 Getting Started

### Prerequisites
- **.NET SDK 8.0** or later
- **Node.js 18+** with npm/yarn
- **SQL Server 2022** or LocalDB
- **Redis 7.0** (optional, for caching)
- **Docker & Docker Compose** (for containerized setup)

### Local Development Setup

#### 1. Clone and Navigate
```bash
git clone <repo-url>
cd PCM
```

#### 2. Setup Backend Database

```bash
# Navigate to API project
cd PCM.Api

# Create database from migrations
dotnet ef database update -p ../PCM.Infrastructure -s PCM.Api.csproj

# Seed initial data (runs in Program.cs automatically)
```

#### 3. Configure Appsettings
Update `PCM.Api/appsettings.json` if needed:
```json
{
    "ConnectionStrings": {
        "DefaultConnection": "Server=YOUR_SERVER;Database=PCMProDb;...",
        "Redis": "localhost:6379"
    },
    "Jwt": {
        "Key": "YOUR_SECRET_KEY_MIN_32_CHARS",
        "Issuer": "PCM.Api",
        "Audience": "PCM.Client",
        "ExpireMinutes": 15
    }
}
```

#### 4. Run Backend
```bash
cd PCM.Api
dotnet run --launch-profile https
# API available at: https://localhost:5001
# Swagger UI: https://localhost:5001
# Hangfire Dashboard: https://localhost:5001/hangfire
```

#### 5. Setup Frontend
```bash
cd pcm-client
npm install
npm run dev
# Frontend available at: http://localhost:5173
```

#### 6. Test Login
- **Email**: `admin@pcm.local`
- **Password**: `Admin@123`

---

## 🐳 Docker Deployment

### Build and Run with Docker Compose

```bash
# From PCM root directory
docker-compose up -d

# Services will be available at:
# - API: http://localhost:5000
# - Frontend: http://localhost
# - SQL Server: localhost:1433 (user: sa, password: PCM@123)
# - Redis: localhost:6379
# - Hangfire: http://localhost:5000/hangfire
```

### docker-compose.yml Services

```yaml
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: PCM@123
      MSSQL_PID: Developer
    ports:
      - "1433:1433"

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"

  api:
    build:
      context: .
      dockerfile: PCM.Api/Dockerfile
    environment:
      ConnectionStrings__DefaultConnection: "Server=sqlserver;Database=PCMProDb;User Id=sa;Password=PCM@123;"
      ConnectionStrings__Redis: "redis:6379"
    ports:
      - "5000:80"
    depends_on:
      - sqlserver
      - redis

  frontend:
    build:
      context: pcm-client
      dockerfile: Dockerfile
    ports:
      - "80:80"
    depends_on:
      - api
```

---

## 📚 Core Features Documentation

### 1. Authentication & Authorization

```csharp
// Login endpoint returns JWT tokens
POST /api/auth/login
{
    "email": "user@example.com",
    "password": "Password123"
}

Response:
{
    "accessToken": "eyJhbGc...",
    "refreshToken": "refresh_token_value",
    "user": { "id": "...", "email": "..." }
}
```

**Roles**:
- `Admin`: System administration
- `Treasurer`: Manage deposits & finances
- `Referee`: Update match results
- `Member`: Regular user

### 2. E-Wallet System

**Features**:
- Create deposit requests with payment proof
- Admin approval/rejection workflow
- Automatic deduction on booking/tournament registration
- Full transaction history with audit trail
- Prevents negative balance (wallet cannot go below 0)

```csharp
// Request deposit
POST /api/wallet/deposit
{
    "amount": 1000000, // Vietnamese Dong
    "proofImageUrl": "https://..."
}

// Admin approves
PUT /api/wallet/deposits/{requestId}/approve
{
    "isApproved": true
}
```

### 3. Smart Court Booking

**Booking Flow**:
1. Check available slots (with Redis caching)
2. Attempt to reserve with Optimistic Locking
3. Deduct wallet balance
4. Broadcast real-time update via SignalR

**Recurring Booking**:
- Support weekly patterns (e.g., "Monday, Wednesday, Friday")
- Automatic conflict detection
- User can choose: skip conflicts or cancel all

```csharp
// Single booking
POST /api/bookings
{
    "courtId": 1,
    "startTime": "2026-02-15T17:00:00Z",
    "endTime": "2026-02-15T18:00:00Z"
}

// Recurring booking
POST /api/bookings/recurring
{
    "courtId": 1,
    "startTime": "17:00",
    "endTime": "18:00",
    "daysOfWeek": "1,3,5", // Mon, Wed, Fri
    "startDate": "2026-02-15",
    "endDate": "2026-03-15"
}
```

### 4. Tournament System

**Tournament Types**:
- **Duel**: 1v1 matches with prizes
- **Mini-Game**: Team battles (first to N wins)
- **Professional**: Full bracket tournament

**Features**:
- Automatic bracket generation (seeding optional)
- Round tracking: Group → QF → SF → Final
- Real-time match updates
- Prize distribution to winners

```csharp
// Create tournament
POST /api/tournaments
{
    "name": "Spring Championship 2026",
    "startDate": "2026-03-01",
    "endDate": "2026-03-31",
    "type": "Professional", // Duel, MiniGame, Professional
    "format": "Knockout", // Knockout, RoundRobin, TeamBattle
    "entryFee": 500000,
    "prizePool": 10000000
}

// Join tournament
POST /api/tournaments/{id}/join
{
    "teamName": "Team A" // optional
}

// Generate bracket
POST /api/tournaments/{id}/generate-bracket
{
    "useSeeding": true
}
```

### 5. Match & ELO Rating

**ELO Rating Calculation**:
```csharp
// After match result is recorded:
eloChange = 32 * (actualScore - expectedScore)

// Where:
// actualScore = 1 (win), 0.5 (draw), 0 (loss)
// expectedScore = 1 / (1 + 10^((opponentELO - playerELO)/400))
```

**Match Recording**:
```csharp
// Post match result
PUT /api/matches/{matchId}/result
{
    "scoreTeam1": 21,
    "scoreTeam2": 18
}

// System automatically:
// 1. Determines winner
// 2. Calculates ELO changes
// 3. Updates player rankings
// 4. Broadcasts via SignalR
```

### 6. Real-time Updates (SignalR)

**Client-side subscription** (Vue.js):
```javascript
import { useBookingStore } from '@/stores/booking'

const bookingStore = useBookingStore()

// Connect to SignalR
bookingStore.connection.on('BookingUpdated', (bookingId, status) => {
    bookingStore.updateBookingStatus(bookingId, status)
})

bookingStore.connection.on('WalletUpdated', (newBalance) => {
    bookingStore.memberBalance = newBalance
})

bookingStore.connection.on('MatchResultUpdated', (matchData) => {
    bookingStore.updateMatch(matchData)
})
```

### 7. Background Jobs (Hangfire)

**Auto-cancel pending bookings**:
- Runs every 15 minutes
- Cancels bookings pending payment for > 15 minutes
- Refunds wallet balance

**Daily reports**:
- Daily revenue summary
- Ranking updates
- Notification digest

---

## 🔐 Security Considerations

1. **JWT Expiration**: AccessToken expires in 15 minutes, RefreshToken in 7 days
2. **RowVersion**: Prevents concurrent booking updates
3. **Role-based Access**: Endpoints protected by `[Authorize(Roles = "...")]`
4. **HTTPS**: Always use HTTPS in production
5. **CORS**: Configured for trusted origins only
6. **Password Hashing**: AspNetCore Identity handles salting & hashing
7. **Transaction Safety**: Critical operations use explicit transactions

---

## 📋 API Endpoints Summary

### Authentication
```
POST   /api/auth/login
POST   /api/auth/register
POST   /api/auth/refresh-token
POST   /api/auth/logout
```

### Members
```
GET    /api/members/me
PUT    /api/members/{id}/profile
GET    /api/members/{id}
GET    /api/members/leaderboard
```

### Wallet
```
POST   /api/wallet/deposit
GET    /api/wallet/transactions
GET    /api/wallet/deposits (Admin)
PUT    /api/wallet/deposits/{id}/approve (Admin)
```

### Courts & Bookings
```
GET    /api/courts
GET    /api/courts/{id}/slots?date=2026-02-15
POST   /api/bookings
POST   /api/bookings/recurring
PUT    /api/bookings/{id}/cancel
GET    /api/bookings?memberId=1
```

### Tournaments
```
GET    /api/tournaments
POST   /api/tournaments (Admin)
POST   /api/tournaments/{id}/join
GET    /api/tournaments/{id}/bracket
POST   /api/tournaments/{id}/generate-bracket (Admin)
```

### Matches
```
POST   /api/matches (Referee)
PUT    /api/matches/{id}/result (Referee)
GET    /api/matches?tournamentId=1
```

### News
```
GET    /api/news
POST   /api/news (Admin)
PUT    /api/news/{id} (Admin)
DELETE /api/news/{id} (Admin)
```

---

## 📊 Database Schema Highlights

### Core Tables
- `AspNetUsers` - Identity users
- `Members` - Club members (extends users)
- `Courts` - Available courts
- `Bookings` - Court reservations (with RowVersion)

### Wallet System
- `WalletTransactions` - Transaction history
- `DepositRequests` - Deposit request workflow
- `TransactionCategories` - Transaction types

### Tournament System
- `Tournaments` - Tournament definitions
- `TournamentParticipants` - Registration tracking
- `TournamentMatches` - Bracket structure
- `Matches` - Match results with ELO changes

### Performance Features
- Proper indexes on frequently queried columns
- RowVersion for optimistic locking
- Denormalized `Member.WalletBalance` for quick lookups
- Redis cache for static data

---

## 🛠️ Development Workflow

### Adding a New Feature

1. **Define Entity** in `PCM.Core/Entities/`
2. **Create DTO** in `PCM.Application/DTOs/`
3. **Add Validator** in `PCM.Application/Validators/`
4. **Implement Service** in `PCM.Application/Services/`
5. **Add AutoMapper** profile in `PCM.Application/Mappings/`
6. **Create Controller** in `PCM.Api/Controllers/`
7. **Add EF Mapping** in `AppDbContext.OnModelCreating()`
8. **Create Migration**: `dotnet ef migrations add FeatureName -p PCM.Infrastructure -s PCM.Api`
9. **Test thoroughly**

### Running Tests
```bash
# (Test project not included in basic setup)
# Add xUnit or NUnit for unit tests
dotnet test
```

---

## 📞 Support & Troubleshooting

### Common Issues

**Issue**: "Cannot connect to SQL Server"
```bash
# Verify server name and credentials in appsettings.json
# Check if SQL Server is running
sqlcmd -S localhost\SQL2022 -U sa -P yourPassword
```

**Issue**: "Redis connection failed"
```bash
# Check if Redis is running
redis-cli ping
# Or disable Redis in appsettings.json (set to null)
```

**Issue**: "Migration error"
```bash
# Remove problematic migration
dotnet ef migrations remove -p PCM.Infrastructure -s PCM.Api

# Or reset database
dotnet ef database drop -p PCM.Infrastructure -s PCM.Api
dotnet ef database update -p PCM.Infrastructure -s PCM.Api
```

---

## 📝 License & Credits

**Project**: PCM Pro v1.0  
**Created**: 2026-01-30  
**Technology**: ASP.NET Core 8.0 + Vue.js 3  
**Architecture**: Clean Architecture with CQRS patterns

---

## 🤝 Contributing Guidelines

1. Follow Clean Architecture principles
2. Use Dependency Injection for all services
3. Validate input with FluentValidation
4. Use transactions for multi-entity operations
5. Add proper logging with ILogger
6. Write descriptive commit messages

---

**Happy Coding! 🚀**

For more information, see [ARCHITECTURE.md](./ARCHITECTURE.md)
