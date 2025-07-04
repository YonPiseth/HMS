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

-- Drop stored procedures if they exist
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_CreatePatientUser')
    DROP PROCEDURE sp_CreatePatientUser;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_ScheduleAppointment')
    DROP PROCEDURE sp_ScheduleAppointment;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_AllocateRoom')
    DROP PROCEDURE sp_AllocateRoom;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GenerateInvoice')
    DROP PROCEDURE sp_GenerateInvoice;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_UpdateMedicineStock')
    DROP PROCEDURE sp_UpdateMedicineStock;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_CreatePatientVisit')
    DROP PROCEDURE sp_CreatePatientVisit;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_ScheduleRoomMaintenance')
    DROP PROCEDURE sp_ScheduleRoomMaintenance;
GO

-- Drop tables if they exist (in reverse dependency order to avoid foreign key issues)
IF OBJECT_ID('tblBillingLineItem', 'U') IS NOT NULL DROP TABLE tblBillingLineItem;
IF OBJECT_ID('tblPayment', 'U') IS NOT NULL DROP TABLE tblPayment;
IF OBJECT_ID('tblPrescriptionDetail', 'U') IS NOT NULL DROP TABLE tblPrescriptionDetail;
IF OBJECT_ID('tblPrescription', 'U') IS NOT NULL DROP TABLE tblPrescription;
IF OBJECT_ID('tblMedicineUsage', 'U') IS NOT NULL DROP TABLE tblMedicineUsage;
IF OBJECT_ID('tblMedicineStock', 'U') IS NOT NULL DROP TABLE tblMedicineStock;
IF OBJECT_ID('tblRoomAllocation', 'U') IS NOT NULL DROP TABLE tblRoomAllocation;
IF OBJECT_ID('tblRoomMaintenance', 'U') IS NOT NULL DROP TABLE tblRoomMaintenance;
IF OBJECT_ID('tblDoctorSchedule', 'U') IS NOT NULL DROP TABLE tblDoctorSchedule;
IF OBJECT_ID('tblPatientDocument', 'U') IS NOT NULL DROP TABLE tblPatientDocument;
IF OBJECT_ID('tblPatientVisit', 'U') IS NOT NULL DROP TABLE tblPatientVisit;
IF OBJECT_ID('tblAppointment', 'U') IS NOT NULL DROP TABLE tblAppointment;
IF OBJECT_ID('tblBilling', 'U') IS NOT NULL DROP TABLE tblBilling;
IF OBJECT_ID('tblInvoice', 'U') IS NOT NULL DROP TABLE tblInvoice; -- Drop Invoice before Billing due to FK in Billing
IF OBJECT_ID('tblRoom', 'U') IS NOT NULL DROP TABLE tblRoom;
IF OBJECT_ID('tblPatientDisease', 'U') IS NOT NULL DROP TABLE tblPatientDisease;
IF OBJECT_ID('tblDisease', 'U') IS NOT NULL DROP TABLE tblDisease;
IF OBJECT_ID('tblMedicine', 'U') IS NOT NULL DROP TABLE tblMedicine;
IF OBJECT_ID('tblMedicineCategory', 'U') IS NOT NULL DROP TABLE tblMedicineCategory;
IF OBJECT_ID('tblSupplier', 'U') IS NOT NULL DROP TABLE tblSupplier;
IF OBJECT_ID('tblPatient', 'U') IS NOT NULL DROP TABLE tblPatient;
IF OBJECT_ID('tblDoctor', 'U') IS NOT NULL DROP TABLE tblDoctor;
IF OBJECT_ID('tblDoctorType', 'U') IS NOT NULL DROP TABLE tblDoctorType;
IF OBJECT_ID('tblUser', 'U') IS NOT NULL DROP TABLE tblUser;
GO

-- Create tblUser
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
GO

-- Create tblDoctorType
CREATE TABLE tblDoctorType (
    SpecializationID INT IDENTITY(1,1) PRIMARY KEY,
    SpecializationName NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200),
    IsDeleted BIT DEFAULT 0
);
GO

