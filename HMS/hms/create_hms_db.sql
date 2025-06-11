-- HMS Database Creation Script
-- Run this script in SQL Server Management Studio or a compatible tool

-- First, switch to master database
USE master;
GO

-- Drop database if it exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'HMS')
BEGIN
    -- Kill all connections to the database
    DECLARE @kill varchar(8000) = '';  
    SELECT @kill = @kill + 'kill ' + CONVERT(varchar(5), session_id) + ';'  
    FROM sys.dm_exec_sessions
    WHERE database_id  = db_id('HMS')
    EXEC(@kill);

    -- Drop the database
    ALTER DATABASE HMS SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE HMS;
END
GO

-- Create a new database
CREATE DATABASE HMS;
GO

USE HMS;
GO

-- Drop stored procedure if it exists
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_CreatePatientUser')
    DROP PROCEDURE sp_CreatePatientUser;
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
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Admin', 'Patient', 'Doctor')),
    IsActive BIT DEFAULT 1,
    LastLoginDate DATETIME,
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- Insert default users
INSERT INTO tblUser (Username, Password, FullName, Role)
VALUES ('admin', 'admin123', 'System Administrator', 'Admin');

INSERT INTO tblUser (Username, Password, FullName, Role)
VALUES ('patient', 'patient123', 'John Doe', 'Patient');

INSERT INTO tblUser (Username, Password, FullName, Role)
VALUES ('doctor', 'doctor123', 'Dr. Jane Smith', 'Doctor');
GO

-- Create tblDoctorType for doctor specialization
CREATE TABLE tblDoctorType (
    SpecializationID INT IDENTITY(1,1) PRIMARY KEY,
    SpecializationName NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200),
    IsDeleted BIT DEFAULT 0
);

-- Insert sample data for tblDoctorType
INSERT INTO tblDoctorType (SpecializationName, Description) VALUES ('Cardiology', 'Heart and blood vessel disorders');
INSERT INTO tblDoctorType (SpecializationName, Description) VALUES ('Neurology', 'Nervous system disorders');
INSERT INTO tblDoctorType (SpecializationName, Description) VALUES ('Pediatrics', 'Child health');
INSERT INTO tblDoctorType (SpecializationName, Description) VALUES ('Orthopedics', 'Bone and joint disorders');
INSERT INTO tblDoctorType (SpecializationName, Description) VALUES ('Dermatology', 'Skin disorders');
GO

-- Create tblDoctor with specialization option
CREATE TABLE tblDoctor (
    DoctorID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    SpecializationID INT,
    ContactNumber NVARCHAR(20),
    Email NVARCHAR(100),
    Address NVARCHAR(200),
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (SpecializationID) REFERENCES tblDoctorType(SpecializationID)
);

-- Insert sample doctor
INSERT INTO tblDoctor (FirstName, LastName, SpecializationID, ContactNumber, Email)
VALUES ('Jane', 'Smith', 1, '1234567890', 'jane.smith@hospital.com');
GO

-- Create tblPatient
CREATE TABLE tblPatient (
    PatientID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Gender NVARCHAR(10) NOT NULL,
    BloodType NVARCHAR(5),
    ContactNumber NVARCHAR(20),
    Email NVARCHAR(100),
    Address NVARCHAR(200),
    InsuranceNumber NVARCHAR(50),
    Family NVARCHAR(200),
    Status NVARCHAR(20) DEFAULT 'Active',
    ProfilePhoto VARBINARY(MAX),
    IsDeleted BIT DEFAULT 0,
    UserID INT,
    FOREIGN KEY (UserID) REFERENCES tblUser(UserID)
);

-- Insert sample patient
INSERT INTO tblPatient (FirstName, LastName, DateOfBirth, Gender, UserID)
VALUES ('John', 'Doe', '1990-01-01', 'Male', 2);
GO

-- Create tblRoom
CREATE TABLE tblRoom (
    RoomID INT IDENTITY(1,1) PRIMARY KEY,
    RoomNumber NVARCHAR(10) NOT NULL UNIQUE,
    RoomType NVARCHAR(50) NOT NULL,
    Capacity INT NOT NULL,
    Status NVARCHAR(20) DEFAULT 'Available',
    PatientID INT,
    FOREIGN KEY (PatientID) REFERENCES tblPatient(PatientID)
);

