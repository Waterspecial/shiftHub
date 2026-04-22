# ShiftHub — CLAUDE.md

## What This Project Is
ShiftHub is a multi-tenant SaaS platform that replaces WhatsApp-based shift management for cleaning, housekeeping, security, and staffing agencies. Each agency gets an isolated workspace. Workers can belong to multiple agencies (like Slack workspaces).

## Technology Stack
| Layer | Technology |
|---|---|
| Backend | .NET 10 / ASP.NET Core Web API |
| ORM | Entity Framework Core with PostgreSQL (Npgsql) |
| Database | PostgreSQL 16 |
| Cache | Redis |
| Real-time | SignalR |
| Background jobs | Hangfire (Phase 2+) |
| Auth | ASP.NET Identity + JWT. Workers: phone + OTP. Managers: email + password |
| Push notifications | Firebase Cloud Messaging (FCM) |
| SMS | Twilio |
| File storage | Azure Blob Storage |
| Tax | HMRC API (PAYE + NI) |
| Mobile | React Native or Flutter |
| Web dashboard | React / Next.js |

## Solution Structure
```
src/
  ShiftHub.Domain/         # Entities, enums, value objects. Zero dependencies.
  ShiftHub.Application/    # CQRS commands/queries, business logic, interfaces
  ShiftHub.Infrastructure/ # EF Core, repositories, external services
  ShiftHub.API/            # ASP.NET Core Web API — controllers, middleware, JWT
  ShiftHub.Worker/         # Hangfire background jobs (Phase 2+, not active yet)
apps/
  mobile/                  # React Native / Flutter worker app
  web/                     # Next.js manager dashboard
```

## Project Reference Rules (Clean Architecture)
- Domain → nothing
- Application → Domain
- Infrastructure → Application + Domain
- API → Application + Infrastructure
- Worker → Application + Infrastructure

## Multi-Tenancy
- Shared database with EF Core global query filters scoped to `OrgId`
- JWT token contains `UserId` + active `OrgId`
- `TenantResolutionMiddleware` validates membership before every request
- Workers are linked to agencies via `OrgMembership` bridge table (not a direct foreign key)

## Key Design Decisions
- No GPS verification for clock-in/out — just timestamps
- No cross-agency overlap detection — agencies are fully isolated
- Personal calendar is client-side only — the backend never merges cross-workspace shifts
- Releasing a shift reopens the slot and notifies other workers — no penalty applied

## Build Phases
- **Phase 1 (Weeks 1–10):** Operations MVP — shifts, clock-in, workspace switcher ← current
- **Phase 2 (Weeks 11–15):** Payroll — timesheets, PAYE/NI, payslips
- **Phase 3 (Weeks 16–20):** CRM core — contracts, complaints, invoices
- **Phase 4 (Weeks 21–26):** CRM advanced — leads pipeline, analytics, reporting

## Progress Tracker

### Done
- [x] Git repo + `.gitignore`
- [x] Solution + 5 projects scaffolded (`Domain`, `Application`, `Infrastructure`, `API`, `Worker`)
- [x] Project references wired (clean architecture)
- [x] `docker-compose.yml` — PostgreSQL + Redis
- [x] `.env.example` — all environment variable keys
- [x] NuGet packages installed (MediatR, FluentValidation, EF Core/Npgsql, Identity, Redis, JwtBearer, Swagger)
- [x] Domain enums: `PayFrequency`, `UserRole`, `MembershipStatus`, `ShiftStatus`, `AssignmentStatus`
- [x] Domain entities: `Organisation`, `User`, `OrgMembership`, `Client`, `Site`, `PayRate`, `Shift`, `ShiftAssignment`, `Timesheet`

### Up Next
- [ ] `ShiftHubDbContext` — EF Core DbContext with multi-tenant query filters
- [ ] Repository interfaces in Application
- [ ] Repository implementations in Infrastructure
- [ ] JWT authentication setup
- [ ] `TenantResolutionMiddleware`
- [ ] API controllers (Phase 1 endpoints)

## Common Commands
```bash
# Run the API
dotnet run --project src/ShiftHub.API

# Run background worker (Phase 2+)
dotnet run --project src/ShiftHub.Worker

# Add a new EF Core migration
dotnet ef migrations add <MigrationName> --project src/ShiftHub.Infrastructure --startup-project src/ShiftHub.API

# Apply migrations to database
dotnet ef database update --project src/ShiftHub.Infrastructure --startup-project src/ShiftHub.API

# Start local Postgres + Redis
docker-compose up -d

# Build entire solution
dotnet build
```

## Environment Variables (see .env.example)
- `ConnectionStrings__DefaultConnection` — PostgreSQL connection string
- `Redis__ConnectionString` — Redis connection string
- `Jwt__Secret` — JWT signing key
- `Jwt__Issuer` — JWT issuer
- `Jwt__Audience` — JWT audience
- `Twilio__AccountSid`, `Twilio__AuthToken`, `Twilio__FromNumber` — SMS
- `Firebase__ServerKey` — Push notifications
- `Azure__BlobStorageConnectionString` — File storage
