# Hotel Booking Engine

A web-based hotel booking engine built with ASP.NET Core 10 and MySQL for backend and React.js for frontend. Guests can search rooms and make reservations. Staff manage inventory, rates, and bookings from an admin panel. No payment processing — guests pay at the property.

## Stack

- **Backend:** ASP.NET Core 10
- **Database:** MySQL + Entity Framework Core 9
- **Auth:** JWT + bcrypt *(coming soon)*

## Getting Started

### Prerequisites

- .NET 10 SDK
- MySQL 8+
- `dotnet-ef` CLI tool

```bash
dotnet tool install --global dotnet-ef
```

### Setup

1. Clone the repo:
```bash
git clone https://github.com/Tabassum-Wasila/hotel-booking-engine.git
cd hotel-booking-engine/backend
```

2. Copy the config template and fill in your database credentials:
```bash
cp appsettings.Development.example.json appsettings.Development.json
```

3. Restore packages:
```bash
dotnet restore
```

4. Apply migrations:
```bash
dotnet ef database update
```

5. Run the app:
```bash
dotnet run
```

The database will be seeded automatically on first startup in development mode.

## Seed Data

Seeded on startup in development if the database is empty:

- 1 property: Hotel Premier Cyberjaya
- 4 room types: Standard, Deluxe King, Executive Suite, Family Room
- 3 rate plans per room type: Flexible, Non-Refundable, Breakfast Included
- 90 days of inventory
- 2 staff users: `admin@hotel.com` / `agent@hotel.com` (password: `Password123!`)
- 5 sample reservations across different statuses

## Project Structure

```
backend/
  Controllers/        # API controllers (coming soon)
  Data/               # DbContext and seed data
  Models/             # Entity models
  Migrations/         # EF Core migrations
frontend/             # Frontend app (coming soon)
```

---

## Coming Soon

### Backend
- REST API endpoints (availability search, booking, auth)
- JWT authentication for staff
- Admin panel API (inventory, rate plans, reports)

### Frontend
- **Search & Availability** — date picker, guest count, room results
- **Booking Flow** — room selection → guest details → confirmation page
- **Manage Booking** — lookup by reference + email, modify or cancel
- **Admin Panel**
  - Inventory calendar
  - Rate plan management
  - Reservations list with filters
  - Occupancy and booking reports
  - Staff user management
