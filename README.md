# FU News Management System - PRN232 Assignment 02 (Security Web API)

A secure full-stack News Management application built with **.NET 8 (ASP.NET Core Web API with OData v8 & JWT Authentication/Authorization)** and **ASP.NET Core Razor Pages**.

---

## 🏗 Solutions & Architecture

Dự án được phân chia độc lập thành 2 thư mục **Assignment01** và **Assignment02**:

- **Assignment01**:
  - Unified Solution: `Assignment01/27_LeThanhHung_Assignment01.sln`
  - Backend Solution: `Assignment01/27_Assignment01_BackEnd/27_Assignment01_BackEnd.sln`
  - Frontend Solution: `Assignment01/27_Assignment01_FrontEnd/27_Assignment01_FrontEnd.sln`
- **Assignment02**:
  - Unified Solution: `Assignment02/27_LeThanhHung_Assignment02.sln`
  - Backend Solution: `Assignment02/27_Assignment02_BackEnd/27_LeThanhHung_PRN232_A02_BE.sln` (hoặc `27_Assignment02_BackEnd.sln`)
  - Frontend Solution: `Assignment02/27_Assignment02_FrontEnd/27_LeThanhHung_PRN232_A02_FE.sln` (hoặc `27_Assignment02_FrontEnd.sln`)

```
Assignment/
├── Assignment01/
│   ├── 27_Assignment01_BackEnd/
│   │   ├── 27_Assignment01_BackEnd.sln
│   │   └── FUNewsManagementAPI/
│   ├── 27_Assignment01_FrontEnd/
│   │   ├── 27_Assignment01_FrontEnd.sln
│   │   └── FUNewsManagementClient/
│   └── 27_LeThanhHung_Assignment01.sln    # Solution tổng hợp Ass01
│
├── Assignment02/
│   ├── 27_Assignment02_BackEnd/
│   │   ├── 27_LeThanhHung_PRN232_A02_BE.sln
│   │   ├── 27_Assignment02_BackEnd.sln
│   │   └── FUNewsManagementAPI/
│   ├── 27_Assignment02_FrontEnd/
│   │   ├── 27_LeThanhHung_PRN232_A02_FE.sln
│   │   ├── 27_Assignment02_FrontEnd.sln
│   │   └── FUNewsManagementClient/
│   └── 27_LeThanhHung_Assignment02.sln    # Solution tổng hợp Ass02
│
├── FUNewsManagement.sql
├── README.md
└── PROJECT_ANALYSIS.md
```

---

## 🚀 Setup & Execution Guide

### 1. Database Setup
1. Open SQL Server Management Studio (SSMS).
2. Execute the script [`FUNewsManagement.sql`](./FUNewsManagement.sql) to create the `FUNewsManagement` database and initial seed data.
3. If your SQL Server connection string differs from default (`Server=.;Database=FUNewsManagement;Trusted_Connection=True;TrustServerCertificate=True;`), update it in:
   - `Assignment02/27_Assignment02_BackEnd/FUNewsManagementAPI/appsettings.json`

### 2. Run Backend API (Assignment 02)
```bash
cd Assignment02/27_Assignment02_BackEnd/FUNewsManagementAPI
dotnet run --urls "http://localhost:5000;https://localhost:7000"
```
- Swagger UI (có nút Authorize nhập JWT Token): `http://localhost:5000/swagger`

### 3. Run Frontend Client (Assignment 02)
```bash
cd Assignment02/27_Assignment02_FrontEnd/FUNewsManagementClient
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

