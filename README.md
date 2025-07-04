# ប្រព័ន្ធគ្រប់គ្រងមន្ទីរពេទ្យ (HMS)

## សង្ខេប
ប្រព័ន្ធគ្រប់គ្រងមន្ទីរពេទ្យ (HMS) គឺជាកម្មវិធី Desktop ដែលត្រូវបានរចនាឡើងសម្រាប់គ្រប់គ្រងប្រតិបត្តិការនានានៃមន្ទីរពេទ្យ រួមមាន កំណត់ត្រាអ្នកជំងឺ ការណាត់ជួបវេជ្ជបណ្ឌិត ការគ្រប់គ្រងបន្ទប់ និងវិក្កយបត្រ។ វាបានផ្តល់ដំណោះស្រាយពេញលេញសម្រាប់ស្ថាប័នសុខាភិបាល ដើម្បីធ្វើឲ្យដំណើរការរដ្ឋបាល និងវេជ្ជសាស្ត្ររបស់ពួកគេមានប្រសិទ្ធភាព។

## តម្រូវការប្រើប្រាស់

### តម្រូវការប្រព័ន្ធ
- Windows 10 ឬថ្មីជាងនេះ
- .NET 6.0 Runtime ឬថ្មីជាងនេះ
- SQL Server 2019 ឬថ្មីជាងនេះ (Express edition គាំទ្រ)
- អង្គភាពចងចាំ (RAM) 4GB ឡើងទៅ
- ទំហំថាសទំនេរ 1GB

### តម្រូវការអភិវឌ្ឍន៍
- Visual Studio 2022 ឬថ្មីជាងនេះ
- .NET 6.0 SDK
- SQL Server Management Studio (SSMS) ឬ Azure Data Studio
- Git (សម្រាប់គ្រប់គ្រងកំណែ)

### NuGet Packages
- Microsoft.Data.SqlClient (6.0.2)
- System.Data.SqlClient (4.8.5)
- System.Configuration.ConfigurationManager (6.0.0)

## ការរៀបចំមូលដ្ឋានទិន្នន័យ

### 1. ការតំឡើង SQL Server
- ទាញយក និងតំឡើង SQL Server (Express edition គឺឥតគិតថ្លៃ និងគ្រប់គ្រាន់សម្រាប់អភិវឌ្ឍន៍)
- នៅពេលតំឡើង សូមជ្រើសរើសបន្ថែម៖
  - Database Engine Services
  - SQL Server Management Studio (SSMS)
  - Client Tools Connectivity

### 2. បង្កើតមូលដ្ឋានទិន្នន័យ
1. បើក SQL Server Management Studio (SSMS)
2. តភ្ជាប់ទៅ SQL Server instance របស់អ្នក
3. បើកឯកសារ `create_hms_db.sql` ដែលមាននៅក្នុងថត `hms`
4. ប្រតិបត្តិស្គ្រីបដើម្បី៖
   - បង្កើតមូលដ្ឋានទិន្នន័យ HMS
   - បង្កើតតារាងទាំងអស់
   - កំណត់ Stored Procedures
   - កំណត់ទិន្នន័យដំបូង