-- Insert sample rooms
INSERT INTO tblRoom (RoomNumber, RoomType, Capacity, Status)
VALUES ('101', 'General', 2, 'Available'),
       ('102', 'Private', 1, 'Available'),
       ('103', 'ICU', 1, 'Available');
GO

-- Create tblAppointment
CREATE TABLE tblAppointment (
    AppointmentID INT IDENTITY(1,1) PRIMARY KEY,
    PatientID INT NOT NULL,
    DoctorID INT NOT NULL,
    AppointmentDate DATE NOT NULL,
    AppointmentTime TIME NOT NULL,
    Reason NVARCHAR(200),
    Status NVARCHAR(20) DEFAULT 'Scheduled',
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (PatientID) REFERENCES tblPatient(PatientID),
    FOREIGN KEY (DoctorID) REFERENCES tblDoctor(DoctorID)
);
GO

-- Create tblBilling
CREATE TABLE tblBilling (
    BillingID INT IDENTITY(1,1) PRIMARY KEY,
    PatientID INT NOT NULL,
    InvoiceNumber NVARCHAR(50) NOT NULL UNIQUE,
    ConsultationFee DECIMAL(10,2),
    RoomCharges DECIMAL(10,2),
    MedicineCharges DECIMAL(10,2),
    OtherCharges DECIMAL(10,2),
    TotalAmount DECIMAL(10,2),
    PaymentStatus NVARCHAR(20) DEFAULT 'Pending',
    BillingDate DATE DEFAULT GETDATE(),
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (PatientID) REFERENCES tblPatient(PatientID)
);
GO

-- Create tblInvoice
CREATE TABLE tblInvoice (
    InvoiceID INT IDENTITY(1,1) PRIMARY KEY,
    BillingID INT NOT NULL,
    PatientID INT NOT NULL,
    InvoiceNumber NVARCHAR(50) NOT NULL UNIQUE,
    InvoiceDate DATE DEFAULT GETDATE(),
    DueDate DATE,
    Status NVARCHAR(20) DEFAULT 'Pending',
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (BillingID) REFERENCES tblBilling(BillingID),
    FOREIGN KEY (PatientID) REFERENCES tblPatient(PatientID)
);
GO

-- Create tblDisease
CREATE TABLE tblDisease (
    DiseaseID INT IDENTITY(1,1) PRIMARY KEY,
    DiseaseName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    IsDeleted BIT DEFAULT 0
);

-- Insert sample diseases
INSERT INTO tblDisease (DiseaseName, Description)
VALUES ('Hypertension', 'High blood pressure'),
       ('Diabetes', 'High blood sugar levels'),
       ('Asthma', 'Respiratory condition');
GO

-- Create tblSupplier
CREATE TABLE tblSupplier (
    SupplierID INT IDENTITY(1,1) PRIMARY KEY,
    SupplierName NVARCHAR(100) NOT NULL,
    ContactPerson NVARCHAR(100),
    ContactNumber NVARCHAR(20),
    Email NVARCHAR(100),
    Address NVARCHAR(200),
    IsDeleted BIT DEFAULT 0
);

-- Insert sample supplier
INSERT INTO tblSupplier (SupplierName, ContactPerson, ContactNumber, Email)
VALUES ('MedSupply Co.', 'John Supplier', '9876543210', 'contact@medsupply.com');
GO

-- Create tblMedicine
CREATE TABLE tblMedicine (
    MedicineID INT IDENTITY(1,1) PRIMARY KEY,
    MedicineName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    SupplierID INT,
    Quantity INT DEFAULT 0,
    UnitPrice DECIMAL(10,2),
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (SupplierID) REFERENCES tblSupplier(SupplierID)
);

-- Insert sample medicine
INSERT INTO tblMedicine (MedicineName, Description, SupplierID, Quantity, UnitPrice)
VALUES ('Paracetamol', 'Pain reliever and fever reducer', 1, 100, 5.99);
GO

-- Create stored procedure
CREATE PROCEDURE sp_CreatePatientUser
    @Username NVARCHAR(50),
    @Password NVARCHAR(100),
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Email NVARCHAR(100)
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Insert into tblUser
        INSERT INTO tblUser (Username, Password, FullName, Email, Role)
        VALUES (@Username, @Password, @FirstName + ' ' + @LastName, @Email, 'Patient');
        
        DECLARE @UserID INT = SCOPE_IDENTITY();
        
        -- Insert into tblPatient
        INSERT INTO tblPatient (FirstName, LastName, UserID)
        VALUES (@FirstName, @LastName, @UserID);
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
