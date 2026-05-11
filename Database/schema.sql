-- ============================================================
-- Error 200 Car Dealership - SQL Server (T-SQL) Schema
-- CSAI202 Fall 2024 - Team 29
-- Run this entire script in SQL Server Management Studio (SSMS)
-- ============================================================

USE master;
GO

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'error200_dealership')
BEGIN
    CREATE DATABASE error200_dealership;
END
GO

USE error200_dealership;
GO

-- ============================================================
-- TABLE: Customer
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Customer')
CREATE TABLE Customer (
    CustomerID    INT IDENTITY(1,1) PRIMARY KEY,
    FirstName     NVARCHAR(50)  NOT NULL,
    LastName      NVARCHAR(50)  NOT NULL,
    Email         NVARCHAR(100) NOT NULL UNIQUE,
    PhoneNumber   NVARCHAR(20),
    Address       NVARCHAR(255),
    City          NVARCHAR(100),
    [State]       NVARCHAR(100),
    Password      NVARCHAR(100) NOT NULL DEFAULT '',
    CreatedAt     DATETIME DEFAULT GETDATE()
);
GO

-- ============================================================
-- TABLE: Manager
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Manager')
CREATE TABLE Manager (
    ManagerID     INT IDENTITY(1,1) PRIMARY KEY,
    FirstName     NVARCHAR(50)  NOT NULL,
    LastName      NVARCHAR(50)  NOT NULL,
    Email         NVARCHAR(100) NOT NULL UNIQUE,
    Password      NVARCHAR(100) NOT NULL,
    PhoneNumber   NVARCHAR(20),
    HireDate      DATE,
    CreatedAt     DATETIME DEFAULT GETDATE()
);
GO

-- ============================================================
-- TABLE: SalesEmployee
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SalesEmployee')
CREATE TABLE SalesEmployee (
    EmployeeID    INT IDENTITY(1,1) PRIMARY KEY,
    ManagerID     INT,
    FirstName     NVARCHAR(50)  NOT NULL,
    LastName      NVARCHAR(50)  NOT NULL,
    Email         NVARCHAR(100) NOT NULL UNIQUE,
    Password      NVARCHAR(100) NOT NULL,
    PhoneNumber   NVARCHAR(20),
    HireDate      DATE,
    CreatedAt     DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ManagerID) REFERENCES Manager(ManagerID)
);
GO

-- ============================================================
-- TABLE: Inventory
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Inventory')
CREATE TABLE Inventory (
    InventoryID   INT IDENTITY(1,1) PRIMARY KEY,
    Location      NVARCHAR(100),
    TotalCars     INT DEFAULT 0,
    LastUpdated   DATETIME DEFAULT GETDATE()
);
GO

-- ============================================================
-- TABLE: Car
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Car')
CREATE TABLE Car (
    CarID         INT IDENTITY(1,1) PRIMARY KEY,
    InventoryID   INT,
    Make          NVARCHAR(50)   NOT NULL,
    Model         NVARCHAR(50)   NOT NULL,
    CarYear       INT            NOT NULL,
    Color         NVARCHAR(30),
    Mileage       INT            DEFAULT 0,
    Price         DECIMAL(12,2)  NOT NULL,
    [Status]      NVARCHAR(20)   DEFAULT 'Available',
    FuelType      NVARCHAR(30),
    Transmission  NVARCHAR(30),
    Description   NVARCHAR(MAX),
    ImageURL      NVARCHAR(255),
    AddedAt       DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (InventoryID) REFERENCES Inventory(InventoryID)
);
GO

-- ============================================================
-- TABLE: Supplier
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Supplier')
CREATE TABLE Supplier (
    SupplierID    INT IDENTITY(1,1) PRIMARY KEY,
    SupplierName  NVARCHAR(100) NOT NULL,
    ContactEmail  NVARCHAR(100),
    PhoneNumber   NVARCHAR(20),
    Address       NVARCHAR(255)
);
GO

