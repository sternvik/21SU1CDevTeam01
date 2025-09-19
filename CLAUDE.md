# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is **RestoNation** - a restaurant management system built as a .NET 8 solution with a multi-layered architecture for a Swedish restaurant chain. The system manages restaurants, customers, bookings, orders, and loyalty transactions across different regions.

## Architecture

The solution follows a **4-layer architecture pattern**:

1. **EntitetsLager** - Entity/Domain layer containing POCO classes for database entities
2. **DataLager** - Data access layer with Entity Framework Core, Repository pattern, and Unit of Work pattern
3. **AffärsLager** - Business logic layer with controllers for business operations
4. **PresentationsLager** - Presentation layer (WPF application)

### Key Architectural Patterns

- **Repository Pattern**: Generic repository in `DataLager/Repository.cs` for data access operations
- **Unit of Work Pattern**: Coordinated transactions across repositories in `DataLager/UnitOfWork.cs`
- **Entity Framework Code First**: Database created/managed through EF migrations
- **No Cascade Deletes**: All foreign key relationships use `DeleteBehavior.NoAction` to prevent cascade deletion issues

## Database Configuration

The system uses **SQL Server** with connection string hardcoded in `ApplikationDbContext.cs`:
```
Data Source=sqlutb2-db.hb.se,56077;Initial Catalog=suht2501;User ID=suht2501;Password=RBE152;Encrypt=True;TrustServerCertificate=True
```

Database is automatically created with seed data when UnitOfWork is first instantiated.

## Core Entities

The system manages these key entities:
- **Region** - Restaurant regions (Norr, Öst, Väst, Syd)
- **Restaurang** - Individual restaurants (18 total across regions)
- **Kund** - Customers with loyalty program integration
- **Anvandare** - System users (Servitör, Admin, Restaurangchef, VD roles)
- **Bord** - Restaurant tables with capacity and unique codes
- **Bokning** - Table reservations
- **Bestallning** & **BestallningsRad** - Orders and order line items
- **Meny** - Menu items with categories (À la carte, Dagens lunch, Dryck)
- **Transaktion** & **LojalitetsTransaktion** - Payment and loyalty transactions
- **Systemlogg** - System activity logging

## Development Commands

### Building and Running
```bash
# Build the entire solution
dotnet build

# Build specific project
dotnet build DataLager/DataLager.csproj
dotnet build EntitetsLager/EntitetsLager.csproj
dotnet build AffärsLager/AffärsLager.csproj

# Run the WPF application
dotnet run --project PresentationsLager/PresentationsLager.csproj
```

### Entity Framework Migrations
```bash
# Add new migration (from DataLager directory)
cd DataLager
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# View migration status
dotnet ef migrations list
```

## Project Dependencies

- **.NET 8.0** - Target framework
- **Entity Framework Core 9.0.9** - ORM with SQL Server provider
- **CommunityToolkit.Mvvm 8.4.0** - MVVM framework for WPF
- **WPF** - Windows Presentation Foundation for UI

## Business Domain Notes

- **Swedish restaurant chain** with 18 locations across 4 regions
- **Loyalty program** integrated with customer transactions
- **Role-based access**: VD, Restaurangchef, Admin, Servitör
- **Table management** with coded tables (e.g., "N1001", "V2015")
- **Multi-region operations** with region-specific restaurant assignments
- **Comprehensive menu system** with pricing and categories

## WPF MVVM Architecture Guidelines

Based on analysis of the Digital-Inovation-Grupp-8-master reference project, follow this proven WPF MVVM structure:

### Folder Structure
```
PresentationsLager/
├── Views/           - XAML Windows and UserControls
├── ViewModels/      - MVVM ViewModels with CommunityToolkit.Mvvm
├── Models/          - UI-specific model classes
├── Services/        - UI services (e.g., PasswordService for secure binding)
└── Bilder/          - Image assets
```

### Key MVVM Patterns Used

**ViewModels:**
- Inherit from `ObservableObject` (CommunityToolkit.Mvvm)
- Use `[ObservableProperty]` for data binding properties
- Use `[RelayCommand]` for button/action commands
- Implement `CloseAction` pattern for window management

**Views:**
- Set DataContext in XAML: `<vm:ViewModelName/>`
- Use consistent styling with `<Window.Resources>` for button styles
- Bind to ViewModel properties: `{Binding PropertyName}`
- Bind to commands: `{Binding CommandName}`

**Services:**
- `LösenordService` shows proper PasswordBox binding pattern for secure input
- Use dependency injection pattern for business logic controllers

### RestoNation-Specific Requirements

**User Authentication & Roles:**
- Servitör/säljare (same role)
- Admin (manages menus, basic data, can delete customers)
- Restaurangchef (access to statistics, not bookings)
- VD (aggregate statistics across organization)

**Core UI Requirements from PDFs:**
- **Fast login/logout** - efficient switching between servers
- **Table booking interface** - region → restaurant → table selection
- **Order entry** - tied to tables and loyalty points
- **Statistics dashboards** - role-based views
- **Customer search** - quick lookup by phone/name
- **Loyalty program** - Bronze/Silver/Gold tiers with points display

**Design Guidelines:**
- Use RestoNation brand colors: Navy Blue (#102c48), Soft Beige (#F7F4EF), Mustard Accent (#F4B942)
- Prioritize usability over strict brand compliance
- **Critical: Must be fast and intuitive** - often used by temporary staff
- Auto-logout after inactivity period
- Default "home restaurant" per user

## Current State

Many controller classes in AffärsLager contain only TODO comments indicating incomplete business logic implementation. The data layer and entity models are fully implemented with seed data. The WPF presentation layer needs to be built following the MVVM patterns shown in the reference project.