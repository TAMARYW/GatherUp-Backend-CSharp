# GatherUp — Event Management System---

## Features

### Authentication & Users

* User login with JWT authentication
* Secure API access using Bearer Tokens
* User profile and notification preferences management

### Event Management

* Create and update events
* Define event owner and managers
* Manage event details and schedules

### Participants

* Invite participants via email
* RSVP management (Approve / Decline attendance)
* Bulk invitation sending

### Financial Management

* Payment tracking
* Vendor and debt management
* Digital receipt uploads
* Net budget calculation
* Automated payment reminders

### Polls

* Create polls with multiple questions and options
* Vote and update votes
* View poll results with percentages

### Notifications

* Automatic email notifications
* Configurable notification preferences per user

Supported notification types:

* Attendance confirmations
* Payment reminders
* Poll votes
* New polls
* Event updates

---

## Technologies

| Technology           | Description             |
| -------------------- | ----------------------- |
| .NET 10              | Backend framework       |
| ASP.NET Core Web API | REST API                |
| XML Storage          | Persistent data storage |
| JWT Bearer Tokens    | Authentication          |
| Swagger / OpenAPI    | API documentation       |
| SMTP (Gmail)         | Email delivery          |

---

## Project Structure

```text
GatherUpSystem/
├── GatherUp.Core/            # Domain models, interfaces, exceptions
├── GatherUp.BL/              # Business logic services
├── GatherUp.Infrastructure/  # XML repositories, JWT, email services
├── GatherUp.API/             # Controllers, DTOs, middleware
└── XML/                      # XML data files
```

---

## Getting Started

### Prerequisites

* .NET 10 SDK

### Clone Repository

```bash
git clone https://github.com/TAMARYW/GatherUp-Backend-CSharp.git
cd GatherUp-Backend-CSharp/GatherUpSystem
```

### Configure Application Settings

Edit:

```text
GatherUp.API/appsettings.json
```

```json
{
  "Jwt": {
    "SecretKey": "YOUR_SECRET_KEY_HERE",
    "Issuer": "GatherUp.API",
    "Audience": "GatherUp.Client",
    "ExpiryMinutes": 120
  },
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "SenderEmail": "YOUR_EMAIL@gmail.com",
    "SenderName": "GatherUp",
    "AppPassword": "YOUR_APP_PASSWORD",
    "EnableSsl": true
  }
}
```

Generate a Gmail App Password:
https://support.google.com/accounts/answer/185833

### Run the API

```bash
cd GatherUp.API
dotnet run
```

### Open Swagger

```text
https://localhost:{PORT}/swagger
```

---

## Data Storage

All application data is stored in XML files under the `XML/` directory.

| File                 | Purpose                 |
| -------------------- | ----------------------- |
| Person.xml           | Registered users        |
| Event.xml            | Events and participants |
| VendorAllocation.xml | Vendors and receipts    |
| Poll.xml             | Polls and votes         |
| ReceiptDetails.xml   | Receipt repository      |

---

## Authentication

Login endpoint returns a JWT token:

```http
POST /api/Auth/login
```

Example response:

```json
{
  "token": "jwt-token",
  "id": 1,
  "name": "John Doe"
}
```

Authenticated requests must include:

```http
Authorization: Bearer {token}
```

---

## Common HTTP Responses

| Status Code               | Description                    |
| ------------------------- | ------------------------------ |
| 200 OK                    | Request completed successfully |
| 400 Bad Request           | Business validation error      |
| 401 Unauthorized          | Missing or invalid token       |
| 404 Not Found             | Requested resource not found   |
| 500 Internal Server Error | Unexpected server error        |

---

## API Documentation

Swagger UI is available after running the application:

```text
https://localhost:{PORT}/swagger
```

Use Swagger to:

* Explore endpoints
* Test requests
* View request/response schemas
* Authenticate using JWT tokens

---

## Architecture

The project follows a layered architecture:

```text
API Layer
    ↓
Business Logic Layer (BL)
    ↓
Infrastructure Layer
    ↓
XML Storage
```

### Responsibilities

**GatherUp.API**

* Controllers
* DTOs
* Middleware
* Authentication configuration

**GatherUp.BL**

* Business rules
* Validation
* Application services

**GatherUp.Infrastructure**

* XML repositories
* JWT token generation
* Email services

**GatherUp.Core**

* Domain models
* Interfaces
* Custom exceptions

```
```
