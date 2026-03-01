# Hospital Management System (HMS)

**ចំណាំ**

| Username | Password | Role   |
|----------|----------|--------|
| `admin`  | `admin`  | Admin  |
| `Admin`  | `admin123` | Admin  |

Application Windows Forms ពេញលេញដែលបានបង្កើតជាមួយ C# សម្រាប់គ្រប់គ្រង operations នៃមន្ទីរពេទ្យ រួមមាន patient records, doctor information, appointments, billing, medications, និងច្រើនទៀត។

![.NET Version](https://img.shields.io/badge/.NET-8.0-blue)
![C#](https://img.shields.io/badge/C%23-Latest-green)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-red)

---

## 📋 តារាងមាតិកា

- [ការពន្យល់អំពី Project](#ការពន្យល់អំពី-project)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Prerequisites](#prerequisites)
- [ការណែនាំ Setup](#ការណែនាំ-setup)
  - [សម្រាប់ ZIP Distribution](#សម្រាប់-zip-distribution)
  - [សម្រាប់ GitHub Clone](#សម្រាប់-github-clone)
- [Database Setup](#database-setup)
- [Configuration](#configuration)
- [ការដំណើរការ Application](#ការដំណើរការ-application)
- [Default Login Credentials](#default-login-credentials)
- [Project Structure](#project-structure)
- [ស្ថាបត្យកម្មរចនា និង Patterns](#ស្ថាបត្យកម្មរចនា-និង-patterns)
- [ការពន្យល់អំពី Forms](#ការពន្យល់អំពី-forms)
- [Screenshots](#screenshots)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)
- [សេចក្តីសន្និដ្ឋាន](#សេចក្តីសន្និដ្ឋាន)

---

## 📝 ការពន្យល់អំពី Project

Hospital Management System (HMS) គឺជា desktop application ដែលបានរចនាឡើងដើម្បី streamline hospital operations។ វាផ្តល់ interface ដែលងាយប្រើសម្រាប់គ្រប់គ្រង:

- **Patient Records**: Patient information ពេញលេញ, medical history, និង room assignments
- **Doctor Management**: Doctor profiles, specializations, និង schedules
- **Appointment Scheduling**: Book និង manage patient appointments
- **Billing & Invoicing**: Generate bills និង invoices សម្រាប់ patient services
- **Medicine Management**: Track medicine inventory និង stock
- **Disease Management**: Maintain disease records និង descriptions
- **Room Management**: Manage hospital rooms, availability, និង assignments
- **Supplier Management**: Track suppliers និង information របស់ពួកគេ

Application នេះមាន role-based access control (Admin, Doctor, Patient) និង user interface ទំនើប ងាយស្រួល។

---

## ✨ Features

### Core Features
- ✅ **User Authentication**: Login system មានសុវត្ថិភាពជាមួយ role-based access (Admin, Doctor, Patient)
- ✅ **Patient Management**: Add, update, delete, និង search patient records ជាមួយ profile photos
- ✅ **Doctor Management**: Manage doctor profiles, specializations, និង experience
- ✅ **Appointment Scheduling**: Schedule, update, និង track patient appointments
- ✅ **Billing System**: Create bills ជាមួយ line items, discounts, និង tax calculations
- ✅ **Invoice Generation**: Generate និង view invoices ជាមួយ receipt printing
- ✅ **Medicine Management**: Track medicine inventory, categories, និង pricing
- ✅ **Disease Management**: Maintain disease database ជាមួយ descriptions
- ✅ **Room Management**: Manage room availability, types, និង assignments
- ✅ **Supplier Management**: Track supplier information និង contacts

### Technical Features
- ✅ **Centralized Database Connection**: ប្រើ `DatabaseHelper` class សម្រាប់ database access ដូចគ្នា
- ✅ **Modern UI**: Interface ស្អាត, professional ជាមួយ styling ដូចគ្នា
- ✅ **Search Functionality**: Search លឿននៅទូទាំង modules ទាំងអស់
- ✅ **Data Validation**: Input validation និង error handling តាមរយៈ message boxes
- ✅ **Soft Delete**: Records ត្រូវបាន mark ជា deleted ជំនួសឱ្យ remove ជាអចិន្ត្រៃយ៍
- ✅ **Image Support**: Profile photo upload និង display សម្រាប់ patients និង doctors

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

មុនពេលចាប់ផ្តើម, សូមធានាថាអ្នកមានដំឡើងខាងក្រោម:

1. **Visual Studio 2022** (ឬកំណែក្រោយ) ជាមួយ:
   - .NET 8.0 SDK
   - Windows Forms development workload
   - SQL Server Data Tools (optional, សម្រាប់ database management)

2. **SQL Server** (មួយក្នុងចំណោមខាងក្រោម):
   - SQL Server 2019 ឬកំណែក្រោយ
   - SQL Server Express (ឥតគិតថ្លៃ)
   - SQL Server LocalDB (រួមបញ្ចូលជាមួយ Visual Studio)

3. **SQL Server Management Studio (SSMS)** ឬ Azure Data Studio (សម្រាប់ database setup)

---

## 🚀 ការណែនាំ Setup

### សម្រាប់ ZIP Distribution

1. **Extract Project**
   - Extract ZIP file ទៅកន្លែងដែលអ្នកចង់ (ឧទាហរណ៍: `D:\Projects\HMS`)
   - ធានាថា files ទាំងអស់ត្រូវបាន extract ឱ្យបានត្រឹមត្រូវ

2. **បើកក្នុង Visual Studio**
   - Navigate ទៅកាន់ folder ដែលបាន extract
   - Double-click `HMS.sln` ដើម្បីបើក solution ក្នុង Visual Studio
   - រង់ចាំ Visual Studio restore NuGet packages (អាចចំណាយពេលមួយរយៈ)

3. **បន្តជាមួយ [Database Setup](#database-setup)**

### សម្រាប់ GitHub Clone

1. **Clone Repository**
   ```bash
   git clone https://github.com/YonPiseth/HMS.git
   cd HMS
   ```

2. **បើកក្នុង Visual Studio**
   - Double-click `HMS.sln` ដើម្បីបើក solution
   - Visual Studio នឹង restore NuGet packages ដោយស្វ័យប្រវត្តិ

3. **បន្តជាមួយ [Database Setup](#database-setup)**

---

## 💾 Database Setup

1. **Configure Database Connection** ⚠️ **ដំបូង**
   - Update connection string ក្នុង `HMS/App.config` (សូមមើល [Configuration](#-configuration) section)
   - Default ប្រើ `.\SQLEXPRESS` - ផ្លាស់ប្តូរប្រសិនបើ SQL Server instance របស់អ្នកខុសគ្នា

2. **Run Database Script**
   - បើក SQL Server Management Studio (SSMS)
   - Connect ទៅ SQL Server instance របស់អ្នក
   - បើក និង execute `HMS/create_hms_db.sql` (ចុច `F5`)
   - រង់ចាំ script បញ្ចប់ដោយជោគជ័យ

3. **Verify Database Creation**
   - ពិនិត្យថា `HMS` database មានជាមួយ tables ទាំងអស់បានបង្កើត
   - Verify initial data ត្រូវបាន populate (user accounts, ជាដើម)

---

## ⚙️ Configuration

### Database Connection String

**⚠️ ត្រូវការ: Configure SQL Server connection របស់អ្នកមុនពេល run application។**

Update connection string ក្នុង `HMS/App.config` ដើម្បី match SQL Server instance របស់អ្នក:

```xml
<connectionStrings>
    <add name="HMSConnection"
         connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True;TrustServerCertificate=True;"
         providerName="Microsoft.Data.SqlClient" />
</connectionStrings>
```

**ផ្លាស់ប្តូរ `Data Source` value ដោយផ្អែកលើ SQL Server setup របស់អ្នក:**
- `.\SQLEXPRESS` - SQL Server Express (default)
- `(localdb)\MSSQLLocalDB` - LocalDB
- `localhost` - Default SQL Server instance
- `ServerName\InstanceName` - Named instance
- `IPAddress,Port` - Remote server (ឧទាហរណ៍: `192.168.1.100,1433`)

**សម្រាប់ SQL Authentication** (ជំនួសឱ្យ Windows Authentication), ផ្លាស់ប្តូរទៅ:
```xml
connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=HMS;User ID=YourUsername;Password=YourPassword;TrustServerCertificate=True;"
```

បន្ទាប់ពីផ្លាស់ប្តូរ connection string, rebuild application។

### Alternative: Update DatabaseHelper.cs

ប្រសិនបើ `App.config` មិនដំណើរការ, អ្នកអាច update `HMS/DatabaseHelper.cs` ដោយផ្ទាល់:

```csharp
public static SqlConnection GetConnection()
{
    return new SqlConnection("Data Source=YOUR_SERVER;Initial Catalog=HMS;Integrated Security=True;TrustServerCertificate=True;");
}
```

---

## ▶️ ការដំណើរការ Application

1. **Build Solution**
   - ក្នុង Visual Studio, right-click លើ `HMS` project
   - Select **Build** ឬ **Rebuild**
   - រង់ចាំ build បញ្ចប់ (ពិនិត្យ errors ក្នុង Output window)

2. **Run Application**
   - ចុច `F5` ឬ click **Start** button
   - Application នឹង launch ជាមួយ login form

3. **Login**
   - ប្រើ default credentials (សូមមើលខាងក្រោម)
   - បន្ទាប់ពី login ជោគជ័យ, អ្នកនឹងឃើញ main dashboard

---

## 🔐 Default Login Credentials

Credentials ខាងក្រោមត្រូវបានបង្កើតដោយ database script:

| Username | Password | Role   |
|----------|----------|--------|
| `admin`  | `admin`  | Admin  |
| `Admin`  | `admin123` | Admin  |

**ចំណាំ**: អ្នកអាចបង្កើត users បន្ថែមតាម database ឬ application (ប្រសិនបើ user management ត្រូវបាន implement)។

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

## 🏗️ ស្ថាបត្យកម្មរចនា និង Patterns

គម្រោងនេះអនុវត្តតាម **ស្ថាបត្យកម្ម N-Layer** បែបទំនើប (UI → Service → Repository → Database) និងប្រើប្រាស់ **Gang of Four (GoF)** design patterns ជាច្រើនដើម្បីធានាបាននូវភាពងាយស្រួលក្នុងការថែទាំ និងពង្រីក។

### 🧩 ចំណាត់ថ្នាក់ Patterns

| ក្រុម (Group) | Pattern ដែលប្រើ | ការអនុវត្តក្នុង HMS |
| :--- | :--- | :--- |
| **Creational** | **Factory Method** | `BaseEntityControl` កំណត់ `CreateEntityForm()` ដែល subclasses អនុវត្តដើម្បីបង្កើត form ជាក់លាក់។ |
| | **Singleton** | `DatabaseHelper` ផ្តល់នូវចំណុចកណ្តាលសម្រាប់គ្រប់គ្រង connection។ |
| | **Dependency Injection** | ប្រើក្នុង `Program.cs` តាមរយៈ `ServiceCollection` ដើម្បីគ្រប់គ្រង object lifecycles។ |
| **Structural** | **Facade / Proxy** | **Service Layer** (`DoctorService`, ល។) ដើរតួជា interface សាមញ្ញរវាង UI និង Repositories។ |
| | **Repository** | `IRepository<T>` និង `BaseRepository<T>` គ្រប់គ្រង abstraction នៃ logic សម្រាប់ការចូលប្រើ data។ |
| **Behavioral** | **Template Method** | `BaseRepository` កំណត់គ្រោងឆ្អឹងនៃ CRUD operations ដែលអនុញ្ញាតឱ្យ repositories ជាក់លាក់ប្តូរ SQL logic បាន។ |
| | **Observer** | មានស្រាប់ក្នុង WinForms event system (ឧទាហរណ៍៖ ការចុចប៊ូតុងដែលជូនដំណឹងដល់ form)។ |

សម្រាប់ការវិភាគលម្អិតអំពីការអនុវត្តទាំងនេះ សូមមើល [OOP_IMPLEMENTATION.md](./OOP_IMPLEMENTATION.md)។

---

## 📋 ការពន្យល់អំពី Forms

### SplashForm
SplashForm គឺជា loading screen ដំបូងដែលបង្ហាញនៅពេល application ចាប់ផ្តើម។ វាមាន animated progress bar, application logo, និង version information ដើម្បីផ្តល់ visual feedback កំឡុងពេល application initialization។ Form នេះនឹងបិទដោយស្វ័យប្រវត្តិនៅពេល loading process បញ្ចប់ ហើយប្តូរទៅ login screen។

### LoginForm
LoginForm គ្រប់គ្រង user authentication ដោយផ្ទៀងផ្ទាត់ username និង password credentials ទាក់ទងនឹង database។ វាគាំទ្រ role-based access control (Admin, Doctor, Patient) និង update user's last login timestamp នៅពេល authentication ជោគជ័យ។ Form នេះផ្តល់ interface ស្អាត ទំនើប ជាមួយ validation ដើម្បីធានាថា fields ទាំងពីរត្រូវបានបញ្ចូលមុនពេល submit។

### MainForm
MainForm ដើរតួជា central dashboard និង navigation hub សម្រាប់ application ទាំងមូលបន្ទាប់ពី login ជោគជ័យ។ វាបង្ហាញ navigation panel ជាមួយ buttons សម្រាប់ចូលប្រើ modules ផ្សេងៗ (Patients, Doctors, Appointments, Billing, ជាដើម) និង adjust interface ដោយស្វ័យប្រវត្តិដោយផ្អែកលើ user's role។ Form នេះប្រើ content panel system ដើម្បីបង្ហាញ user controls ផ្សេងៗ និងផ្តល់ role-specific views ដូចជា hiding navigation សម្រាប់ Patient users។

### PatientForm
PatientForm អនុញ្ញាតឱ្យ users បន្ថែម ឬកែប្រែ patient information ពេញលេញ រួមមាន personal details, contact information, medical data, និង profile photos។ វាគាំទ្រ room assignment functionality ដោយភ្ជាប់ patients ទៅ hospital rooms ជាក់លាក់ និងរួមបញ្ចូល validation ដើម្បីធានាថា required fields ទាំងអស់ត្រូវបានបំពេញឱ្យបានត្រឹមត្រូវ។ Form នេះអាចដំណើរការក្នុង "new patient" និង "edit existing patient" modes ដោយ handle database insertions ឬ updates ដោយស្វ័យប្រវត្តិតាមដែល។

### DoctorForm
DoctorForm អនុញ្ញាតឱ្យគ្រប់គ្រង doctor profiles ជាមួយ fields សម្រាប់ personal information, specialization, qualifications, experience, និង availability status។ វារួមបញ្ចូល photo upload capability និង integrate ជាមួយ doctor specialization lookup table ដើម្បីរក្សា data consistency។ Form នេះ validate required fields ទាំងអស់ និងគាំទ្រ both creating new doctor records និង updating existing ones។

### AppointmentForm
AppointmentForm ជួយក្នុងការ schedule patient appointments ដោយអនុញ្ញាតឱ្យជ្រើសរើស patient, doctor, date, time, និង appointment reason។ វារួមបញ្ចូល status tracking (Scheduled, Completed, Cancelled, No Show) និង load available patients និង doctors ពី database ទៅក្នុង dropdown lists។ Form នេះធានាថា mandatory fields ទាំងអស់ត្រូវបានជ្រើសរើសមុនពេល save appointment record។

### BillingForm
BillingForm ផ្តល់ interface ពេញលេញសម្រាប់ការ create និង manage patient bills ជាមួយ line items, quantities, និង pricing។ វា calculate subtotals ដោយស្វ័យប្រវត្តិ, apply discounts និង tax rates, និង compute grand total ជា real-time នៅពេល items ត្រូវបានបន្ថែម ឬ modified។ Form នេះគាំទ្រ editing existing bills និងរួមបញ្ចូល validation ដើម្បីធានាថា line item យ៉ាងហោចណាស់មួយមានមុនពេល save។

### InvoiceForm
InvoiceForm អនុញ្ញាតឱ្យ users ជ្រើសរើស unpaid bills ច្រើនសម្រាប់ patient និង combine ពួកវាទៅក្នុង invoice តែមួយ។ វាបង្ហាញ list នៃ available bills ជាមួយ checkboxes សម្រាប់ selection និងបង្ហាញ preview នៃ selected bill line items ក្នុង grid ដាច់ដោយឡែក។ Form នេះ generate invoice record ថ្មី, link selected bills ទៅ invoice, និងបើក receipt form នៅពេល creation ជោគជ័យ។

### InvoiceReceiptForm
InvoiceReceiptForm បង្ហាញ receipt view ដែលបាន format នៃ invoice ដែលបាន generate ជាមួយ line items ទាំងអស់, totals, និង payment information។ វាផ្តល់ print-friendly layout បង្ហាញ patient details, invoice number, date, និង billing items ទាំងអស់ដែលពាក់ព័ន្ធ។ Form នេះដើរតួជា both display និង printing interface សម្រាប់ invoice documentation។

### PatientInvoiceHistoryForm
PatientInvoiceHistoryForm បង្ហាញ list ពេញលេញនៃ invoices ទាំងអស់ដែលពាក់ព័ន្ធនឹង patient ជាក់លាក់។ វាបង្ហាញ invoice details រួមមាន dates, amounts, payment status, និងអនុញ្ញាតឱ្យ users មើល ឬ print individual invoice receipts។ Form នេះផ្តល់ historical tracking នៃ patient's billing និង payment records។

### MedicineForm
MedicineForm គ្រប់គ្រង medicine inventory ដោយអនុញ្ញាតឱ្យបញ្ចូល medicine details រួមមាន name, category, supplier, description, dosage, side effects, pricing, និង stock quantity។ វា integrate ជាមួយ supplier lookup table និងរួមបញ្ចូល validation ដើម្បីធានាថា required fields ទាំងអស់ត្រូវបានបំពេញ។ Form នេះគាំទ្រ both adding new medicines និង updating existing inventory records។

### DiseaseForm
DiseaseForm ផ្តល់ interface សាមញ្ញសម្រាប់ការ maintain disease database ជាមួយ disease names និង descriptions។ វារួមបញ្ចូល validation ដើម្បីធានាថា both disease name និង description fields ត្រូវបានបំពេញមុនពេល save។ Form នេះជួយ maintain catalog ពេញលេញនៃ diseases សម្រាប់ reference ក្នុង patient records និង medical documentation។

### RoomForm
RoomForm គ្រប់គ្រង hospital room information រួមមាន room number, type, floor, capacity, daily rate, និង availability status។ វា integrate ជាមួយ room type lookup table និងរួមបញ្ចូល validation សម្រាប់ required fields ដូចជា room number, type, និង status។ Form នេះគាំទ្រ creating new room records និង updating existing room information, រួមមាន capacity និង pricing details។

### SupplierForm
SupplierForm គ្រប់គ្រង supplier information រួមមាន company name, contact person, email, phone, និង address details។ វារួមបញ្ចូល email និង phone number format validation ដើម្បីធានា data quality និងត្រូវការ mandatory fields ទាំងអស់ត្រូវបានបំពេញ។ Form នេះគាំទ្រ both adding new suppliers និង updating existing supplier records សម្រាប់ medicine procurement management។

### PatientRegistrationForm
PatientRegistrationForm ផ្តល់ patient registration interface លម្អិតជាមួយ validation ពេញលេញ រួមមាន email format, phone number format, និង required field checks។ វារួមបញ្ចូល tooltips សម្រាប់ user guidance និងប្រើ error provider ដើម្បីបង្ហាញ validation messages inline។ Form នេះគាំទ្រ both new patient registration និង editing existing patient records ជាមួយ data validation ត្រឹមត្រូវមុន database operations។

---


## 🔧 Troubleshooting

### Common Issues

#### 1. **"Cannot connect to database" Error**
- **Solution**: Verify SQL Server កំពុង run និង connection string ក្នុង `App.config` ត្រឹមត្រូវ
- ពិនិត្យ SQL Server services: `Services.msc` → SQL Server (SQLEXPRESS)
- Test connection ក្នុង SSMS ជាមួយ connection string ដូចគ្នា

#### 2. **"Image not found" Error**
- **Solution**: ធានាថា `Image` folder ត្រូវបាន copy ទៅ output directory
- ពិនិត្យ `HMS.csproj` រួមមាន: `<Content Include="Image\**">`
- Rebuild solution

#### 3. **Build Errors**
- **Solution**: Restore NuGet packages: Right-click solution → Restore NuGet Packages
- Clean និង rebuild: Build → Clean Solution, បន្ទាប់មក Build → Rebuild Solution
- ពិនិត្យ .NET 8.0 SDK ត្រូវបាន install: `dotnet --version`

#### 4. **Database Script Fails**
- **Solution**: ធានាថាអ្នកមាន permissions ដើម្បី create databases
- ពិនិត្យ `HMS` database ដែលមានស្រាប់ និង drop វាមុន
- Run script section by section ប្រសិនបើត្រូវការ

#### 5. **Login Fails**
- **Solution**: Verify user accounts មាននៅក្នុង `tblUser` table
- ពិនិត្យ `IsActive = 1` សម្រាប់ user account
- Verify password ត្រឹមត្រូវ (case-sensitive)

---

## 🤝 Contributing

Contributions ត្រូវបានស្វាគមន៍! សូមធ្វើតាមជំហានខាងក្រោម:

1. **Fork repository**
2. **Create feature branch** (`git checkout -b feature/AmazingFeature`)
3. **Commit changes** (`git commit -m 'Add some AmazingFeature'`)
4. **Push to branch** (`git push origin feature/AmazingFeature`)
5. **Open Pull Request**

### Contribution Guidelines
- ធ្វើតាម C# coding conventions
- បន្ថែម comments សម្រាប់ complex logic
- Test changes របស់អ្នកឱ្យបានពេញលេញ
- Update documentation ប្រសិនបើត្រូវការ

---

## 📄 License

Project នេះត្រូវបាន license ក្រោម **Group 2** - សូមមើល LICENSE file សម្រាប់ details។

---

## 📧 Contact

**Project Maintainer**: YonPiseth

- **Email**: Pisethyon987@gmail.com
- **GitHub**: [@YonPiseth](https://github.com/YonPiseth)

សម្រាប់សំណួរ, ការផ្តល់យោបល់, ឬ issues, សូមបើក issue នៅលើ GitHub ឬទាក់ទង maintainer។

---

## 🙏 Acknowledgments

- អរគុណចំពោះ contributors ទាំងអស់ដែលបានជួយកែលម្អ project នេះ
- អរគុណពិសេសចំពោះ .NET និង SQL Server communities សម្រាប់ documentation ល្អបំផុត

---

## 📝 សេចក្តីសន្និដ្ឋាន

Hospital Management System (HMS) គឺជា desktop application ពេញលេញ និងមានប្រសិទ្ធភាពដែលបានបង្កើតឡើងដើម្បី streamline និង automate operations នៃមន្ទីរពេទ្យ។ System នេះផ្តល់ solution ពេញលេញសម្រាប់ការគ្រប់គ្រង patient records, doctor information, appointments, billing, invoicing, medicine inventory, disease database, room management, និង supplier information ក្នុង platform តែមួយ។

ជាមួយ role-based access control (Admin, Doctor, Patient), system នេះធានាថា users មាន access ទៅកាន់ features និង data ដែលសមរម្យតាម role របស់ពួកគេ។ Modern user interface ជាមួយ validation ពេញលេញ, search functionality, និង soft delete mechanism ធ្វើឱ្យ system នេះងាយប្រើ និងអាចពឹងផ្អែកបាន។

Technical architecture ដែលប្រើ centralized database connection, Windows Forms framework, និង SQL Server database ធានាថា system នេះមាន performance ល្អ, scalable, និងងាយ maintain។ Features ដូចជា real-time billing calculations, invoice generation, appointment scheduling, និង comprehensive reporting ផ្តល់ tools ចាំបាច់សម្រាប់ hospital staff ដើម្បីគ្រប់គ្រង daily operations ឱ្យបានមានប្រសិទ្ធភាព។

System នេះមិនត្រឹមតែជួយកាត់បន្ថយ workload របស់ hospital staff ប៉ុណ្ណោះទេ ប៉ុន្តែក៏ជួយកែលម្អ accuracy នៃ data, reduce errors, និង enhance overall patient care experience។ ជាមួយ documentation ពេញលេញ, setup instructions ច្បាស់លាស់, និង troubleshooting guide, HMS គឺជា solution ដែលត្រៀមខ្លួនសម្រាប់ deployment និង usage ក្នុង environment ជាក់ស្តែង។

ក្នុងសង្ខាប់, Hospital Management System នេះផ្តល់ foundation រឹងមាំសម្រាប់ការគ្រប់គ្រង hospital operations ទាំងមូល, ដោយផ្តល់ tools និង features ចាំបាច់ដើម្បីធ្វើឱ្យ hospital management កាន់តែ efficient, accurate, និង user-friendly។

---

**Made with ❤️ by Group 2**