-- ============================================================
-- TABLE: Accessory
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Accessory')
CREATE TABLE Accessory (
    AccessoryID   INT IDENTITY(1,1) PRIMARY KEY,
    SupplierID    INT,
    [Name]        NVARCHAR(200) NOT NULL,
    Category      NVARCHAR(50),
    Price         DECIMAL(10,2) NOT NULL,
    Stock         INT           DEFAULT 0,
    ImageURL      NVARCHAR(255),
    Description   NVARCHAR(MAX),
    FOREIGN KEY (SupplierID) REFERENCES Supplier(SupplierID)
);
GO

-- ============================================================
-- TABLE: Payment
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Payment')
CREATE TABLE Payment (
    PaymentID     INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID    INT,
    SupplierID    INT,
    Amount        DECIMAL(12,2) NOT NULL,
    PaymentMethod NVARCHAR(50),
    PaymentDate   DATETIME DEFAULT GETDATE(),
    [Status]      NVARCHAR(20) DEFAULT 'Pending',
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
    FOREIGN KEY (SupplierID) REFERENCES Supplier(SupplierID)
);
GO

-- ============================================================
-- TABLE: Sale
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Sale')
CREATE TABLE Sale (
    SaleID        INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID    INT,
    CarID         INT,
    EmployeeID    INT,
    PaymentID     INT,
    SaleDate      DATE          NOT NULL,
    SalePrice     DECIMAL(12,2) NOT NULL,
    CreatedAt     DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CustomerID)  REFERENCES Customer(CustomerID),
    FOREIGN KEY (CarID)       REFERENCES Car(CarID),
    FOREIGN KEY (EmployeeID)  REFERENCES SalesEmployee(EmployeeID),
    FOREIGN KEY (PaymentID)   REFERENCES Payment(PaymentID)
);
GO

-- ============================================================
-- TABLE: Commission
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Commission')
CREATE TABLE Commission (
    CommissionID  INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeID    INT,
    CustomerID    INT,
    SaleID        INT,
    Amount        DECIMAL(10,2) NOT NULL,
    CommissionDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EmployeeID)  REFERENCES SalesEmployee(EmployeeID),
    FOREIGN KEY (CustomerID)  REFERENCES Customer(CustomerID),
    FOREIGN KEY (SaleID)      REFERENCES Sale(SaleID)
);
GO

-- ============================================================
-- TABLE: Appointment
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Appointment')
CREATE TABLE Appointment (
    AppointmentID   INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID      INT,
    EmployeeID      INT,
    AppointmentType NVARCHAR(100),
    AppointmentDate DATE          NOT NULL,
    AppointmentTime TIME          NOT NULL,
    Area            NVARCHAR(100),
    City            NVARCHAR(100),
    [State]         NVARCHAR(100),
    [Status]        NVARCHAR(20)  DEFAULT 'Scheduled',
    Notes           NVARCHAR(MAX),
    CreatedAt       DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CustomerID)  REFERENCES Customer(CustomerID),
    FOREIGN KEY (EmployeeID)  REFERENCES SalesEmployee(EmployeeID)
);
GO

-- ============================================================
-- TABLE: Feedback  (linked to Appointment per ERD)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Feedback')
CREATE TABLE Feedback (
    FeedbackID    INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID    INT,
    AppointmentID INT,
    Rating        TINYINT       CHECK (Rating BETWEEN 1 AND 5),
    Comments      NVARCHAR(MAX),
    SubmittedAt   DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CustomerID)    REFERENCES Customer(CustomerID),
    FOREIGN KEY (AppointmentID) REFERENCES Appointment(AppointmentID)
);
GO

-- ============================================================
-- TABLE: [Order]
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Order')
CREATE TABLE [Order] (
    OrderID       INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID    INT,
    OrderDate     DATETIME DEFAULT GETDATE(),
    TotalAmount   DECIMAL(12,2),
    [Status]      NVARCHAR(20) DEFAULT 'Pending',
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
);
GO

