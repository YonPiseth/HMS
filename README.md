# Hospital Management System (HMS)

## Overview
The Hospital Management System (HMS) is a desktop application designed to manage hospital operations, including patient records, doctor appointments, room management, and billing. It provides a comprehensive solution for healthcare facilities to streamline their administrative and clinical processes.

## Requirements

### System Requirements
- Windows 10 or later
- .NET 6.0 Runtime or later
- SQL Server 2019 or later (Express edition is supported)
- Minimum 4GB RAM
- 1GB free disk space

### Development Requirements
- Visual Studio 2022 or later
- .NET 6.0 SDK
- SQL Server Management Studio (SSMS) or Azure Data Studio
- Git (for version control)

### NuGet Packages
- Microsoft.Data.SqlClient (6.0.2)
- System.Data.SqlClient (4.8.5)
- System.Configuration.ConfigurationManager (6.0.0)

## Database Setup

### 1. SQL Server Installation
- Download and install SQL Server (Express edition is free and sufficient for development)
- During installation, ensure the following components are selected:
  - Database Engine Services
  - SQL Server Management Studio (SSMS)
  - Client Tools Connectivity

### 2. Database Creation
1. Open SQL Server Management Studio (SSMS)
2. Connect to your SQL Server instance
3. Open the `create_hms_db.sql` file located in the `hms` directory
4. Execute the script to:
   - Create the HMS database
   - Create all necessary tables
   - Set up stored procedures
   - Configure initial data

### 3. Database Configuration
1. Open `App.config` in the project
2. Update the connection string with your SQL Server details:
   ```xml
   <connectionStrings>
     <add name="HMSConnection" 
          connectionString="Server=YOUR_SERVER;Database=HMS;Trusted_Connection=True;TrustServerCertificate=True;"
          providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

## Building and Running

### 1. Clone the Repository
```bash
git clone https://github.com/yourusername/HMS.git
cd HMS
```

### 2. Build the Project
```bash
cd hms
dotnet build
```

### 3. Run the Application
```bash
dotnet run
```

## Features

### Patient Management
- Add, edit, and delete patient records
- Upload and manage patient documents
- Track patient medical history
- Manage patient insurance information

### Doctor Management
- Maintain doctor profiles with specialization
- Track doctor schedules and availability
- Manage doctor qualifications and experience
- Upload and manage doctor profile photos

### Appointment Scheduling
- Schedule patient appointments
- Set appointment priorities
- Manage follow-up appointments
- Send appointment reminders

### Room Management
- Track room availability
- Manage room types and rates
- Schedule room maintenance
- Handle room allocations

### Billing and Invoicing
- Generate patient bills
- Track payments
- Manage insurance claims
- Generate financial reports

### Medicine Management
- Track medicine inventory
- Manage medicine categories
- Record medicine usage
- Generate prescription reports

## Troubleshooting

### Common Issues

1. **Database Connection Error**
   - Verify SQL Server is running
   - Check connection string in App.config
   - Ensure Windows Authentication is enabled

2. **Image Upload Issues**
   - Ensure sufficient disk space
   - Check file permissions
   - Verify image format (PNG/JPG supported)

3. **Application Crashes**
   - Check Windows Event Viewer for errors
   - Verify .NET Runtime is installed
   - Ensure all NuGet packages are restored

### Support
For additional support:
1. Check the [Issues](https://github.com/yourusername/HMS/issues) section
2. Create a new issue with detailed error information
3. Contact the development team

## License
This project is licensed under the MIT License. See the LICENSE file for details.

## Contributing
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request 
