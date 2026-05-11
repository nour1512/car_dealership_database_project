# 🚗 Error 200 Car Dealership
ASP.NET Core Razor Pages + SQL Server (SSMS)

---

## 📁 Project Structure

```
Error200Dealership/
├── Error200Dealership.sln
├── Error200Dealership.csproj      ← NuGet: System.Data.SqlClient only
├── Program.cs
├── appsettings.json
├── Database/
│   └── schema.sql                 ← Run this in SSMS first
├── wwwroot/
│   ├── css/                       ← All Carvilla CSS (unchanged)
│   ├── fonts/
│   └── images/accessories/        ← Accessory images
└── Pages/
    ├── Index.cshtml / .cs         ← Login + Sign Up
    ├── Customer.cshtml / .cs      ← Customer homepage (cars list)
    ├── Appointments.cshtml / .cs  ← Book appointment + history
    ├── Accessories.cshtml / .cs   ← Shop + cart + checkout
    ├── FeedbackPage.cshtml / .cs  ← Leave & view reviews
    ├── Employee.cshtml / .cs      ← Employee dashboard
    ├── Manager.cshtml / .cs       ← Full admin: CRUD all tables
    └── OrderHistory.cshtml / .cs  ← Customer order history
```

---

##  Setup — 3 Steps

### Step 1 — Create the Database in SSMS

1. Open **SQL Server Management Studio**
2. Connect to your server instance
3. Click **File → Open → File** and open `Database/schema.sql`
4. Press **F5** to execute

This creates the `error200_dealership` database with all tables and seed data.

### Step 2 — Update the Connection String

Open **every** `.cshtml.cs` file and update this line with your SQL Server instance name:

```csharp
private string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=error200_dealership;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
```

Common values for `Data Source`:
| Instance Type | Value |
|---|---|
| SQL Server Express | `.\SQLEXPRESS` |
| Default instance | `.` or `localhost` |
| Named instance | `.\INSTANCENAME` |

### Step 3 — Run the Project

```bash
cd Error200Dealership
dotnet restore
dotnet run
```

Or press **F5** in Visual Studio / VS Code.

Open: **http://localhost:5000**

---

##  Login Credentials

| Role     | Email                        | Password |
|----------|------------------------------|----------|
| Manager  | `admin@error200.com`         | `admin123` |
| Employee | `james.carter@error200.com`  | `pass123`  |
| Customer | Register via Sign Up form    | your choice |

**Quick admin shortcut:** type `admin` / `admin` in the login form.

---

##  Database Tables (13 tables)

| Table          | Purpose                              |
|----------------|--------------------------------------|
| `Customer`     | Registered customers + login         |
| `Manager`      | Admin accounts                       |
| `SalesEmployee`| Employee accounts (managed by Manager)|
| `Inventory`    | Showroom inventory locations         |
| `Car`          | Vehicle listings                     |
| `Supplier`     | Accessory suppliers                  |
| `Accessory`    | Car accessories for sale             |
| `Payment`      | Payment records                      |
| `Sale`         | Car purchase transactions            |
| `Commission`   | Employee commissions on sales        |
| `Appointment`  | Bookings by customers                |
| `Feedback`     | Customer reviews + ratings           |
| `Order`        | Accessory cart orders                |
| `OrderItem`    | Line items per order                 |

---

##  Pages & Features

| Page | Features |
|------|----------|
| `Index` | Login (Customer / Employee / Manager) + Customer Sign Up |
| `Customer` | Hero, services, live car inventory from DB, CTA |
| `Appointments` | Book appointment → saved to DB, view recent bookings |
| `Accessories` | Live products from DB, add-to-cart, checkout → saves order |
| `FeedbackPage` | Star rating form → saved to DB, live reviews list |
| `Employee` | View appointments, mark complete, see cars + customers |
| `Manager` | Full CRUD: Cars, Employees, Appointments, Accessories, Suppliers, Orders, Feedback |
| `OrderHistory` | List of all orders placed |

---

##  Tech Stack

| Layer | Technology |
|-------|-----------|
| Language | C# 12 |
| Framework | ASP.NET Core 8 Razor Pages |
| Database | Microsoft SQL Server (via SSMS) |
| Data Access | `System.Data.SqlClient` — raw SQL, no ORM |
| Frontend CSS | Carvilla Template (Bootstrap 3, Poppins, Font Awesome) |