-- Create tblDoctor
CREATE TABLE tblDoctor (
    DoctorID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    SpecializationID INT,
    ContactNumber NVARCHAR(20),
    Email NVARCHAR(100),
    Address NVARCHAR(200),
    Qualification NVARCHAR(100),
    Experience INT,
    WorkingHours NVARCHAR(100),
    Department NVARCHAR(50),
    YearsOfExperience INT,
    ProfilePhoto VARBINARY(MAX),
    IsAvailable BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (SpecializationID) REFERENCES tblDoctorType(SpecializationID)
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
    PatientFamily NVARCHAR(200),
    Status NVARCHAR(20) DEFAULT 'Active',
    ProfilePhoto VARBINARY(MAX),
    LastVisitDate DATE,
    UserID INT, -- Kept as per your original schema
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (UserID) REFERENCES tblUser(UserID)
);
GO

-- Create tblRoom
CREATE TABLE tblRoom (
    RoomID INT IDENTITY(1,1) PRIMARY KEY,
    RoomNumber NVARCHAR(20) NOT NULL UNIQUE,
    RoomType NVARCHAR(50),
    Floor INT,
    Capacity INT,
    RatePerDay DECIMAL(10,2),
    Status NVARCHAR(20) DEFAULT 'Available',
    PatientID INT,
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (PatientID) REFERENCES tblPatient(PatientID)
);
GO

-- Create tblAppointment
CREATE TABLE tblAppointment (
    AppointmentID INT IDENTITY(1,1) PRIMARY KEY,
    PatientID INT NOT NULL,
    DoctorID INT NOT NULL,
    AppointmentDate DATE NOT NULL,
    AppointmentTime TIME NOT NULL,
    Duration INT, -- Duration in minutes
    Priority NVARCHAR(20),
    Reason NVARCHAR(200),
    FollowUpDate DATE,
    FollowUpNotes NVARCHAR(500),
    CancellationReason NVARCHAR(200),
    Status NVARCHAR(20) DEFAULT 'Scheduled',
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (PatientID) REFERENCES tblPatient(PatientID),
    FOREIGN KEY (DoctorID) REFERENCES tblDoctor(DoctorID)
);
GO

-- Create tblMedicineCategory
CREATE TABLE tblMedicineCategory (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200),
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
    Category NVARCHAR(50),
    Description NVARCHAR(500),
    UnitPrice DECIMAL(10,2),
    StockQuantity INT DEFAULT 0,
    IsDeleted BIT DEFAULT 0
);
GO

-- Create tblDisease
CREATE TABLE tblDisease (
    DiseaseID INT IDENTITY(1,1) PRIMARY KEY,
    DiseaseName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    IsDeleted BIT DEFAULT 0
);
GO

-- Create tblPatientDisease
CREATE TABLE tblPatientDisease (
    PatientDiseaseID INT IDENTITY(1,1) PRIMARY KEY,
    PatientID INT NOT NULL,
    DiseaseID INT NOT NULL,
    DiagnosisDate DATE DEFAULT GETDATE(),
    Notes NVARCHAR(500),
    Status NVARCHAR(20) DEFAULT 'Active',
    FOREIGN KEY (PatientID) REFERENCES tblPatient(PatientID),
    FOREIGN KEY (DiseaseID) REFERENCES tblDisease(DiseaseID)
);
GO

-- Create tblInvoice (MODIFIED: Created before tblBilling, Simplified Schema)
CREATE TABLE tblInvoice (
    InvoiceID INT PRIMARY KEY IDENTITY(1,1),
    PatientID INT NOT NULL,
    InvoiceDate DATETIME DEFAULT GETDATE(),
    DueDate DATETIME,
    TotalAmount DECIMAL(18, 2) NOT NULL,
    PaymentStatus NVARCHAR(50) NOT NULL, -- e.g., 'Pending', 'Paid', 'Partially Paid'
    Notes NVARCHAR(MAX),
    CreatedBy INT, -- Assuming UserID
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (PatientID) REFERENCES tblPatient(PatientID),
    FOREIGN KEY (CreatedBy) REFERENCES tblUser(UserID)
);
GO

-- Create tblBilling
CREATE TABLE tblBilling (
    BillingID INT PRIMARY KEY IDENTITY(1,1),
    PatientID INT NOT NULL,
    InvoiceID INT, -- Nullable to allow bills to be created before an invoice
    BillingDate DATETIME DEFAULT GETDATE(),
    SubTotal DECIMAL(18,2) NOT NULL,
    DiscountPercentage DECIMAL(5,2) DEFAULT 0,
    TaxPercentage DECIMAL(5,2) DEFAULT 0,
    GrandTotal DECIMAL(18,2) NOT NULL,
    IsDeleted BIT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE(),
    LastModifiedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (PatientID) REFERENCES tblPatient(PatientID),
    FOREIGN KEY (InvoiceID) REFERENCES tblInvoice(InvoiceID) -- FK to tblInvoice
);
GO

