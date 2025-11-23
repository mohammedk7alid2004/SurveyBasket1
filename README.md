# 📊 Survey Management API

A robust and scalable survey management system that enables creating, publishing, and managing surveys, collecting responses, analyzing results, and supporting secure authentication, authorization, background processing, email notifications, rate limiting, and system health monitoring.

---

## 🚀 Features

### 🔐 User Authentication & Authorization
- Secure login using **JWT tokens**
- **Role-based access control** for protected endpoints

### 📋 Survey Management
- Create surveys
- Update surveys
- Delete surveys
- Publish surveys to users

### ❓ Question Management
- Add questions to surveys
- Edit questions
- Remove questions

### 📝 Response Collection
- Collect user responses
- Store responses in the database

### 📈 Result Analysis
- Detailed report generation
- Aggregated response statistics
- Insights per question

### ✉️ Email Notifications
- Send survey invitations
- Send confirmation emails after completion

### 🛡️ Rate Limiting
- Protect API from abuse using **IP-based throttling**

### ❤️ Health Checks
- Monitor API and service availability

### 🕒 Background Jobs (Hangfire)
- Schedule and process background tasks

### 📖 Swagger Integration
- Interactive API documentation

---

## 🛠️ Technologies Used

- **ASP.NET Core 8.0**
- **Entity Framework Core 8.0**
- **Microsoft Identity**
- **FluentValidation**
- **Hangfire**
- **Serilog**
- **Swagger / Swashbuckle**
- **Mapster**

---

## 📦 Installation

```bash
git clone https://github.com/USERNAME/REPO.git
cd REPO
