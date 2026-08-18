# TechStore – ASP.NET MVC E-Commerce Microservices

## Overview
Scalable e-commerce example built with ASP.NET MVC and a microservices-based backend. Business capabilities are separated into product, order, authentication and payment services.

## Architecture
```text
ASP.NET MVC Client
       |
       +--> Product API --------> Product DB
       +--> Order API ----------> Order DB
       +--> Authentication API -> Auth DB
       +--> Payment API ---------> Payment DB
```

## Core Capabilities
- Product catalog and product details
- Shopping cart operations
- Order creation and history
- Authentication and protected resources
- Payment request/confirmation workflow
- REST-based service integration

## Architecture Principles
- Loose coupling between business capabilities
- Clear service boundaries
- Independent deployment/scaling potential
- Service-level data ownership
- REST integration

## Step-by-Step Flow
1. User opens the ASP.NET MVC client.
2. Product API provides catalog information.
3. User adds products to the cart.
4. Order API creates and persists the order.
5. Authentication protects secured operations.
6. Payment API processes the payment workflow.
7. Order state can be tracked independently of other services.

## Technology Stack
`C#` `ASP.NET MVC` `ASP.NET Core` `REST APIs` `Microservices` `SQL Server` `Distributed Systems`

## LinkedIn Project Description
**Built a modular e-commerce platform using ASP.NET MVC and independent .NET microservices for products, orders, authentication and payments, demonstrating REST integration, service isolation and scalable architecture.**

## Recommended Enhancements
- API Gateway
- Docker/Kubernetes
- RabbitMQ/Azure Service Bus
- Redis caching
- Centralized logging
- OpenTelemetry
- Automated testing and CI/CD

## Repository
https://github.com/NaraP/ASP.NET-MVC-Client-with-Microservices-Architecture
