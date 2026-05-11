# Barber Shop Management System API

A secure and scalable **ASP.NET Core Web API** for managing barber shop operations including appointment booking, barber schedules, authentication, reviews, and automated background processing.

Built using **ASP.NET Core**, **Entity Framework Core**, **SQL Server**, **ASP.NET Identity**, and **JWT Authentication**.

---

## 🚀 Features

* JWT Authentication & Role-Based Authorization (Admin, Barber, Customer)
* Appointment booking system with overlap prevention
* Timezone-safe scheduling with UTC handling
* Barber schedule and availability management
* Automatic availability slot generation
* Customer review & rating system
* Automated review email notifications
* Background services for cleanup and automation
* Mailjet email integration
* Clean layered architecture

---

## 🏗 Architecture

```text id="ovwo6q"
Controller → Service → Generic Repository → DbContext
```

* **Controllers:** business logic, validation, authorization
* **Services:** database operations
* **Repositories:** generic CRUD abstraction
* **DbContext:** Entity Framework Core interaction

---

## 🧰 Tech Stack

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* ASP.NET Identity
* JWT Authentication
* AutoMapper
* Background Services
* Mailjet

---

## ⚙️ Main Functionalities

### 📅 Appointment System

* Book appointments with barbers
* Prevent overlapping bookings
* Generate available time slots
* Store appointments safely in UTC

### ✂️ Barber Scheduling

* Date-based schedules
* Day-off management
* Schedule validation

### ⭐ Review System

* Post-service customer reviews
* Token-based review requests
* Automated review emails

### 🔄 Background Processing

* Archive completed appointments
* Delete old schedules
* Send pending review notifications automatically

---

## 🔐 Security

* JWT-secured endpoints
* Role-based authorization
* ASP.NET Identity integration
* Protected APIs using `[Authorize]`

---

base

---

## 📬 API Testing

Available via:

* Swagger
* Postman

---

## 👨‍💻 Author

Developed by Remah.
