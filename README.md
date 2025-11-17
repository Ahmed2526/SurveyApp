<div align="center">

# 🚀 Survey-App API

### A scalable and secure survey and polling API built with ASP.NET Core

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/sql-server)
[![Redis](https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white)](https://redis.io/)
[![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=JSON%20web%20tokens&logoColor=white)](https://jwt.io/)

[Features](#-key-features) • [Architecture](#%EF%B8%8F-architecture) • [Tech Stack](#%EF%B8%8F-tech-stack) • [Getting Started](#-getting-started) • [API Documentation](#-api-documentation)

</div>

---

## 📋 Overview

Survey-App API is a **production-ready** survey and polling platform that enables users to register, participate in polls, and receive real-time updates. Built with enterprise-grade patterns and best practices, it provides a robust foundation for polling systems with admin operations, authentication, background jobs, and performance optimization.

### 🎯 What It Does

- **User Management**: Secure registration and authentication with email verification
- **Poll Creation**: Create, publish, and manage surveys with multiple question types
- **Real-Time Participation**: Users can vote and see results instantly
- **Background Processing**: Automated email notifications and scheduled cleanup tasks
- **Admin Dashboard**: Comprehensive management of users, polls, and system health
- **Performance Optimization**: Redis caching and optimized queries for high throughput

---

## ✨ Key Features

### 🔐 Security & Authentication
- **JWT Authentication** with refresh token rotation
- **ASP.NET Identity** for user management
- **Role-Based Access Control** (RBAC) with custom policies
- **Email Verification** and password reset flows
- **Rate Limiting** to prevent abuse

### ⚡ Performance & Scalability
- **Redis Caching** (Upstash) for frequently accessed data
- **Optimized LINQ Queries** with proper indexing
- **Pagination & Filtering** on all list endpoints
- **Asynchronous Operations** throughout the stack

### 🏗️ Architecture & Design
- **3-Tier Architecture**: Controller → Service → Repository
- **Repository Pattern** for data access abstraction
- **Dependency Injection** for loose coupling
- **Clean Code Principles** with SOLID design patterns
- **API Versioning** for backward compatibility

### 📊 Monitoring & Reliability
- **Health Checks** for Redis, SQL Server, and application status
- **Structured Logging** with Serilog (Console, File, Seq)
- **Global Exception Handling** with proper HTTP status codes
- **Correlation IDs** for request tracing

### 🔄 Background Processing
- **Hangfire** for job scheduling and processing
- **Email Notifications** via SendGrid
- **Scheduled Cleanup** tasks
- **Retry Policies** for failed operations

### 📧 Email Integration
- **SendGrid** for transactional emails
- **HTML Email Templates** for professional communication
- **Email Verification** during registration
- **Password Reset** flows
- **Custom Notifications** for poll updates
---

## 🏗️ Architecture

### Three-Tier Architecture
```
┌─────────────────────────────────────────────────────────────┐
│                     Presentation Layer                       │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │ Controllers  │  │  Middleware  │  │   Filters    │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│                      Business Logic Layer                    │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   Services   │  │  Validators  │  │    DTOs      │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│                      Data Access Layer                       │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │ Repositories │  │   DbContext  │  │   Entities   │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
```

## 🛠️ Tech Stack

### Core Technologies

| Technology | Purpose | Version |
|------------|---------|---------|
| **ASP.NET Core** | Web API Framework | 8.0 |
| **Entity Framework Core** | ORM & Data Access | 8.0 |
| **SQL Server** | Relational Database | 2022 |
| **Redis (Upstash)** | Distributed Caching | Latest |
| **Hangfire** | Background Job Processing | 1.8+ |

### Authentication & Security

- **ASP.NET Identity** - User management
- **JWT** - Token-based authentication

### Infrastructure & DevOps

- **Serilog** - Structured logging
- **SendGrid** - Email service
- **Swagger/OpenAPI** - API documentation
- **Health Checks** - Application monitoring
- **Rate Limiting** - API protection

### Development Tools

- **AutoMapper** - Object mapping
- **FluentValidation** - Input validation
---

## 🚀 Getting Started

### Prerequisites

- **.NET 8 SDK** or later
- **SQL Server** (2019 or later)
- **Redis** (or Upstash account)
- **SendGrid Account** (for emails)
- **Visual Studio 2022**

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👥 Authors

- **Ahmed ebrahim**

---


<div align="center">

**⭐ If you found this project helpful, please give it a star!**

Made with ❤️ using ASP.NET Core

</div>