-- Create tblBillingLineItem
CREATE TABLE tblBillingLineItem (
    LineItemID INT PRIMARY KEY IDENTITY(1,1),
    BillingID INT NOT NULL,
    Description NVARCHAR(255) NOT NULL,
    Quantity DECIMAL(10,2) NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    LineTotal DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (BillingID) REFERENCES tblBilling(BillingID)
);
GO

-- Create tblInvoiceLineItem
CREATE TABLE tblInvoiceLineItem (
    LineItemID INT PRIMARY KEY IDENTITY(1,1),
    InvoiceID INT NOT NULL,
    Description NVARCHAR(255) NOT NULL,
    Quantity DECIMAL(10,2) NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    LineTotal DECIMAL(18,2) NOT NULL,
    BillingID INT NULL, -- Can be linked to the original billing item
    FOREIGN KEY (InvoiceID) REFERENCES tblInvoice(InvoiceID),
    FOREIGN KEY (BillingID) REFERENCES tblBilling(BillingID)
);
GO

-- Create tblPayment (Modified to link to InvoiceID or BillingID)
CREATE TABLE tblPayment (
    PaymentID INT IDENTITY(1,1) PRIMARY KEY,
    BillingID INT NULL, -- Can be linked to a specific bill
    InvoiceID INT NULL, -- Can be linked to an invoice
    Amount DECIMAL(10,2) NOT NULL,
    PaymentDate DATETIME DEFAULT GETDATE(),
    PaymentMethod NVARCHAR(50) NOT NULL,
    TransactionID NVARCHAR(100),
    Status NVARCHAR(20) DEFAULT 'Completed',
    Notes NVARCHAR(200),
    FOREIGN KEY (BillingID) REFERENCES tblBilling(BillingID),
    FOREIGN KEY (InvoiceID) REFERENCES tblInvoice(InvoiceID),
    CONSTRAINT CHK_Payment_Link CHECK (BillingID IS NOT NULL OR InvoiceID IS NOT NULL)
);
GO

-- Create tblPrescription
CREATE TABLE tblPrescription (
    PrescriptionID INT IDENTITY(1,1) PRIMARY KEY,
    PatientID INT NOT NULL,
    DoctorID INT NOT NULL,
    PrescriptionDate DATE DEFAULT GETDATE(),
    Notes NVARCHAR(500),
    Status NVARCHAR(20) DEFAULT 'Active',
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (PatientID) REFERENCES tblPatient(PatientID),
    FOREIGN KEY (DoctorID) REFERENCES tblDoctor(DoctorID)
);
GO

-- Create tblPrescriptionDetail
CREATE TABLE tblPrescriptionDetail (
    PrescriptionDetailID INT IDENTITY(1,1) PRIMARY KEY,
    PrescriptionID INT NOT NULL,
    MedicineID INT NOT NULL,
    Dosage NVARCHAR(50) NOT NULL,
    Frequency NVARCHAR(50) NOT NULL,
    Duration NVARCHAR(50) NOT NULL,
    Instructions NVARCHAR(200),
    FOREIGN KEY (PrescriptionID) REFERENCES tblPrescription(PrescriptionID),
    FOREIGN KEY (MedicineID) REFERENCES tblMedicine(MedicineID)
);
GO

-- Create tblRoomAllocation
CREATE TABLE tblRoomAllocation (
    AllocationID INT IDENTITY(1,1) PRIMARY KEY,
    RoomID INT NOT NULL,
    PatientID INT NOT NULL,
    CheckInDate DATETIME NOT NULL,
    CheckOutDate DATETIME,
    Status NVARCHAR(20) DEFAULT 'Active',
    Notes NVARCHAR(200),
    FOREIGN KEY (RoomID) REFERENCES tblRoom(RoomID),
    FOREIGN KEY (PatientID) REFERENCES tblPatient(PatientID)
);
GO

-- Create tblMedicineStock
CREATE TABLE tblMedicineStock (
    StockID INT IDENTITY(1,1) PRIMARY KEY,
    MedicineID INT NOT NULL,
    BatchNumber NVARCHAR(50) NOT NULL,
    Quantity INT NOT NULL,
    ManufacturingDate DATE,
    ExpiryDate DATE,
    PurchaseDate DATE,
    PurchasePrice DECIMAL(10,2),
    SupplierID INT,
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (MedicineID) REFERENCES tblMedicine(MedicineID),
    FOREIGN KEY (SupplierID) REFERENCES tblSupplier(SupplierID)
);
GO