-- ============================================================
-- TABLE: OrderItem
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderItem')
CREATE TABLE OrderItem (
    OrderItemID   INT IDENTITY(1,1) PRIMARY KEY,
    OrderID       INT,
    AccessoryID   INT,
    Quantity      INT           DEFAULT 1,
    UnitPrice     DECIMAL(10,2),
    FOREIGN KEY (OrderID)     REFERENCES [Order](OrderID),
    FOREIGN KEY (AccessoryID) REFERENCES Accessory(AccessoryID)
);
GO

-- ============================================================
-- SEED DATA: Manager
-- ============================================================
IF NOT EXISTS (SELECT TOP 1 1 FROM Manager)
BEGIN
    INSERT INTO Manager (FirstName, LastName, Email, Password, PhoneNumber, HireDate)
    VALUES ('Admin', 'Manager', 'admin@error200.com', 'admin123', '+1-555-0001', '2019-01-01');
END
GO

-- ============================================================
-- SEED DATA: SalesEmployee
-- ============================================================
IF NOT EXISTS (SELECT TOP 1 1 FROM SalesEmployee)
BEGIN
    INSERT INTO SalesEmployee (ManagerID, FirstName, LastName, Email, Password, PhoneNumber, HireDate) VALUES
    (1, 'James',  'Carter',   'james.carter@error200.com',   'pass123', '+1-555-0101', '2020-01-15'),
    (1, 'Sarah',  'Mitchell', 'sarah.mitchell@error200.com', 'pass123', '+1-555-0102', '2021-03-20'),
    (1, 'Ahmed',  'Hassan',   'ahmed.hassan@error200.com',   'pass123', '+1-555-0103', '2022-06-10');
END
GO

-- ============================================================
-- SEED DATA: Inventory
-- ============================================================
IF NOT EXISTS (SELECT TOP 1 1 FROM Inventory)
BEGIN
    INSERT INTO Inventory (Location, TotalCars) VALUES ('Main Showroom Floor', 8);
END
GO

-- ============================================================
-- SEED DATA: Cars
-- ============================================================
IF NOT EXISTS (SELECT TOP 1 1 FROM Car)
BEGIN
    INSERT INTO Car (InventoryID, Make, Model, CarYear, Color, Mileage, Price, [Status], FuelType, Transmission, Description) VALUES
    (1, 'Mercedes-Benz', 'S-Class',     2023, 'Black',  500,   250000, 'Available', 'Petrol',  'Automatic', 'Luxury sedan with premium features'),
    (1, 'Ferrari',       '488 GTB',     2022, 'Red',    1200,  450000, 'Available', 'Petrol',  'Automatic', 'High-performance sports car'),
    (1, 'Lamborghini',   'Huracan',     2023, 'Yellow', 800,   650000, 'Available', 'Petrol',  'Automatic', 'Supercar with stunning performance'),
    (1, 'Bugatti',       'Chiron',      2022, 'Blue',   300,   4500000,'Available', 'Petrol',  'Automatic', 'The pinnacle of automotive engineering'),
    (1, 'McLaren',       'GT',          2023, 'Orange', 400,   380000, 'Available', 'Petrol',  'Automatic', 'Grand tourer built for long journeys'),
    (1, 'Rolls-Royce',   'Ghost',       2023, 'White',  200,   950000, 'Available', 'Petrol',  'Automatic', 'The ultimate luxury experience'),
    (1, 'Porsche',       '911 Carrera', 2023, 'Silver', 600,   220000, 'Available', 'Petrol',  'Manual',    'Iconic sports car heritage'),
    (1, 'Maserati',      'Ghibli',      2022, 'Grey',   3000,  180000, 'Available', 'Petrol',  'Automatic', 'Italian luxury performance sedan');
END
GO

