# UML Diagram Explanations for Presentation

## 1. Complete System Class Diagram

Explanation:

Diagram នេះផ្តល់នូវទិដ្ឋភាពទូទៅនៃ architecture ទាំងមូលនៃ Hospital Management System ដោយបង្ហាញ components សំខាន់ៗទាំងអស់ និង relationships របស់វា។ វាបង្ហាញពីរបៀបដែល system ត្រូវបានរៀបចំទៅជា layers ដាច់ដោយឡែក៖

- Models Layer (HMS.Models): Contains business entities សំខាន់ៗ including Person (abstract base class), Patient, Doctor, និង Supplier។ Person class implements IContactable interface ខណៈដែល Supplier implements វាដោយផ្ទាល់ពីព្រោះវាមិនមែនជា person entity។

- Repositories Layer (HMS.Repositories): Implements Repository Pattern ជាមួយ IRepository<T> interface, BaseRepository<T> abstract class, និង concrete implementations (PatientRepository, DoctorRepository, SupplierRepository)។ Layer នេះ handles data access operations ទាំងអស់។

- Controls Layer (HMS.Controls): Contains BaseEntityControl<T, TRepository> ដែល provides common UI functionality សម្រាប់ entity management។ វា uses repositories ដើម្បី perform CRUD operations។

- UI Layer (HMS.UI): Contains helper classes (UIHelper, UITheme) ដែល provide consistent styling និង theming នៅទូទាំង application។

Diagram នេះ illustrates OOP principles សំខាន់ៗ: Inheritance (Person → Patient/Doctor), Interface Implementation (IContactable, IRepository), Abstraction (abstract classes), និង Dependency Relationships (Controls use Repositories, Repositories manage Models)។

---

## 2. Person Hierarchy Class Diagram

Explanation:

Diagram នេះ demonstrates Inheritance និង Polymorphism ក្នុង model layer។ វាបង្ហាញពីរបៀបដែល abstract Person class serves as base class សម្រាប់ Patient និង Doctor, sharing common properties ដូចជា FirstName, LastName, Email, Phone, និង Address។

OOP concepts សំខាន់ៗ illustrated:

- Inheritance: Patient និង Doctor inherit from Person, inheriting properties និង methods ទាំងអស់ពី base class។ នេះ promotes code reuse និង establishes "is-a" relationship (Patient IS A Person, Doctor IS A Person)។

- Interface Implementation: Person class implements IContactable, ដែល defines contract សម្រាប់ entities ដែលមាន contact information។ Supplier also implements IContactable directly, showing that different inheritance hierarchies can implement same interface។

- Polymorphism: Person class defines virtual methods ដូចជា Validate(), GetDisplayName(), និង CanBeDeleted() ដែល can be overridden ក្នុង derived classes។ Patient និង Doctor override methods ទាំងនេះ to provide entity-specific behavior (ឧទាហរណ៍: Patient can only be deleted if Status is not "Active", while Doctor can only be deleted if not available)។

- Abstraction: Person class is abstract, meaning it cannot be instantiated directly។ វា serves as template សម្រាប់ creating person entities, hiding implementation details while providing common interface។

