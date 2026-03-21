# 📚 Book Lending System API

A RESTful API for managing a book lending system where users can register, log in, and borrow or return books from a shared catalog.

---

## 🛠️ Tech Stack

- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM with SQL Server
- **ASP.NET Core Identity** - Authentication & Authorization
- **JWT** - Token-based authentication
- **AutoMapper** - Object mapping
- **MediatR** - CQRS pattern
- **FluentValidation** - Request validation
- **Hangfire** - Background jobs
- **MailKit** - Email notifications
- **Cloudinary** - Image storage
- **Serilog** - Structured logging
- **Swagger** - API documentation
- **XUnit & NSubstitute** - Unit testing

---

## 🏗️ Architecture

Clean Architecture with the following layers:
```
├── BookLending.API            → Controllers, Middleware, Program.cs
├── BookLending.Application    → CQRS Handlers, DTOs, Validators, Abstractions
├── BookLending.Domain         → Entities, Enums, Constants
├── BookLending.Infrastructure → Context, Migrations, Repositories, Services
└── BookLending.Tests          → Unit Tests
```

---

## 🔑 Features

### Authentication
- Register as a Reader
- Login with JWT token

### Book Catalog
- Admins can create, update, and delete books
- Readers can view all books and book details

### Book Borrowing
- Readers can borrow one book at a time
- Due date is automatically set to 7 days after borrowing
- Readers can return borrowed books
- Readers can view their borrowing history
- Admins can view all borrowing records

### Background Jobs
- Daily job checks for overdue books
- Sends email reminders to users with overdue books via MailKit
- Monitored via Hangfire Dashboard at `/hangfire`

---

## 📌 API Endpoints

### Account
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/Account/register` | Register a new user |
| POST | `/api/Account/login` | Login and get JWT token |

### Books
| Method | Endpoint | Description | Role |
|--------|----------|-------------|------|
| POST | `/api/Books/admin` | Create a new book | Admin |
| PUT | `/api/Books/admin/{id}` | Update a book | Admin |
| DELETE | `/api/Books/admin/{id}` | Delete a book | Admin |
| GET | `/api/Books` | Get all books | All |
| GET | `/api/Books/{id}` | Get book by ID | All |

### Borrowing
| Method | Endpoint | Description | Role |
|--------|----------|-------------|------|
| POST | `/api/Borrowing/reader/borrow/{bookId}` | Borrow a book | Reader |
| POST | `/api/Borrowing/reader/return/{bookId}` | Return a book | Reader |
| GET | `/api/Borrowing/reader/my-history` | Get borrowing history | Reader |
| GET | `/api/Borrowing/admin/records` | Get all borrowing records | Admin |

---

## 🧪 Unit Tests

Tests written using **XUnit** and **NSubstitute** with **InMemory EF Core**:

| Area | Test |
|------|------|
| Borrowing | Borrow book when not available → Error |
| Borrowing | Borrow book when available → Success |
| Borrowing | Borrow when user has active borrow → Error |
| Books | Create book with valid data → Success |
| Books | Create book when mapper returns null → Error |
| Auth | Register with valid data → Success |
| Auth | Register with duplicate username → Error |
| Auth | Register with weak password → Error |

---

## ⚙️ Setup

### 1. Clone the repository
```bash
git clone https://github.com/your-username/book-lending-system.git
```

### 2. Configure `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-sql-server-connection-string"
  },
  "JWT": {
    "Key": "your-secret-key",
    "Issuer": "your-issuer",
    "Audience": "your-audience"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your@gmail.com",
    "SenderName": "Book Lending System",
    "SenderPassword": "your-app-password"
  },
  "Cloudinary": {
    "Cloud": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  }
}
```

### 3. Apply migrations
```bash
dotnet ef database update
```

### 4. Run the project
```bash
dotnet run --project BookLending.API
```

### 5. Access Swagger

🔗 **Live API:** [https://book-lending.runasp.net/swagger/index.html](https://book-lending.runasp.net/swagger/index.html)
```
https://localhost:{port}/swagger
```

### 6. Access Hangfire Dashboard
```
https://localhost:{port}/hangfire
```
