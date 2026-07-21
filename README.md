# OwlPost

OwlPost is a high-performance, real-time messaging platform built with .NET 10. It implements a decoupled, event-driven architecture using RabbitMQ for messaging and SignalR for real-time client communication.

## 🚀 Features

- **Real-time Messaging** — Powered by SignalR for low-latency delivery.
- **Decoupled Architecture** — Clean separation of concerns using a Core layer and modular service extensions.
- **Event-Driven** — Asynchronous communication via RabbitMQ for reliable message processing.
- **Granular Rate Limiting** — Built-in protection policies for different operation types:

  | Policy | Capacity | Refill |
  |---|---|---|
  | Read | 200 tokens | 50 / 10s |
  | Write | 20 tokens | 5 / 10s |
  | Authentication | 5 tokens | 1 / min |
  | Admin | 500 tokens | 100 / 10s |
  | Upload | 3 tokens | 1 / 30s |

- **Security** — Dedicated permission layers (`IMessagingPermissionService`, `IRoomPermissionService`) to manage access control.
- **Dockerized** — Full container support for the API and infrastructure (SQL Server, RabbitMQ).

## 🛠 Tech Stack

- **Framework:** .NET 10
- **Database:** SQL Server
- **Message Broker:** RabbitMQ
- **Real-time Engine:** SignalR
- **Architecture:** Unit of Work, Repository Pattern, Dependency Injection
- **Serialization:** Custom strategies with LZ4 and stream pooling

## 📁 Project Structure

| Project | Responsibility |
|---|---|
| `OwlPost.Core` | Domain models, interfaces, and core business logic. |
| `OwlPost.Sql` | Entity Framework Core implementation, repositories, and migrations. |
| `OwlPost.RabbitMq` | Topology management, message consumers, and producer services. |
| `OwlPost.SignalR` | Hub implementations and real-time notification services. |
| `OwlPost.Serializer` | Singleton-pattern serialization with LZ4 support. |
| `OwlPost.Sanitizer` | Input validation and cleaning services. |
| `OwlPost` | Main API entry point, controllers, and configuration. |

## ⚙️ Getting Started

### Prerequisites

- Docker Desktop
- .NET 10 SDK
- JetBrains Rider (Recommended)

### Running with Docker

The project includes a `docker-compose.yaml` to set up the environment automatically:

```bash
docker-compose up -d
```

This starts:

- SQL Server 2022 (Port `1433`)
- RabbitMQ (AMQP: `5672`, Management UI: `15672`)
- OwlPost API (Port `8080`)

### Configuration

Update `appsettings.json` or use environment variables for production:

- `ConnectionStrings:DefaultConnection` — SQL Server connection string.
- `RabbitMqOptions` — Host, Port, and Credential settings.

## 📖 API & Hubs

- **Swagger UI:** `http://localhost:8080/openapi/v1/index.html`
- **SignalR Hub:** `/hubs/chat`

## 📝 Project Implementation Details

- **Database Migrations:** Run `dotnet ef database update --project OwlPost.Sql` to initialize the schema.
- **Authentication:** The system utilizes JWT (JSON Web Tokens) Bearer authentication integrated with ASP.NET Core Identity.

## License

Distributed under the MIT License.

## Maintainer

Maintained by M.Mahdi