- Encapsulation: Properties are encapsulated with visibility modifiers (+ for public, # for protected), controlling access to class members។

---

## 3. Repository Pattern Class Diagram

Explanation:

Diagram នេះ illustrates Repository Pattern, design pattern ដែល abstracts data access logic។ វា demonstrates របៀប OOP principles are applied to create flexible, maintainable data access layer។

OOP concepts សំខាន់ៗ:

- Interface Segregation: IRepository<T> interface defines clean, focused contract សម្រាប់ CRUD operations។ Repositories ទាំងអស់ implement interface នេះ, ensuring consistent behavior across different entity types។

- Inheritance: BaseRepository<T> is abstract base class ដែល provides common implementation សម្រាប់ CRUD operations ទាំងអស់។ Concrete repositories (PatientRepository, DoctorRepository, SupplierRepository) inherit from base class នេះ, inheriting shared functionality while implementing entity-specific logic។

- Template Method Pattern: BaseRepository<T> uses Template Method Pattern - វា defines algorithm structure (ឧទាហរណ៍: GetAll() method) but delegates specific steps (ដូចជា MapDataReader() និង CreateInsertCommand()) to derived classes។ នេះ allows code reuse while maintaining flexibility។

- Polymorphism: Repositories ទាំងអស់ can be used polymorphically through IRepository<T> interface។ Code can work with any repository type without knowing specific implementation។

- Abstraction: Abstract methods ក្នុង BaseRepository<T> (MapDataReader(), CreateInsertCommand(), etc.) hide implementation details from consumers។ Derived classes must implement methods ទាំងនេះ, but base class handles overall flow។

- Generic Programming: Use of generics (IRepository<T>, BaseRepository<T>) allows type-safe, reusable code ដែល works with any entity type។

Diagram នេះ shows របៀប PatientRepository, DoctorRepository, និង SupplierRepository follow same pattern but have entity-specific implementations for data mapping និង command creation។

---

## 4. Control Hierarchy Class Diagram

Explanation:

Diagram នេះ shows UI architecture using inheritance និង Template Method Pattern។ BaseEntityControl<T, TRepository> is abstract base class ដែល provides common UI functionality សម្រាប់ entity management controls។

OOP concepts សំខាន់ៗ:

- Inheritance: BaseEntityControl<T, TRepository> inherits from UserControl (WinForms base class), gaining standard control functionality ទាំងអស់ while adding entity-specific features។

- Template Method Pattern: Base control defines UI structure និង common operations (ដូចជា button click handlers), but delegates entity-specific operations to abstract methods ដែល must be implemented by derived classes។ ឧទាហរណ៍: btnAdd_Click() is implemented ក្នុង base class, but it calls abstract CreateEntityForm() method ដែល each derived class implements differently។

- Dependency on Interfaces: Control uses IRepository<T> interface rather than concrete repository classes, demonstrating Dependency Inversion Principle - high-level modules (controls) depend on abstractions (interfaces), not concrete implementations។

- Composition: Control uses UIHelper និង IRepository<T> through composition, showing how classes can work together without tight coupling។

- Abstraction: Abstract methods ដូចជា GetTitle(), LoadData(), និង CreateEntityForm() define contract ដែល derived classes must fulfill, hiding implementation details while providing consistent interface។

- Encapsulation: Protected members (#) are used for internal UI components និង repository, ensuring proper encapsulation while allowing derived classes to access necessary members។

Pattern នេះ allows creating new entity management controls (ដូចជា PatientControl, DoctorControl) with minimal code duplication - they only need to implement abstract methods while inheriting common functionality ទាំងអស់។

---

## 5. Polymorphic Repository Sequence Diagram

Explanation:

Sequence diagram នេះ demonstrates Polymorphism in action, showing how same interface can be used with different implementations at runtime។

OOP concepts សំខាន់ៗ:

- Interface Polymorphism: Client code works with IRepository<Patient> និង IRepository<Doctor> interfaces, not concrete classes។ នេះ allows same code pattern to work with different entity types។

- Runtime Polymorphism: When GetAll() is called on IRepository<Patient>, it actually calls PatientRepository.GetAll() method។ Similarly, IRepository<Doctor> calls DoctorRepository.GetAll()។ Actual implementation is determined at runtime based on object type។

- Code Reusability: BaseRepository<T> class provides common GetAll() implementation, ដែល is inherited by both PatientRepository និង DoctorRepository។ However, each repository uses its own TableName property និង MapDataReader() method, showing how polymorphism enables code reuse while maintaining flexibility។

- Inheritance in Action: Repositories ទាំងពីរ inherit GetAll() from BaseRepository<T>, but base class uses abstract properties និង methods (TableName, MapDataReader()) that are implemented differently in each derived class។

- Loose Coupling: Client code doesn't need to know specific repository implementation - it works with interface, making code more flexible និង maintainable។

Diagram នេះ illustrates how polymorphism enables writing generic code that works with multiple types, reducing code duplication និង improving maintainability។

---

## 6. Patient Insert Sequence Diagram

Explanation:

Sequence diagram នេះ shows complete use case - inserting new patient into system។ វា demonstrates how OOP components ទាំងអស់ work together in real-world scenario។

OOP concepts សំខាន់ៗ in action:

- Separation of Concerns: Flow is divided into distinct layers - UI (PatientControl, PatientForm), Business Logic (PatientRepository), Data Access (BaseRepository), និង Database (DatabaseHelper, SQL Server)។ Each layer has specific responsibility។

- Inheritance: PatientRepository inherits Insert() method from BaseRepository<Patient>។ Base class handles overall flow (get connection, execute command, handle result), while derived class provides entity-specific implementation (CreateInsertCommand(), SetEntityId())។

- Template Method Pattern: BaseRepository.Insert() method defines algorithm (get connection → create command → execute → set ID), but delegates specific steps to abstract methods implemented by PatientRepository។

- Polymorphism: PatientRepository is used through its base class interface, but actual implementation methods are called based on concrete type។

- Encapsulation: Each class encapsulates its own responsibilities - form handles UI និង validation, repository handles data access, និង database helper manages connections។

- Abstraction: Form doesn't need to know details of SQL command creation or database connection management - it simply calls repository.Insert(patient) និង repository handles complexity ទាំងអស់។

Diagram នេះ shows complete lifecycle: user interaction → form validation → repository insertion → database execution → result feedback → data refresh។ នេះ demonstrates how OOP principles enable clean, maintainable code that separates concerns while maintaining clear communication between components។

---

## Summary

Diagrams ទាំងប្រាំមួយនេះ collectively demonstrate:

1. Complete System: Overall architecture និង component relationships
2. Person Hierarchy: Inheritance និង polymorphism in models
3. Repository Pattern: Design pattern implementation with abstraction
4. Control Hierarchy: Template Method Pattern in UI layer
5. Polymorphic Repository: Runtime polymorphism in action
6. Patient Insert Sequence: Complete use case showing principles ទាំងអស់ working together

Together, they showcase well-architected system that follows OOP principles និង design patterns, resulting in maintainable, extensible, និង testable code។

