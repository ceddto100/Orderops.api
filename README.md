# OrderOps API

A portfolio-ready internal operations API for a direct-to-consumer e-commerce business. Built to learn .NET, SQL Server, Entity Framework Core, and backend architecture patterns relevant to enterprise development.

## Tech Stack

- **ASP.NET Core 8** Web API
- **C#**
- **Entity Framework Core** (SQL Server provider)
- **SQL Server**
- **Swagger / OpenAPI** for API documentation
- Clean service-layer architecture

## Domain

This API manages six core entities for an e-commerce operations system:

| Entity | Purpose |
|---|---|
| **Customer** | People who place orders |
| **Product** | Items available for sale (with SKU and stock tracking) |
| **Order** | A customer's purchase, containing one or more items |
| **OrderItem** | Line items within an order |
| **Payment** | Money received against an order |
| **Refund** | Money returned against a payment |

## Business Rules

- **Stock validation**: Cannot create an order if any product has insufficient stock
- **Stock reduction**: When an order is created, stock quantities are reduced automatically
- **Calculated totals**: Order total is computed from (quantity x unit price) of all items
- **Payment limits**: Cannot pay more than the remaining order balance
- **Refund limits**: Cannot refund more than the original payment amount
- **Status flow**: Orders move through `Pending` -> `Paid` -> `Refunded`
- **Validation errors**: All rules return clear, specific error messages

## Setup Instructions

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB, Express, or Developer edition)

### 1. Clone and navigate

```bash
git clone <your-repo-url>
cd Orderops.api/src/OrderOps.Api
```

### 2. Update connection string

Edit `appsettings.json` or `appsettings.Development.json` if your SQL Server instance uses a different name:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=OrderOpsDb;Trusted_Connection=true;TrustServerCertificate=true;"
}
```

For SQL Server with a password:
```json
"DefaultConnection": "Server=localhost;Database=OrderOpsDb;User Id=sa;Password=YourPassword;TrustServerCertificate=true;"
```

### 3. Install EF Core tools (if needed)

```bash
dotnet tool install --global dotnet-ef
```

### 4. Create the database with migrations

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

This creates the database, all tables, and seeds sample Customers and Products.

### 5. Run the API

```bash
dotnet run
```

The API will start at `http://localhost:5000`. Swagger UI opens at the root URL.

## API Endpoints

| Method | Route | Description |
|---|---|---|
| GET | `/api/customers` | List all customers |
| POST | `/api/customers` | Create a customer |
| GET | `/api/products` | List all products |
| POST | `/api/products` | Create a product |
| GET | `/api/orders` | List all orders |
| GET | `/api/orders/{id}` | Get order by ID (with items) |
| POST | `/api/orders` | Create an order |
| POST | `/api/payments` | Record a payment for an order |
| POST | `/api/refunds` | Create a refund for a payment |
| GET | `/api/reports/daily-revenue` | Daily revenue report |
| GET | `/api/reports/top-products` | Top-selling products report |

### Example: Create an Order

```json
POST /api/orders
{
  "customerId": 1,
  "items": [
    { "productId": 1, "quantity": 2 },
    { "productId": 3, "quantity": 1 }
  ]
}
```

### Example: Record a Payment

```json
POST /api/payments
{
  "orderId": 1,
  "amount": 109.97,
  "paymentType": "CreditCard"
}
```

### Example: Create a Refund

```json
POST /api/refunds
{
  "paymentId": 1,
  "amount": 29.99,
  "reason": "Item arrived damaged"
}
```

## Project Structure

```
src/OrderOps.Api/
├── Controllers/       # HTTP endpoints - thin, delegates to services
├── Data/              # DbContext, EF configuration, seed data
├── DTOs/              # Request/response objects (no database concerns)
├── Middleware/         # Global exception handling
├── Models/            # Entity classes matching database tables
├── Services/          # Business logic layer
├── Program.cs         # App startup and dependency injection
├── appsettings.json   # Configuration
sql/
└── useful-queries.sql # SQL learning queries against this schema
```

## What I Learned from the Refactor

This project was intentionally built in two mental stages:

### Stage A (Legacy-like patterns) - what to avoid:
- All business logic inside controller action methods
- Database queries mixed directly with HTTP response logic
- No DTOs - returning raw entity objects (exposes internal structure)
- No service layer - controllers doing validation, querying, and saving
- No centralized error handling - try/catch in every endpoint
- Hard to test because logic is coupled to HTTP context

### Stage B (Clean version) - what was applied:
- **Service layer** extracts all business logic out of controllers
- **DTOs** separate what the API sends/receives from what the database stores
- **Exception middleware** handles errors in one place
- **Dependency injection** makes services testable and replaceable
- **Thin controllers** just receive requests and return responses
- **Separation of concerns** means each file has one job

## SQL Learning

The `sql/useful-queries.sql` file contains 10 practical queries including:
- JOINs across multiple tables
- GROUP BY with aggregate functions
- HAVING clauses for filtering groups
- COALESCE for handling NULLs
- Date functions and formatting
- Subquery patterns

## Testing with Swagger

1. Run the API (`dotnet run`)
2. Open `http://localhost:5000` in your browser
3. You'll see all endpoints listed with "Try it out" buttons
4. Start by listing products (GET /api/products) to see seed data
5. Create an order, then pay for it, then try a refund
6. Check the reports to see your data reflected
