# FU News Management System - PRN232 Assignment 02 (Security Web API)

A secure full-stack News Management application built with **.NET 8 (ASP.NET Core Web API with OData v8 & JWT Authentication/Authorization)** and **ASP.NET Core Razor Pages**.

---

## 🏗 Solutions & Architecture

- **Unified Solution**: `27_LeThanhHung_Assignment02.sln` (chứa cả Backend API và Frontend Client)
- **Backend Solution**: `27_LeThanhHung_PRN232_A02_BE.sln` (chứa `FUNewsManagementAPI`)
- **Frontend Solution**: `27_LeThanhHung_PRN232_A02_FE.sln` (chứa `FUNewsManagementClient`)

```
Assignment/
├── 27_Assignment01_BackEnd/       # Backend Solution
│   └── FUNewsManagementAPI/       # Web API project with OData & JWT Security
│       ├── Controllers/           # Accounts, Categories, NewsArticles, Tags, Auth, Reports
│       ├── DAO/                   # Singleton Data Access Objects & DbContext
│       ├── DTOs/                  # Data Transfer Objects
│       ├── Models/                # EF Core Entities
│       └── Repositories/          # Repository Interfaces & Implementations
├── 27_Assignment01_FrontEnd/      # Frontend Solution
│   └── FUNewsManagementClient/    # Razor Pages Web Application
│       ├── Pages/                 # Public, Admin, and Staff management pages
│       ├── Services/              # ApiService with Bearer Token attachment
│       └── DTOs/                  # Independent Client DTOs
└── FUNewsManagement.sql           # Database setup script
```

---

## 🚀 Setup & Execution Guide

### 1. Database Setup
1. Open SQL Server Management Studio (SSMS).
2. Execute the script [`FUNewsManagement.sql`](./FUNewsManagement.sql) to create the `FUNewsManagement` database and initial seed data.
3. If your SQL Server connection string differs from default (`Server=.;Database=FUNewsManagement;Trusted_Connection=True;TrustServerCertificate=True;`), update it in:
   - `27_Assignment01_BackEnd/FUNewsManagementAPI/appsettings.json`

### 2. Run Backend API
```bash
cd 27_Assignment01_BackEnd/FUNewsManagementAPI
dotnet run --urls "http://localhost:5000;https://localhost:7000"
```
- Swagger UI (có nút Authorize nhập JWT Token): `http://localhost:5000/swagger`

### 3. Run Frontend Client
```bash
cd 27_Assignment01_FrontEnd/FUNewsManagementClient
dotnet run --urls "http://localhost:5001;https://localhost:7001"
```
- Open browser: `http://localhost:5001`

---

## 🔑 Default Accounts

| Role | Email | Password | Access / Privileges |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin@FUNewsManagementSystem.org` | `@@abc123@@` *(hoặc `admin`)* | Account Management, Article Statistics & Reports, Export Excel |
| **Staff** | `IsabellaDavid@FUNewsManagement.org` | `@1` | Category Management, News Article CRUD & Tags, Profile, My Articles |
| **Lecturer** | `EmmaWilliam@FUNewsManagement.org` | `@1` | View public articles, View & Edit Profile, Change Password |
| **Public** | *(No login required)* | - | View active articles, search, filter by category & tag |

