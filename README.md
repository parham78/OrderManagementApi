# Order Management API

A backend Order Management and Ecommerce API built with C#, ASP.NET Core, Entity Framework Core, SQL Server, ASP.NET Core Identity, and JWT authentication.

The system provides product, customer, inventory, and order-management capabilities with secure authentication, role-based authorization, customer ownership rules, concurrency handling, and structured API responses.

## Technologies

* C#
* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* ASP.NET Core Identity
* JWT Bearer Authentication
* REST APIs
* LINQ
* Swagger / OpenAPI
* Git

## Features

### Authentication & Authorization

* User registration with ASP.NET Core Identity
* Secure password hashing through Identity
* JWT-based authentication
* Customer and Admin roles
* Role-based authorization
* Admin account bootstrapping
* Customer profiles linked to Identity users
* Current authenticated user resolution
* Customer resource ownership enforcement
* Active/inactive customer validation
* Admin-only management endpoints
* Customer-specific `/api/me/...` endpoints

### Order Management

* Multi-item orders
* Customer-owned order retrieval
* Customer-owned order creation
* Server-derived CustomerId for authenticated customers
* Historical product price snapshots
* Order total calculation
* Pagination
* Ownership-scoped pagination
* Inventory validation
* Stock deduction
* Optimistic concurrency protection

### Product Management

* Product creation
* Product lookup and filtering
* Stock management
* Product price management
* Public product read endpoints
* Admin-only product write operations

### Customer Management

* Customer profiles
* Unique customer email index
* Identity user linkage
* Active/inactive customer status
* Admin-only customer management endpoints

### Data & Persistence

* Entity Framework Core
* SQL Server
* Code-first migrations
* Relational database modeling
* Foreign keys
* One-to-one and one-to-many relationships
* DTO-based API requests and responses
* Asynchronous database operations
* LINQ queries and projections
* `AsNoTracking` read queries

### Reliability & Error Handling

* Global exception handling
* HTTP `400 Bad Request`
* HTTP `401 Unauthorized`
* HTTP `403 Forbidden`
* HTTP `404 Not Found`
* HTTP `409 Conflict`
* Optimistic concurrency using SQL Server `rowversion`
* Concurrent stock-update protection

## Database Relationships

```text
ApplicationUser 1 ──── 0..1 Customer

Customer 1 ──── * Order

Order 1 ──── * OrderItem

Product 1 ──── * OrderItem
```

## Authorization Model

```text
Anonymous User
    └── Public product reads

Customer
    └── /api/me/orders
        ├── View own orders
        ├── View own order by ID
        └── Create orders for the authenticated customer

Admin
    ├── Order management
    ├── Customer management
    └── Product write operations
```

Customer-facing operations do not trust a client-supplied `CustomerId`.

Instead, ownership is resolved from the authenticated identity:

```text
JWT
↓
ApplicationUser ID
↓
Customer profile
↓
Customer ID
↓
Owned resources
```

This prevents customers from creating or accessing orders on behalf of another customer.

## Project Structure

```text
Controllers/
Data/
Dtos/
Exceptions/
Migrations/
Models/
Options/
Services/
```

## Current Status

The API currently includes:

* Product, customer, and order management
* Multi-item orders
* Inventory management
* Entity Framework Core persistence
* JWT authentication
* ASP.NET Core Identity
* Customer and Admin roles
* Customer ownership authorization
* Admin endpoint protection
* Optimistic concurrency handling
* Global exception handling
* Pagination and DTO projections

## Roadmap

* Controlled order lifecycle and state transitions
* Product catalog and categories
* Shopping basket/cart
* Customer addresses
* Checkout workflow
* Transaction and idempotency handling
* Payment integration
* Admin operations and dashboard APIs
* Unit testing
* Integration testing
* CI/CD pipelines
* Azure DevOps
* YAML pipelines
* Docker
* Cloud deployment
* Additional production hardening




