# AvanadeTest



📦 Project Overview

This system is composed of four independent microservices, each with its own database, responsibilities, and API surface:

## 🌐 GatewayAPI


The public entry point for all external clients.
Internal APIs (Sales, Stock) are not directly exposed.

Responsibilities

JWT authentication enforcement

Role-based access (Admin, Buyer)

Accepts all requests from UI/client

Forwards them transparently to the internal microservices using:

ProxyService

HttpClient factory

Does not contain domain logic

💸 SalesAPI
Responsibilities

Registers new sales

Validates Buyer and Product existence

Validates stock before confirming sale

Publishes StockUpdatedEvent messages into RabbitMQ:

main

retry

dead-letter

Returns descriptive errors:

Product not found

Insufficient stock

Invalid buyer

No sales found (NotFound with custom error message)

Key Technologies

Dapper repository for high-performance reads & writes

RabbitMQ Publisher

FluentValidation for sales DTOs

Integration tests using WebApplicationFactory

📦 StockAPI
Responsibilities

Full CRUD for product inventory:

Create product

Update product

Read all / by ID

Delete product

Maintains authoritative product stock levels

Exposes product API endpoints consumed by the Gateway

Consumes RabbitMQ messages from SalesAPI to decrease inventory

With automatic retry logic

DLQ (Dead Letter Queue)

Idempotent update logic

Custom exceptions with mapped HTTP status codes

Key Technologies

EF Core repositories

DTO + Validator layer

BackgroundService consumer connected to RabbitMQ

Retry + DLQ queue design

Integration tests for repository and controller

🔐 IdentityAPI
Responsibilities

Admin & Buyer registration

Login & JWT token generation

Role-based authorization (Admin / Buyer)

Password hashing using Argon2id

Security policies:
✔ Lock user after 10 failed attempts
✔ 1-day cooldown
✔ Refresh last login

DTO validation via FluentValidation

Integration tests ensuring controller/service correctness

Key Technologies

Argon2id with:

Custom salt

Configurable memory, iterations, parallelism

FluentValidation

JWT

EF Core repositories

Integration tests using xUnit + real SQL Server test DB
