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
- ✅ **Data Validation**: Input validation and error handling
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

This project is licensed under **Group 2** @RUPP Year4 Gen26 A1

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
