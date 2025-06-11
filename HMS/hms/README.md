# Hospital Management System (HMS)

## Overview
The Hospital Management System (HMS) is a desktop application designed to manage hospital operations, including patient records, doctor appointments, room management, and billing.

## Prerequisites
- SQL Server (Express or higher)
- .NET Core SDK (latest version)
- Visual Studio or any compatible IDE

## Setup Instructions

### 1. Create the Database
- Open SQL Server Management Studio (SSMS) or any compatible SQL tool.
- Connect to your SQL Server instance.
- Open the `create_hms_db.sql` file located in the `hms` directory.
- Execute the script to create the `HMS` database and its tables.

### 2. Build and Run the Application
- Open a terminal or command prompt.
- Navigate to the `hms` directory:
  ```bash
  cd path/to/hms
  ```
- Build the project:
  ```bash
  dotnet build
  ```
- Run the application:
  ```bash
  dotnet run
  ```

## Features
- **Patient Management**: Add, edit, and delete patient records.
- **Doctor Management**: Manage doctor profiles, including specialization.
- **Appointment Scheduling**: Schedule and manage patient appointments.
- **Room Management**: Manage hospital rooms, including room types and availability.
- **Billing and Invoicing**: Generate and manage patient bills and invoices.

## Troubleshooting
- Ensure that the SQL Server is running and accessible.
- Check the connection string in the application to ensure it points to the correct SQL Server instance.
- If you encounter any issues, please refer to the error messages or contact support.

## License
This project is licensed under the MIT License. See the LICENSE file for details. 