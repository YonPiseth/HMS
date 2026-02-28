# Hospital Management System (HMS)

Note

| Username | Password | Role   |
|----------|----------|--------|
| `admin`  | `admin`  | Admin  |
| `Admin`  | `admin123` | Admin  |

A comprehensive Windows Forms application developed in C# for managing hospital operations including patient records, doctor information, appointments, billing, medications, and more.

![.NET Version](https://img.shields.io/badge/.NET-8.0-blue)
![C#](https://img.shields.io/badge/C%23-Latest-green)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-red)

---

## 📋 Table of Contents

- [Project Description](#project-description)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Prerequisites](#prerequisites)
- [Setup Instructions](#setup-instructions)
  - [For ZIP Distribution](#for-zip-distribution)
  - [For GitHub Clone](#for-github-clone)
- [Database Setup](#database-setup)
- [Configuration](#configuration)
- [Running the Application](#running-the-application)
- [Default Login Credentials](#default-login-credentials)
- [Project Structure](#project-structure)
- [Forms Description](#forms-description)
- [Screenshots](#screenshots)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

---

## 📝 Project Description

Hospital Management System (HMS) is a desktop application designed to streamline hospital operations. It provides a user-friendly interface for managing:

- **Patient Records**: Complete patient information, medical history, and room assignments
- **Doctor Management**: Doctor profiles, specializations, and schedules
- **Appointment Scheduling**: Book and manage patient appointments
- **Billing & Invoicing**: Generate bills and invoices for patient services
- **Medicine Management**: Track medicine inventory and stock
- **Disease Management**: Maintain disease records and descriptions
- **Room Management**: Manage hospital rooms, availability, and assignments
- **Supplier Management**: Track suppliers and their information

The application features role-based access control (Admin, Doctor, Patient) and a modern, intuitive user interface.

---

## ✨ Features

### Core Features
- ✅ **User Authentication**: Secure login system with role-based access (Admin, Doctor, Patient)
- ✅ **Patient Management**: Add, update, delete, and search patient records with profile photos
- ✅ **Doctor Management**: Manage doctor profiles, specializations, and experience
- ✅ **Appointment Scheduling**: Schedule, update, and track patient appointments
- ✅ **Billing System**: Create bills with line items, discounts, and tax calculations
- ✅ **Invoice Generation**: Generate and view invoices with receipt printing
- ✅ **Medicine Management**: Track medicine inventory, categories, and pricing
- ✅ **Disease Management**: Maintain disease database with descriptions
- ✅ **Room Management**: Manage room availability, types, and assignments
- ✅ **Supplier Management**: Track supplier information and contacts

### Technical Features
- ✅ **Centralized Database Connection**: Using `DatabaseHelper` class for consistent database access
- ✅ **Modern UI**: Clean, professional interface with consistent styling
- ✅ **Search Functionality**: Quick search across all modules
- ✅ **Data Validation**: Comprehensive input validation and centralized error handling via message boxes
- ✅ **Soft Delete**: Records are marked as deleted rather than permanently removed
- ✅ **Image Support**: Profile photo upload and display for patients and doctors

---

## 🛠 Technologies Used

- **Framework**: .NET 8.0-windows
- **Language**: C# (Latest)
- **Database**: SQL Server (via `Microsoft.Data.SqlClient`)
- **UI Framework**: Windows Forms
- **NuGet Packages**:
  - `Microsoft.Data.SqlClient` (v6.1.2)
  - `System.Data.SqlClient` (v4.9.0)
  - `System.Configuration.ConfigurationManager` (v9.0.10)

---

## 📦 Prerequisites

Before you begin, ensure you have the following installed:

1. **Visual Studio 2022** (or later) with:
   - .NET 8.0 SDK
   - Windows Forms development workload
   - SQL Server Data Tools (optional, for database management)

2. **SQL Server** (one of the following):
   - SQL Server 2019 or later
   - SQL Server Express (free)
   - SQL Server LocalDB (included with Visual Studio)

3. **SQL Server Management Studio (SSMS)** or Azure Data Studio (for database setup)

---

## 🚀 Setup Instructions

### For ZIP Distribution

1. **Extract the Project**
   - Extract the ZIP file to your desired location (e.g., `D:\Projects\HMS`)
   - Ensure all files are extracted properly

2. **Open in Visual Studio**
   - Navigate to the extracted folder
   - Double-click `HMS.sln` to open the solution in Visual Studio
   - Wait for Visual Studio to restore NuGet packages (this may take a few minutes)

3. **Continue with [Database Setup](#database-setup)**

### For GitHub Clone

1. **Clone the Repository**
   ```bash
   git clone https://github.com/YonPiseth/HMS.git
   cd HMS
   ```

2. **Open in Visual Studio**
   - Double-click `HMS.sln` to open the solution
   - Visual Studio will automatically restore NuGet packages

3. **Continue with [Database Setup](#database-setup)**

---

## 💾 Database Setup

1. **Configure Database Connection** ⚠️ **First**
   - Update the connection string in `HMS/App.config` (see [Configuration](#-configuration) section)
   - The default uses `.\SQLEXPRESS` - change if your SQL Server instance is different

2. **Run the Database Script**
   - Open SQL Server Management Studio (SSMS)
   - Connect to your SQL Server instance
   - Open and execute `HMS/create_hms_db.sql` (Press `F5`)
   - Wait for the script to complete successfully

3. **Verify Database Creation**
   - Check that the `HMS` database exists with all tables created
   - Verify initial data is populated (user accounts, etc.)

---

## ⚙️ Configuration

### Database Connection String

**⚠️ REQUIRED: Configure your SQL Server connection before running the application.**

Update the connection string in `HMS/App.config` to match your SQL Server instance:

```xml
<connectionStrings>
    <add name="HMSConnection"
         connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True;TrustServerCertificate=True;"
         providerName="Microsoft.Data.SqlClient" />
</connectionStrings>
```

**Change the `Data Source` value based on your SQL Server setup:**
- `.\SQLEXPRESS` - SQL Server Express (default)
- `(localdb)\MSSQLLocalDB` - LocalDB
- `localhost` - Default SQL Server instance
- `ServerName\InstanceName` - Named instance
- `IPAddress,Port` - Remote server (e.g., `192.168.1.100,1433`)

**For SQL Authentication** (instead of Windows Authentication), change to:
```xml
connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=HMS;User ID=YourUsername;Password=YourPassword;TrustServerCertificate=True;"
```

After changing the connection string, rebuild the application.

### Alternative: Update DatabaseHelper.cs

If `App.config` doesn't work, you can directly update `HMS/DatabaseHelper.cs`:

```csharp
public static SqlConnection GetConnection()
{
    return new SqlConnection("Data Source=YOUR_SERVER;Initial Catalog=HMS;Integrated Security=True;TrustServerCertificate=True;");
}
```

---

## ▶️ Running the Application

1. **Build the Solution**
   - In Visual Studio, right-click on the `HMS` project
   - Select **Build** or **Rebuild**
   - Wait for the build to complete (check for errors in the Output window)

2. **Run the Application**
   - Press `F5` or click the **Start** button
   - The application will launch with a login form

3. **Login**
   - Use the default credentials (see below)
   - After successful login, you'll see the main dashboard

---

## 🔐 Default Login Credentials

The following credentials are created by the database script:

| Username | Password | Role   |
|----------|----------|--------|
| `admin`  | `admin`  | Admin  |
| `Admin`  | `admin123` | Admin  |

**Note**: You can create additional users through the database or application (if user management is implemented).

---

## 📁 Project Structure

```
HMS/
│
├── HMS/                          # Main project folder
│   ├── Controls/                 # User controls
│   │   ├── PatientControl.cs
│   │   ├── DoctorControl.cs
│   │   ├── AppointmentControl.cs
│   │   ├── BillingControl.cs
│   │   ├── InvoiceControl.cs
│   │   ├── MedicineControl.cs
│   │   ├── DiseaseControl.cs
│   │   ├── RoomControl.cs
│   │   └── SupplierControl.cs
│   │
│   ├── Forms/                    # Windows Forms
│   │   ├── LoginForm.cs
│   │   ├── MainForm.cs
│   │   ├── SplashForm.cs
│   │   ├── PatientForm.cs
│   │   ├── DoctorForm.cs
│   │   ├── AppointmentForm.cs
│   │   ├── BillingForm.cs
│   │   ├── InvoiceForm.cs
│   │   └── ...
│   │
│   ├── Helpers/                  # Helper classes
│   │   ├── DatabaseHelper.cs    # Centralized database connection
│   │   └── UIHelper.cs          # UI styling utilities
│   │
│   ├── Image/                    # Application images
│   │   ├── Loading Image.png
│   │   ├── patient.png
│   │   ├── doctor.png
│   │   └── ...
│   │
│   ├── Program.cs                # Application entry point
│   ├── App.config               # Configuration file
│   ├── create_hms_db.sql        # Database creation script
│   └── HMS.csproj              # Project file
│
├── HMS.sln                       # Solution file
└── README.md                     # This file
```

---

## 📋 Forms Description

### SplashForm
The SplashForm is the initial loading screen displayed when the application starts. It features an animated progress bar, application logo, and version information to provide visual feedback during application initialization. The form automatically closes once the loading process completes, transitioning to the login screen.

### LoginForm
The LoginForm handles user authentication by validating username and password credentials against the database. It supports role-based access control (Admin, Doctor, Patient) and updates the user's last login timestamp upon successful authentication. The form provides a clean, modern interface with validation to ensure both fields are entered before submission.

### MainForm
The MainForm serves as the central dashboard and navigation hub for the entire application after successful login. It displays a navigation panel with buttons for accessing different modules (Patients, Doctors, Appointments, Billing, etc.) and dynamically adjusts the interface based on the user's role. The form uses a content panel system to display different user controls and provides role-specific views, such as hiding navigation for Patient users.

### PatientForm
The PatientForm allows users to add or edit comprehensive patient information including personal details, contact information, medical data, and profile photos. It supports room assignment functionality, linking patients to specific hospital rooms, and includes validation to ensure all required fields are properly filled. The form can operate in both "new patient" and "edit existing patient" modes, automatically handling database insertions or updates accordingly.

### DoctorForm
The DoctorForm enables management of doctor profiles with fields for personal information, specialization, qualifications, experience, and availability status. It includes photo upload capability and integrates with the doctor specialization lookup table to maintain data consistency. The form validates all required fields and supports both creating new doctor records and updating existing ones.

### AppointmentForm
The AppointmentForm facilitates scheduling patient appointments by allowing selection of a patient, doctor, date, time, and appointment reason. It includes status tracking (Scheduled, Completed, Cancelled, No Show) and loads available patients and doctors from the database into dropdown lists. The form ensures all mandatory fields are selected before saving an appointment record.

### BillingForm
The BillingForm provides a comprehensive interface for creating and managing patient bills with line items, quantities, and pricing. It automatically calculates subtotals, applies discounts and tax rates, and computes the grand total in real-time as items are added or modified. The form supports editing existing bills and includes validation to ensure at least one line item exists before saving.

### InvoiceForm
The InvoiceForm allows users to select multiple unpaid bills for a patient and combine them into a single invoice. It displays a list of available bills with checkboxes for selection and shows a preview of selected bill line items in a separate grid. The form generates a new invoice record, links selected bills to the invoice, and opens the receipt form upon successful creation.

### InvoiceReceiptForm
The InvoiceReceiptForm displays a formatted receipt view of a generated invoice with all line items, totals, and payment information. It provides a print-friendly layout showing patient details, invoice number, date, and all associated billing items. The form serves as both a display and printing interface for invoice documentation.

### PatientInvoiceHistoryForm
The PatientInvoiceHistoryForm shows a comprehensive list of all invoices associated with a specific patient. It displays invoice details including dates, amounts, payment status, and allows users to view or print individual invoice receipts. The form provides historical tracking of a patient's billing and payment records.

### MedicineForm
The MedicineForm manages medicine inventory by allowing entry of medicine details including name, category, supplier, description, dosage, side effects, pricing, and stock quantity. It integrates with the supplier lookup table and includes validation to ensure all required fields are completed. The form supports both adding new medicines and updating existing inventory records.

### DiseaseForm
The DiseaseForm provides a simple interface for maintaining the disease database with disease names and descriptions. It includes validation to ensure both the disease name and description fields are filled before saving. The form helps maintain a comprehensive catalog of diseases for reference in patient records and medical documentation.

### RoomForm
The RoomForm manages hospital room information including room number, type, floor, capacity, daily rate, and availability status. It integrates with the room type lookup table and includes validation for required fields such as room number, type, and status. The form supports creating new room records and updating existing room information, including capacity and pricing details.

### SupplierForm
The SupplierForm manages supplier information including company name, contact person, email, phone, and address details. It includes email and phone number format validation to ensure data quality and requires all mandatory fields to be completed. The form supports both adding new suppliers and updating existing supplier records for medicine procurement management.

### PatientRegistrationForm
The PatientRegistrationForm provides a detailed patient registration interface with comprehensive validation including email format, phone number format, and required field checks. It includes tooltips for user guidance and uses an error provider to display validation messages inline. The form supports both new patient registration and editing existing patient records with proper data validation before database operations.

---


## 🔧 Troubleshooting

### Common Issues

#### 1. **"Cannot connect to database" Error**
- **Solution**: Verify SQL Server is running and the connection string in `App.config` is correct
- Check SQL Server services: `Services.msc` → SQL Server (SQLEXPRESS)
- Test connection in SSMS with the same connection string

#### 2. **"Image not found" Error**
- **Solution**: Ensure the `Image` folder is copied to the output directory
- Check `HMS.csproj` includes: `<Content Include="Image\**">`
- Rebuild the solution

#### 3. **Build Errors**
- **Solution**: Restore NuGet packages: Right-click solution → Restore NuGet Packages
- Clean and rebuild: Build → Clean Solution, then Build → Rebuild Solution
- Check .NET 8.0 SDK is installed: `dotnet --version`

#### 4. **Database Script Fails**
- **Solution**: Ensure you have permissions to create databases
- Check for existing `HMS` database and drop it first
- Run the script section by section if needed

#### 5. **Login Fails**
- **Solution**: Verify user accounts exist in `tblUser` table
- Check `IsActive = 1` for the user account
- Verify password is correct (case-sensitive)

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. **Fork the repository**
2. **Create a feature branch** (`git checkout -b feature/AmazingFeature`)
3. **Commit your changes** (`git commit -m 'Add some AmazingFeature'`)
4. **Push to the branch** (`git push origin feature/AmazingFeature`)
5. **Open a Pull Request**

### Contribution Guidelines
- Follow C# coding conventions
- Add comments for complex logic
- Test your changes thoroughly
- Update documentation if needed

---

## 📄 License

This project is licensed under **Group 2** - see the LICENSE file for details.

---

## 📧 Contact

**Project Maintainer**: YonPiseth

- **Email**: Pisethyon987@gmail.com
- **GitHub**: [@YonPiseth](https://github.com/YonPiseth)

For questions, suggestions, or issues, please open an issue on GitHub or contact the maintainer.

---

## 🙏 Acknowledgments

- Thanks to all contributors who have helped improve this project
- Special thanks to the .NET and SQL Server communities for excellent documentation

---

**Made with ❤️ by Group 2**