-- Create tblMedicineUsage
CREATE TABLE tblMedicineUsage (
    UsageID INT IDENTITY(1,1) PRIMARY KEY,
    MedicineID INT NOT NULL,
    PatientID INT NOT NULL,
    PrescriptionID INT,
    Quantity INT NOT NULL,
    UsageDate DATE DEFAULT GETDATE(),
    Notes NVARCHAR(200),
    FOREIGN KEY (MedicineID) REFERENCES tblMedicine(MedicineID),
    FOREIGN KEY (PatientID) REFERENCES tblPatient(PatientID),
    FOREIGN KEY (PrescriptionID) REFERENCES tblPrescription(PrescriptionID)
);
GO

-- Create tblPatientVisit
CREATE TABLE tblPatientVisit (
    VisitID INT IDENTITY(1,1) PRIMARY KEY,
    PatientID INT NOT NULL,
    DoctorID INT NOT NULL,
    VisitDate DATE DEFAULT GETDATE(),
    VisitType NVARCHAR(50),
    Symptoms NVARCHAR(MAX),
    Diagnosis NVARCHAR(MAX),
    Treatment NVARCHAR(MAX),
    FollowUpDate DATE,
    Status NVARCHAR(20) DEFAULT 'Active',
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (PatientID) REFERENCES tblPatient(PatientID),
    FOREIGN KEY (DoctorID) REFERENCES tblDoctor(DoctorID)
);
GO

-- Create tblPatientDocument
CREATE TABLE tblPatientDocument (
    DocumentID INT IDENTITY(1,1) PRIMARY KEY,
    PatientID INT NOT NULL,
    DocumentType NVARCHAR(50) NOT NULL,
    DocumentName NVARCHAR(100) NOT NULL,
    DocumentData VARBINARY(MAX),
    UploadDate DATETIME DEFAULT GETDATE(),
    Description NVARCHAR(200),
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (PatientID) REFERENCES tblPatient(PatientID)
);
GO

-- Create tblDoctorSchedule
CREATE TABLE tblDoctorSchedule (
    ScheduleID INT IDENTITY(1,1) PRIMARY KEY,
    DoctorID INT NOT NULL,
    DayOfWeek INT NOT NULL, -- 1 = Sunday, 7 = Saturday
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    IsAvailable BIT DEFAULT 1,
    FOREIGN KEY (DoctorID) REFERENCES tblDoctor(DoctorID)
);
GO

-- Create tblRoomMaintenance
CREATE TABLE tblRoomMaintenance (
    MaintenanceID INT IDENTITY(1,1) PRIMARY KEY,
    RoomID INT NOT NULL,
    MaintenanceType NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200),
    StartDate DATE NOT NULL,
    EndDate DATE,
    Status NVARCHAR(20) DEFAULT 'Scheduled',
    Cost DECIMAL(10,2),
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (RoomID) REFERENCES tblRoom(RoomID)
);
GO

