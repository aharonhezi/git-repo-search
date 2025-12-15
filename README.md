# GitHub Repository Search Application

A full-stack application for searching GitHub repositories with bookmarking functionality. Built with Angular 18 and .NET 8 Web API.

---

## Prerequisites

### Backend Requirements

- **.NET SDK 8.0** or later
  - Download from: https://dotnet.microsoft.com/download/dotnet/8.0
  - Verify installation: `dotnet --version`

### Frontend Requirements

- **Node.js LTS (20.x recommended)**
  - Download from: https://nodejs.org/
  - Verify installation: `node --version`

- **npm** (comes with Node.js)
  - Verify installation: `npm --version`

- **Angular CLI**
  - Install globally: `npm install -g @angular/cli`
  - Verify installation: `ng version`

---

## Starting the Project

### Backend Setup

1. **Navigate to the server directory:**
   ```bash
   cd server
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Run the backend:**
   ```bash
   dotnet run
   ```

   The API will be available at:
   - **HTTPS:** `https://localhost:5001`
   - **HTTP:** `http://localhost:5000`
   - **Swagger UI:** `https://localhost:5001/swagger`

### Frontend Setup

1. **Navigate to the client directory:**
   ```bash
   cd client
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```
   
   > **Note:** Use `npm` only. Do not use `pnpm` or `yarn`.

3. **Run the frontend:**
   ```bash
   ng serve
   ```

   The application will be available at: **`http://localhost:4200`**

---

## User Credentials

The application uses **hard-coded user credentials** for authentication. This is intentional for this exercise/demo project.

### Available Test Accounts

| Username | Password      |
|----------|---------------|
| `admin`  | `admin#pass#` |
| `user1`  | `password123` |
| `user2`  | `secret@2024` |

### Why Are Credentials Hard-Coded?

The user credentials are hard-coded in the `AuthenticationService` class (`server/Services/AuthenticationService.cs`) as a simple in-memory dictionary.

This approach was chosen for the following reasons:

1. **Simplicity**  
   No database setup or configuration is required, making it easy to get the project running quickly.

2. **Demo/Exercise Purpose**  
   This is a coding exercise focused on demonstrating the application's core functionality (GitHub search, bookmarks, authentication flow) rather than production-ready user management.

3. **Zero Dependencies**  
   Eliminates the need for database migrations, connection strings, or additional infrastructure.

4. **Quick Testing**  
   Allows immediate testing of the authentication and user isolation features without additional setup steps.

> **⚠️ Important:** In a production environment, credentials should **never** be hard-coded. A proper implementation would use:
> - A secure database for user storage
> - Password hashing (e.g., bcrypt, Argon2)
> - Secure password policies
> - User registration and management features
> - Proper secret management and environment variables

---

## JWT Secret Keys

The application uses **hard-coded JWT secret keys** in `server/appsettings.json` and `server/appsettings.Development.json`. This is intentional for this exercise/demo project to simplify setup and eliminate the need for environment variable configuration.

> **⚠️ Important:** In a production environment, JWT secret keys should **never** be hard-coded or committed to version control. Use environment variables or secure secret management services instead.