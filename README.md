\# Order Management API



A backend Order Management API built with C#, ASP.NET Core, Entity Framework Core, and SQL Server.



\## Technologies



\* C#

\* .NET 10

\* ASP.NET Core Web API

\* Entity Framework Core

\* SQL Server

\* REST API



\## Features



\* Product management

\* Customer management

\* Order management

\* Multi-item orders

\* Inventory/stock management

\* Historical product price snapshots

\* DTO-based API responses

\* Pagination

\* Entity Framework Core migrations

\* Database relationships and foreign keys

\* Unique customer email index

\* Global exception handling

\* Optimistic concurrency using SQL Server `rowversion`

\* Concurrency conflicts returned as HTTP `409 Conflict`

\* Asynchronous database operations

\* Efficient LINQ queries and DTO projections



\## Database Relationships



```text

Customer 1 ──── \* Order



Order 1 ──── \* OrderItem



Product 1 ──── \* OrderItem

```



\## Project Structure



```text

Controllers/

Data/

Dtos/

Exceptions/

Migrations/

Models/

Services/

```



\## Current Status



The core ASP.NET Core and Entity Framework Core backend functionality is complete. The project is being developed as part of a structured backend development.



