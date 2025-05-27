-- HMS Database Creation Script
-- Run this script in SQL Server Management Studio or a compatible tool

-- Create a new database
CREATE DATABASE HMS;
GO

USE HMS;
GO

-- Drop tables if they exist (in reverse dependency order)
IF OBJECT_ID('tblInvoice', 'U') IS NOT NULL DROP TABLE tblInvoice;
IF OBJECT_ID('tblBilling', 'U') IS NOT NULL DROP TABLE tblBilling;
IF OBJECT_ID('tblAppointment', 'U') IS NOT NULL DROP TABLE tblAppointment;
IF OBJECT_ID('tblRoom', 'U') IS NOT NULL DROP TABLE tblRoom;
IF OBJECT_ID('tblDisease', 'U') IS NOT NULL DROP TABLE tblDisease;
IF OBJECT_ID('tblMedicine', 'U') IS NOT NULL DROP TABLE tblMedicine;
IF OBJECT_ID('tblSupplier', 'U') IS NOT NULL DROP TABLE tblSupplier;
IF OBJECT_ID('tblPatient', 'U') IS NOT NULL DROP TABLE tblPatient;
IF OBJECT_ID('tblDoctor', 'U') IS NOT NULL DROP TABLE tblDoctor;
IF OBJECT_ID('tblDoctorType', 'U') IS NOT NULL DROP TABLE tblDoctorType;
IF OBJECT_ID('tblUser', 'U') IS NOT NULL DROP TABLE tblUser;
GO

-- Table: tblUser
CREATE TABLE tblUser (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Role NVARCHAR(20) NOT NULL,
    IsActive BIT DEFAULT 1,
    LastLoginDate DATETIME,
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- Insert default admin user
INSERT INTO tblUser (Username, Password, FullName, Role)
VALUES ('admin', 'admin123', 'System Administrator', 'Admin');

-- Create tblDoctorType for doctor specialization
CREATE TABLE tblDoctorType (
    SpecializationID INT IDENTITY(1,1) PRIMARY KEY,
    SpecializationName NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200),
    IsDeleted BIT DEFAULT 0
);
GO

-- Create tblDoctor with specialization option
CREATE TABLE tblDoctor (
    DoctorID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    SpecializationID INT,
    ContactNumber NVARCHAR(20),
    Email NVARCHAR(100),
    YearsOfExperience INT,
    ProfilePhoto VARBINARY(MAX),
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (SpecializationID) REFERENCES tblDoctorType(SpecializationID)
);
GO

-- Create tblRoomType for room options
CREATE TABLE tblRoomType (
    RoomTypeID INT IDENTITY(1,1) PRIMARY KEY,
    RoomTypeName NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200),
    IsDeleted BIT DEFAULT 0
);
GO

-- Create tblRoom with room type option
CREATE TABLE tblRoom (
    RoomID INT IDENTITY(1,1) PRIMARY KEY,
    RoomNumber NVARCHAR(20) NOT NULL,
    RoomTypeID INT,
    Floor INT,
    Capacity INT,
    RatePerDay DECIMAL(10, 2),
    Status NVARCHAR(20),
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (RoomTypeID) REFERENCES tblRoomType(RoomTypeID)
);
GO

-- Create tblPatient
CREATE TABLE tblPatient (
    PatientID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    DateOfBirth DATE,
    Gender NVARCHAR(10),
    BloodType NVARCHAR(5),
    ContactNumber NVARCHAR(20),
    Email NVARCHAR(100),
    Address NVARCHAR(200),
    InsuranceNumber NVARCHAR(50),
    Family NVARCHAR(100),
    Status NVARCHAR(20),
    Photo VARBINARY(MAX),
    IsDeleted BIT DEFAULT 0
);
GO

-- Create tblAppointment
CREATE TABLE tblAppointment (
    AppointmentID INT IDENTITY(1,1) PRIMARY KEY,
    PatientID INT NOT NULL,
    DoctorID INT NOT NULL,
    Date DATE NOT NULL,
    Time TIME NOT NULL,
    Reason NVARCHAR(200),
    Status NVARCHAR(20),
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (PatientID) REFERENCES tblPatient(PatientID),
    FOREIGN KEY (DoctorID) REFERENCES tblDoctor(DoctorID)
);
GO

-- Create tblInvoice
CREATE TABLE tblInvoice (
    InvoiceID INT IDENTITY(1,1) PRIMARY KEY,
    PatientID INT NOT NULL,
    TotalAmount DECIMAL(10, 2) NOT NULL,
    InvoiceDate DATE NOT NULL,
    PaymentStatus NVARCHAR(20),
    DueDate DATE,
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (PatientID) REFERENCES tblPatient(PatientID)
);
GO

-- Create tblBilling
CREATE TABLE tblBilling (
    BillingID INT IDENTITY(1,1) PRIMARY KEY,
    PatientID INT NOT NULL,
    ConsultationFee DECIMAL(10, 2),
    RoomCharges DECIMAL(10, 2),
    MedicineCharges DECIMAL(10, 2),
    OtherCharges DECIMAL(10, 2),
    TotalAmount DECIMAL(10, 2) NOT NULL,
    PaymentStatus NVARCHAR(20),
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (PatientID) REFERENCES tblPatient(PatientID)
);
GO

-- Create tblDisease
CREATE TABLE tblDisease (
    DiseaseID INT IDENTITY(1,1) PRIMARY KEY,
    DiseaseName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(200),
    Symptoms NVARCHAR(200),
    Treatment NVARCHAR(200),
    IsDeleted BIT DEFAULT 0
);
GO

-- Create tblSupplier
CREATE TABLE tblSupplier (
    SupplierID INT IDENTITY(1,1) PRIMARY KEY,
    SupplierName NVARCHAR(100) NOT NULL,
    ContactPerson NVARCHAR(100),
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    Address NVARCHAR(200),
    IsDeleted BIT DEFAULT 0
);
GO

-- Create tblMedicine
CREATE TABLE tblMedicine (
    MedicineID INT IDENTITY(1,1) PRIMARY KEY,
    MedicineName NVARCHAR(100) NOT NULL,
    SupplierID INT,
    Dosage NVARCHAR(50),
    SideEffects NVARCHAR(200),
    Price DECIMAL(10, 2),
    Category NVARCHAR(50),
    Description NVARCHAR(200),
    UnitPrice DECIMAL(10, 2),
    StockQuantity INT,
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (SupplierID) REFERENCES tblSupplier(SupplierID)
);
GO

-- Insert sample data for tblDoctorType
INSERT INTO tblDoctorType (SpecializationName, Description) VALUES ('Cardiology', 'Heart and blood vessel disorders');
INSERT INTO tblDoctorType (SpecializationName, Description) VALUES ('Neurology', 'Nervous system disorders');
INSERT INTO tblDoctorType (SpecializationName, Description) VALUES ('Pediatrics', 'Child health');
INSERT INTO tblDoctorType (SpecializationName, Description) VALUES ('Orthopedics', 'Bone and joint disorders');
INSERT INTO tblDoctorType (SpecializationName, Description) VALUES ('Dermatology', 'Skin disorders');
GO

-- Insert sample data for tblRoomType
INSERT INTO tblRoomType (RoomTypeName, Description) VALUES ('Standard', 'Basic room with essential amenities');
INSERT INTO tblRoomType (RoomTypeName, Description) VALUES ('Deluxe', 'Enhanced room with additional comfort');
INSERT INTO tblRoomType (RoomTypeName, Description) VALUES ('Suite', 'Luxury room with premium services');
GO 