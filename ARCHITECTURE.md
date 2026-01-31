# PCM Pro - Clean Architecture Implementation Guide

## 📋 Project Structure

```
PCM/
├── PCM.Core/                 # Domain Layer (Business Logic)
│   ├── Entities/            # Domain entities (Member, Booking, etc.)
│   ├── Enums/               # Business enums (BookingStatus, etc.)
│   └── Interfaces/          # Repository & Service contracts
│
├── PCM.Application/         # Application Layer (Use Cases)
│   ├── DTOs/                # Data Transfer Objects
│   ├── Services/            # Business logic services
│   ├── Validators/          # FluentValidation rules
│   ├── Mappings/            # AutoMapper profiles
│   └── DependencyInjection.cs
│
├── PCM.Infrastructure/      # Infrastructure Layer (External Services)
│   ├── Persistence/         # EF Core DbContext, Repositories
│   ├── Services/            # Redis, Email, SignalR, Hangfire
│   ├── Hubs/                # SignalR hubs
│   └── DependencyInjection.cs
│
└── PCM.Api/                 # Presentation Layer (API)
    ├── Controllers/         # Thin API endpoints
    ├── Program.cs           # Configuration & startup
    └── appsettings.json
```

## 🎯 Key Architecture Patterns

### 1. Clean Architecture (4 Layers)
- **Core**: No external dependencies, pure business logic
- **Application**: Use cases, DTOs, validators
- **Infrastructure**: Data access, third-party services
- **API**: HTTP endpoints, request/response handling

### 2. Repository Pattern with Unit of Work
```csharp
// Ensures transactional consistency
public class BookingService
{
    public async Task CreateBooking(CreateBookingRequest request)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // 1. Check and lock court slot (Optimistic Locking)
            var booking = new Booking { ... };
            await _unitOfWork.GetRepository<Booking>().AddAsync(booking);
            
            // 2. Deduct wallet balance in same transaction
            member.WalletBalance -= booking.TotalPrice;
            _unitOfWork.GetRepository<Member>().Update(member);
            
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
```

### 3. Optimistic Locking (Row Version)
Prevents race conditions when multiple users book the same court simultaneously:
```csharp
public class Booking
{
    // ...
    public byte[]? RowVersion { get; set; } // Timestamp token
}

// In UPDATE: Check if RowVersion matches before updating
// If doesn't match → DbUpdateConcurrencyException → Inform user
```

### 4. Dependency Injection
All services registered in DependencyInjection.cs files for easy management.

## 🔐 Authentication & Authorization

### JWT Token Flow
```
1. User Login → Generate AccessToken (15 min) + RefreshToken (7 days)
2. Frontend stores tokens, uses AccessToken for API calls
3. Token expires → Use RefreshToken to get new AccessToken
4. Logout → Revoke RefreshToken in DB
```

### Roles
- **Admin**: Full system access
- **Treasurer**: Approve deposits, manage wallet
- **Referee**: Update match results
- **Member**: Normal user

## 💾 Database Design Highlights

### Concurrency Control
- `Booking` and `Member` have `RowVersion` (timestamp) for Optimistic Locking
- Prevents double-booking when multiple users select same slot

### Transaction Safety
- Wallet balance updates use Unit of Work transactions
- Deposits, bookings, and matches update in atomic operations

### Performance
- Proper indexes on frequently queried columns
- `Member.WalletBalance` stored denormalized (not in separate table) for speed

### ELO Rating System
- `Member.RankELO` (double) stores DUPR rating
- Updated after each ranked match based on opponent rating difference
- Automatically adjusted by Match service

## 🔴 Real-time Features (SignalR)

### Live Updates
```csharp
// When booking confirmed
await _signalRService.SendToAllAsync("BookingUpdated", bookingId, "Confirmed");

// When match result posted
await _signalRService.SendToGroupAsync("tournament_1", "MatchResultUpdated", matchData);

// When wallet balance changes
await _signalRService.SendToUserAsync(memberId.ToString(), "WalletUpdated", newBalance);
```

## ⚙️ Background Jobs (Hangfire)

### Auto-Cancel Pending Bookings
```csharp
// Runs every hour
public async Task CancelExpiredPendingBookings()
{
    var expiredBookings = await _unitOfWork.GetRepository<Booking>()
        .FindAsync(b => b.Status == BookingStatus.PendingPayment 
                     && b.CreatedDate < DateTime.UtcNow.AddMinutes(-15));
    
    foreach (var booking in expiredBookings)
    {
        booking.Status = BookingStatus.Cancelled;
        // Refund wallet
        member.WalletBalance += booking.TotalPrice;
        // ...
    }
}
```

## 📦 Redis Cache Strategy

### What to Cache
1. **Courts List** (rarely changes)
   - Key: `courts:all`
   - TTL: 1 hour

2. **News (Pinned)** (frequent reads)
   - Key: `news:pinned`
   - TTL: 30 minutes

3. **Leaderboard** (sorted sets for ranking)
   - Key: `leaderboard:monthly`
   - TTL: 1 hour

### Cache Invalidation
```csharp
// After creating new booking
await _cacheService.RemoveAsync($"court:{courtId}:slots:*");
```

## 🚀 Deployment with Docker

See `docker-compose.yml`:
- **SQL Server**: Database
- **Redis**: Cache & leaderboards
- **Backend API**: ASP.NET Core application
- **Frontend**: Vue.js (Nginx)

```bash
docker-compose up -d
# API: http://localhost:5000
# Frontend: http://localhost:80
# SQL Server: localhost,1433
```

## 📝 Key Business Logic Locations

| Feature | Layer | File |
|---------|-------|------|
| Booking with concurrency | Application | `Services/BookingService.cs` |
| Wallet transactions | Application | `Services/WalletService.cs` |
| ELO rating calculation | Application | `Services/MatchService.cs` |
| Tournament bracket generation | Application | `Services/TournamentService.cs` |
| Deposit approval | Application | `Services/DepositService.cs` |
| Background jobs | Infrastructure | `Services/HangfireBackgroundJobService.cs` |
| Real-time notifications | Infrastructure | `Hubs/NotificationHub.cs` |
| Caching | Infrastructure | `Services/RedisCacheService.cs` |

## ⚠️ Important Notes

1. **Never bypass Repository**: Always use `IUnitOfWork` for data operations
2. **Always validate input**: Use FluentValidation in Application layer
3. **Use transactions**: For operations affecting multiple entities
4. **Cache keys naming**: Use convention `feature:identifier:detail`
5. **Log appropriately**: Use `ILogger<T>` for debugging
6. **Error handling**: Return meaningful error messages to frontend via DTOs

## 🔗 API Endpoints Structure

```
POST   /api/auth/login
POST   /api/auth/register
POST   /api/auth/refresh-token

GET    /api/members/me
PUT    /api/members/{id}/profile

GET    /api/courts
GET    /api/courts/{id}/slots
POST   /api/bookings
POST   /api/bookings/{id}/cancel

POST   /api/wallet/deposit
GET    /api/wallet/transactions
PUT    /api/wallet/deposits/{id}/approve

GET    /api/tournaments
POST   /api/tournaments
POST   /api/tournaments/{id}/join
GET    /api/tournaments/{id}/bracket
POST   /api/tournaments/{id}/generate-bracket

POST   /api/matches
PUT    /api/matches/{id}/result

GET    /api/news
POST   /api/news
```

---

**Author**: PCM Development Team  
**Last Updated**: 2026-01-30  
**Version**: 1.0.0