-- ============================================================
-- SEED DATA: Supplier
-- ============================================================
IF NOT EXISTS (SELECT TOP 1 1 FROM Supplier)
BEGIN
    INSERT INTO Supplier (SupplierName, ContactEmail, PhoneNumber, Address) VALUES
    ('AutoParts Global',    'contact@autopartsglobal.com', '+1-800-1001', '10 Parts Ave, Detroit'),
    ('LuxuryAccessories Co','info@luxacc.com',             '+1-800-1002', '5 Luxury Blvd, Miami');
END
GO

-- ============================================================
-- SEED DATA: Accessories
-- ============================================================
IF NOT EXISTS (SELECT TOP 1 1 FROM Accessory)
BEGIN
    INSERT INTO Accessory (SupplierID, [Name], Category, Price, Stock, ImageURL) VALUES
    (1, 'AOCISKA Car Interior Detailing Brush - Soft Bristle Cleaning',    'Car Interior',    80,    50, 'images/accessories/brush.jpg'),
    (1, 'Car Phone Mount - 360 Rotatable Upgraded Holder',                 'Car Interior',   300,    40, 'images/accessories/mobile-stand.jpg'),
    (1, 'Super Soft Microfiber Car Duster with Extendable Handle',         'Car Exterior',   140,    30, 'images/accessories/wiper.jpg'),
    (1, 'Phosphoor Seat Belt Cover Case - 2 Piece Fits All Cars',          'Car Interior',   150,    60, 'images/accessories/seatbelt.jpg'),
    (2, 'Blind Spot Mirrors - Small Round Convex Rear View Mirror',        'Car Exterior',   100,    80, 'images/accessories/mirrors.jpg'),
    (2, 'Waterproof Car Cover - Aluminum Film PVC Protection',             'Car Exterior',  1305,    20, 'images/accessories/car-cover.jpg'),
    (2, 'Heavy Duty 4pc Front and Rear Rubber Floor Mats',                 'Car Interior',  1000,    35, 'images/accessories/floor-mats.jpg'),
    (2, 'Carbon Fiber Trunk Spoiler Lip Kit Universal',                    'Car Exterior',   500,    15, 'images/accessories/car-spoiler.jpg'),
    (1, 'Carbon Fiber Non-Slip Steering Wheel Cover',                      'Car Interior',   481,    25, 'images/accessories/steering-wheel-cover.jpg'),
    (1, 'Bluetooth Receiver Noise Cancelling 3.5mm AUX Adapter',          'Car Electronics',900,    20, 'images/accessories/car-adapter2.jpg'),
    (2, '10 inch Wireless CarPlay Screen with 4K Dash Cam',               'Car Electronics',10000,  10, 'images/accessories/car-navigation-system.jpg'),
    (2, 'Mud Flaps Kit for Honda Accord Universal Fit',                    'Car Exterior',  10000,   8, 'images/accessories/car-mudflaps.jpg'),
    (1, 'Alloy Wheel Protectors Red Fits 12-19 Inch',                     'Car Exterior',   1000,   30, 'images/accessories/car-wheel-rim-protectors.jpg'),
    (1, 'KaberMisr 2020 LED Fog Light 16 LED 2 Pieces',                   'Car Exterior',    300,   45, 'images/accessories/car-fog-lights.jpg'),
    (2, 'Portable Car Jumper with Tire Inflator 2-in-1',                  'Car Electronics', 4000,  12, 'images/accessories/car-portable-jumpstarter.jpg'),
    (2, 'Alarm and Fingerprint System with Two Remote Controls',           'Car Electronics', 4500,  18, 'images/accessories/car-alarm-system.jpg'),
    (1, 'Black Bumper Guard Strip Anti-Collision Protector',               'Car Exterior',    230,   50, 'images/accessories/car-bumper.jpg'),
    (1, 'Universal Car Seat Back Support Massage Pillow',                  'Car Interior',    100,   40, 'images/accessories/car-backsupport.jpg');
END
GO

PRINT 'Error 200 Dealership database created and seeded successfully.';
GO
