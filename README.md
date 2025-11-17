🚀 Survey-App API

A scalable and secure survey and polling API that enables users to register, participate in polls, and receive real-time updates. The system supports admin operations, authentication, background jobs, and performance-optimized data access through caching and layered architecture.

🏗️ Architecture

3-Tier Architecture:
Controller → Service → Repository

🛠️ Tech Stack

ASP.NET Core

Entity Framework Core

SQL Server

Redis (Caching)

Hangfire (Background Jobs)

JWT + ASP.NET Identity

LINQ, Serilog, SendGrid

API Versioning & Rate Limiting

🔑 Key Features

✔ Secure & Scalable RESTful API built using ASP.NET Core and clean 3-tier architecture principles.

✔ Authentication & Authorization using JWT, ASP.NET Identity, Refresh Tokens, and permission-based access control.

✔ Efficient Data Access with Entity Framework Core and optimized LINQ queries.

✔ API Versioning, Rate Limiting & Health Checks for improved reliability and backward compatibility.

✔ Centralized Error Handling and structured application logging using Serilog.

✔ Background Processing powered by Hangfire (scheduled tasks, email jobs, cleanup tasks, etc.).

✔ Redis Caching to enhance performance and reduce database load during frequent polling operations.

✔ Pagination & Filtering integrated across endpoints for scalable data retrieval.

✔ Clean Repository & Service Layers ensuring maintainability and testability.

✔ SendGrid Email Integration for notifications, verification emails, and scheduled updates.

📌 Overview

This project provides a full-featured survey and polling backend, allowing:

User registration and authentication

Poll creation, publishing, and participation

Background updates and email notifications

Real-time performance improvements through caching

Admin capabilities for managing users and polls

It was built with scalability, maintainability, and clean architecture as core principles.