### 3. ការកំណត់រចនាសម្ព័ន្ធមូលដ្ឋានទិន្នន័យ
1. បើក `App.config` ក្នុងគម្រោង
2. កែប្រែ Connection String ដូចខាងក្រោម៖
   ```xml
   <connectionStrings>
     <add name="HMSConnection" 
          connectionString="Server=YOUR_SERVER;Database=HMS;Trusted_Connection=True;TrustServerCertificate=True;"
          providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

## ការសាងសង់ និងដំណើរការ

### 1. Clone Repository
```bash
git clone https://github.com/yourusername/HMS.git
cd HMS
```

### 2. Build Project
```bash
cd hms
dotnet build
```

### 3. Run Application
```bash
dotnet run
```

## លក្ខណៈពិសេស

### ការគ្រប់គ្រងអ្នកជំងឺ
- បន្ថែម កែប្រែ និងលុបកំណត់ត្រាអ្នកជំងឺ
- ផ្ទុក និងគ្រប់គ្រងឯកសារអ្នកជំងឺ
- តាមដានប្រវត្តិវេជ្ជសាស្ត្រអ្នកជំងឺ
- គ្រប់គ្រងព័ត៌មានធានារ៉ាប់រងរបស់អ្នកជំងឺ

### ការគ្រប់គ្រងវេជ្ជបណ្ឌិត
- រក្សាទុកប្រវត្តិរូបវេជ្ជបណ្ឌិតជាមួយជំនាញ
- តាមដានកាលវិភាគ និងភាពមានស្រាប់របស់វេជ្ជបណ្ឌិត
- គ្រប់គ្រងគុណវុឌ្ឍិ និងបទពិសោធន៍វេជ្ជបណ្ឌិត
- ផ្ទុក និងគ្រប់គ្រងរូបថតវេជ្ជបណ្ឌិត

### ការកំណត់ពេលវេលាជួប
- កំណត់ពេលវេលាជួបអ្នកជំងឺ
- កំណត់អាទិភាពការណាត់ជួប
- គ្រប់គ្រងការណាត់ជួបបន្ត
- ផ្ញើការរំលឹកការណាត់ជួប

### ការគ្រប់គ្រងបន្ទប់
- តាមដានភាពមានស្រាប់នៃបន្ទប់
- គ្រប់គ្រងប្រភេទបន្ទប់ និងតម្លៃ
- កំណត់កាលវិភាគថែទាំបន្ទប់
- គ្រប់គ្រងការបែងចែកបន្ទប់

### វិក្កយបត្រ និងវិក្កយបត្រ
- បង្កើតវិក្កយបត្រអ្នកជំងឺ
- តាមដានការទូទាត់
- គ្រប់គ្រងការទាមទារធានារ៉ាប់រង
- បង្កើតរបាយការណ៍ហិរញ្ញវត្ថុ

### ការគ្រប់គ្រងថ្នាំ
- តាមដានស្តុកថ្នាំ
- គ្រប់គ្រងប្រភេទថ្នាំ
- កត់ត្រាការប្រើប្រាស់ថ្នាំ
- បង្កើតរបាយការណ៍វេជ្ជបញ្ជា

## ការដោះស្រាយបញ្ហា

### បញ្ហាទូទៅ

1. **Database Connection Error**
   - ពិនិត្យមើលថា SQL Server កំពុងដំណើរការ
   - ពិនិត្យ Connection String ក្នុង App.config
   - ប្រាកដថា Windows Authentication បានបើក

2. **Image Upload Issues**
   - ពិនិត្យថាមានទំហំថាសគ្រប់គ្រាន់
   - ពិនិត្យសិទ្ធិឯកសារ
   - ប្រាកដថា Image Format គឺ PNG/JPG

3. **Application Crashes**
   - ពិនិត្យ Windows Event Viewer សម្រាប់កំហុស
   - ពិនិត្យថា .NET Runtime ត្រូវបានដំឡើង
   - ប្រាកដថា NuGet packages ទាំងអស់ត្រូវបាន Restore

### ជំនួយបន្ថែម
សម្រាប់ជំនួយបន្ថែម៖
1. ពិនិត្យផ្នែក [Issues](https://github.com/yourusername/HMS/issues)
2. បង្កើត Issue ថ្មីជាមួយព័ត៌មានកំហុសលម្អិត
3. ទាក់ទងក្រុមអភិវឌ្ឍន៍

## អាជ្ញាប័ណ្ណ
គម្រោងនេះមានអាជ្ញាប័ណ្ណ MIT។ សូមមើលឯកសារ LICENSE សម្រាប់ព័ត៌មានលម្អិត។

## ការចូលរួម
1. Fork repository
2. បង្កើត feature branch
3. Commit ការផ្លាស់ប្តូររបស់អ្នក
4. Push ទៅ branch
5. បង្កើត Pull Request 
