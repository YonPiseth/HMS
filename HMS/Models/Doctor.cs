namespace HMS.Models
{
    /// <summary>
    /// Represents a doctor in the hospital management system
    /// Inherits from Person base class
    /// </summary>
    public class Doctor : Person
    {
        // Doctor-specific properties
        public int DoctorID { get; set; }
        public int SpecializationID { get; set; }
        public string SpecializationName { get; set; }
        public int YearsOfExperience { get; set; }
        public string Qualification { get; set; }
        public string Department { get; set; }
        public string WorkingHours { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsDeleted { get; set; }

        // Default constructor
        public Doctor() : base()
        {
            SpecializationName = string.Empty;
            Qualification = string.Empty;
            Department = string.Empty;
            WorkingHours = string.Empty;
            IsAvailable = true;
        }

        // Constructor with doctor info
        public Doctor(string firstName, string lastName, string email, string phone, 
                     int specializationID, int yearsOfExperience, string address = "") 
            : base(firstName, lastName, email, phone, address)
        {
            SpecializationID = specializationID;
            YearsOfExperience = yearsOfExperience;
            SpecializationName = string.Empty;
            Qualification = string.Empty;
            Department = string.Empty;
            WorkingHours = string.Empty;
            IsAvailable = true;
        }

        // Override validation to include doctor-specific fields
        public override bool Validate()
        {
            return base.Validate() &&
                   SpecializationID > 0;
        }

        // Get doctor's full title
        public string GetTitle()
        {
            return $"{FullName} - {SpecializationName} ({YearsOfExperience} years)";
        }

        // Override virtual methods from Person for polymorphism
        /// <summary>
        /// Gets the display name with specialization
        /// </summary>
        public override string GetDisplayName()
        {
            if (!string.IsNullOrWhiteSpace(SpecializationName))
                return $"{FullName}, {SpecializationName}";
            return FullName;
        }

        /// <summary>
        /// Gets detailed doctor information
        /// </summary>
        public override string GetDisplayInfo()
        {
            return $"{FullName} - {SpecializationName} ({YearsOfExperience} years experience)";
        }

        /// <summary>
        /// Doctors can only be deleted if they are not available
        /// </summary>
        public override bool CanBeDeleted()
        {
            return !IsAvailable;
        }

        /// <summary>
        /// Returns the entity type
        /// </summary>
        public override string GetEntityType()
        {
            return "Doctor";
        }
    }
}