-- Create tblRoomType table
CREATE TABLE tblRoomType (
    RoomTypeID INT IDENTITY(1,1) PRIMARY KEY,
    RoomTypeName NVARCHAR(100) NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

-- Insert default room types
INSERT INTO tblRoomType (RoomTypeName, IsDeleted) VALUES
('Standard', 0),
('Deluxe', 0),
('ICU', 0),
('Suite', 0);

-- Insert sample data
INSERT INTO tblUser (Username, Password, FullName, Role)
VALUES ('admin', 'admin123', 'System Administrator', 'Admin');

INSERT INTO tblUser (Username, Password, FullName, Role)
VALUES ('patient', 'patient123', 'John Doe', 'Patient');

INSERT INTO tblUser (Username, Password, FullName, Role)
VALUES ('doctor', 'doctor123', 'Dr. Jane Smith', 'Doctor');
GO

INSERT INTO tblDoctorType (SpecializationName, Description) 
VALUES ('Cardiology', 'Heart and blood vessel disorders'),
       ('Neurology', 'Nervous system disorders'),
       ('Pediatrics', 'Child health'),
       ('Orthopedics', 'Bone and joint disorders'),
       ('Dermatology', 'Skin disorders');
GO

INSERT INTO tblDoctor (FirstName, LastName, SpecializationID, ContactNumber, Email)
VALUES ('Jane', 'Smith', 1, '1234567890', 'jane.smith@hospital.com');
GO

INSERT INTO tblPatient (FirstName, LastName, DateOfBirth, Gender, UserID)
VALUES ('John', 'Doe', '1990-01-01', 'Male', 2);
GO

INSERT INTO tblRoom (RoomNumber, RoomType, Floor, Capacity, RatePerDay, Status)
VALUES ('101', 'General', 1, 2, 100.00, 'Available'),
       ('102', 'Private', 1, 1, 150.00, 'Available'),
       ('103', 'ICU', 1, 1, 200.00, 'Available');
GO

INSERT INTO tblDisease (DiseaseName, Description)
VALUES ('Hypertension', 'High blood pressure'),
       ('Diabetes', 'High blood sugar levels'),
       ('Asthma', 'Respiratory condition');
GO

INSERT INTO tblSupplier (SupplierName, ContactPerson, Email, Phone, Address)
VALUES ('MedSupply Co.', 'John Supplier', 'contact@medsupply.com', '9876543210', '123 Main St, Anytown, USA');
GO

INSERT INTO tblMedicineCategory (CategoryName, Description)
VALUES ('Antibiotics', 'Medicines that inhibit the growth of or destroy microorganisms'),
       ('Analgesics', 'Pain relieving medicines'),
       ('Antipyretics', 'Fever reducing medicines'),
       ('Antacids', 'Medicines that neutralize stomach acid'),
       ('Antihistamines', 'Medicines that treat allergy symptoms');
GO

INSERT INTO tblMedicine (MedicineName, Category, Description, UnitPrice)
VALUES ('Paracetamol', 'Analgesics', 'Pain reliever and fever reducer', 5.99);
GO

-- Create sequence for invoice numbers
CREATE SEQUENCE dbo.InvoiceNumberSequence
    START WITH 1
    INCREMENT BY 1;
GO

-- Create stored procedures
CREATE PROCEDURE sp_CreatePatientUser
    @Username NVARCHAR(50),
    @Password NVARCHAR(100),
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @DateOfBirth DATE,
    @Gender NVARCHAR(10),
    @Email NVARCHAR(100),
    @ContactNumber NVARCHAR(20),
    @Address NVARCHAR(200)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Insert into tblUser
        INSERT INTO tblUser (Username, Password, FullName, Email, Role)
        VALUES (@Username, @Password, @FirstName + ' ' + @LastName, @Email, 'Patient');
        
        DECLARE @UserID INT = SCOPE_IDENTITY();
        
        -- Insert into tblPatient
        INSERT INTO tblPatient (FirstName, LastName, DateOfBirth, Gender, Email, ContactNumber, Address, UserID)
        VALUES (@FirstName, @LastName, @DateOfBirth, @Gender, @Email, @ContactNumber, @Address, @UserID);
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

CREATE PROCEDURE sp_ScheduleAppointment
    @PatientID INT,
    @DoctorID INT,
    @AppointmentDate DATE,
    @AppointmentTime TIME,
    @Reason NVARCHAR(200)
AS
BEGIN
    BEGIN TRY
        -- Check if the time slot is available
        IF NOT EXISTS (
            SELECT 1 FROM tblAppointment 
            WHERE DoctorID = @DoctorID 
            AND AppointmentDate = @AppointmentDate 
            AND AppointmentTime = @AppointmentTime
            AND Status = 'Scheduled'
        )
        BEGIN
            INSERT INTO tblAppointment (PatientID, DoctorID, AppointmentDate, AppointmentTime, Reason)
            VALUES (@PatientID, @DoctorID, @AppointmentDate, @AppointmentTime, @Reason);
        END
        ELSE
        BEGIN
            RAISERROR('The selected time slot is not available.', 16, 1);
        END
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

CREATE PROCEDURE sp_AllocateRoom
    @RoomID INT,
    @PatientID INT,
    @CheckInDate DATETIME,
    @Notes NVARCHAR(200) = NULL
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Check if room is available
        IF EXISTS (
            SELECT 1 FROM tblRoom 
            WHERE RoomID = @RoomID 
            AND Status = 'Available'
        )
        BEGIN
            -- Update room status
            UPDATE tblRoom 
            SET Status = 'Occupied', 
                PatientID = @PatientID 
            WHERE RoomID = @RoomID;
            
            -- Create room allocation record
            INSERT INTO tblRoomAllocation (RoomID, PatientID, CheckInDate, Notes)
            VALUES (@RoomID, @PatientID, @CheckInDate, @Notes);
            
            COMMIT TRANSACTION;
        END
        ELSE
        BEGIN
            RAISERROR('The selected room is not available.', 16, 1);
        END
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- sp_GenerateInvoice (Functionality now handled by C# application)
-- CREATE PROCEDURE sp_GenerateInvoice
--     @PatientID INT,
--     @ConsultationFee DECIMAL(10,2),
--     @RoomCharges DECIMAL(10,2),
--     @MedicineCharges DECIMAL(10,2),
--     @OtherCharges DECIMAL(10,2)
-- AS
-- BEGIN
--     BEGIN TRY
--         BEGIN TRANSACTION;
        
--         DECLARE @InvoiceNumber NVARCHAR(50) = 'INV-' + CAST(YEAR(GETDATE()) AS VARCHAR) + '-' + CAST(NEXT VALUE FOR dbo.InvoiceNumberSequence AS VARCHAR);
--         DECLARE @TotalAmount DECIMAL(10,2) = @ConsultationFee + @RoomCharges + @MedicineCharges + @OtherCharges;
        
--         -- Create billing record
--         INSERT INTO tblBilling (PatientID, InvoiceNumber, ConsultationFee, RoomCharges, MedicineCharges, OtherCharges, TotalAmount)
--         VALUES (@PatientID, @InvoiceNumber, @ConsultationFee, @RoomCharges, @MedicineCharges, @OtherCharges, @TotalAmount);
        
--         DECLARE @BillingID INT = SCOPE_IDENTITY();
        
--         -- Create invoice record
--         INSERT INTO tblInvoice (BillingID, PatientID, InvoiceNumber, DueDate)
--         VALUES (@BillingID, @PatientID, @InvoiceNumber, DATEADD(DAY, 30, GETDATE()));
        
--         COMMIT TRANSACTION;
--     END TRY
--     BEGIN CATCH
--         ROLLBACK TRANSACTION;
--         THROW;
--     END CATCH
-- END;
-- GO

CREATE PROCEDURE sp_UpdateMedicineStock
    @MedicineID INT,
    @Quantity INT,
    @BatchNumber NVARCHAR(50),
    @ManufacturingDate DATE,
    @ExpiryDate DATE,
    @PurchasePrice DECIMAL(10,2),
    @SupplierID INT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Update medicine quantity
        UPDATE tblMedicine 
        SET StockQuantity = StockQuantity + @Quantity
        WHERE MedicineID = @MedicineID;
        
        -- Add stock record
        INSERT INTO tblMedicineStock (MedicineID, BatchNumber, Quantity, ManufacturingDate, ExpiryDate, PurchasePrice, SupplierID)
        VALUES (@MedicineID, @BatchNumber, @Quantity, @ManufacturingDate, @ExpiryDate, @PurchasePrice, @SupplierID);
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

CREATE PROCEDURE sp_CreatePatientVisit
    @PatientID INT,
    @DoctorID INT,
    @VisitType NVARCHAR(50),
    @Symptoms NVARCHAR(MAX),
    @Diagnosis NVARCHAR(MAX),
    @Treatment NVARCHAR(MAX),
    @FollowUpDate DATE = NULL
AS
BEGIN
    BEGIN TRY
        INSERT INTO tblPatientVisit (PatientID, DoctorID, VisitType, Symptoms, Diagnosis, Treatment, FollowUpDate)
        VALUES (@PatientID, @DoctorID, @VisitType, @Symptoms, @Diagnosis, @Treatment, @FollowUpDate);
        
        -- Update patient's last visit date
        UPDATE tblPatient
        SET LastVisitDate = GETDATE()
        WHERE PatientID = @PatientID;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

CREATE PROCEDURE sp_ScheduleRoomMaintenance
    @RoomID INT,
    @MaintenanceType NVARCHAR(50),
    @Description NVARCHAR(200),
    @StartDate DATE,
    @EndDate DATE,
    @Cost DECIMAL(10,2) = NULL
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Update room status to 'Under Maintenance'
        UPDATE tblRoom
        SET Status = 'Under Maintenance'
        WHERE RoomID = @RoomID;
        
        -- Create maintenance record
        INSERT INTO tblRoomMaintenance (RoomID, MaintenanceType, Description, StartDate, EndDate, Cost)
        VALUES (@RoomID, @MaintenanceType, @Description, @StartDate, @EndDate, @Cost);
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO