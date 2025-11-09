using System;

namespace HMS.Models
{
    /// <summary>
    /// Represents a patient in the hospital management system
    /// Inherits from Person base class
    /// </summary>
    public class Patient : Person
    {
        // Patient-specific properties
        public int PatientID { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string BloodType { get; set; }
        public string InsuranceNumber { get; set; }
        public string PatientFamily { get; set; }
        public string Status { get; set; }
        public int? UserID { get; set; }
        public bool IsDeleted { get; set; }
        public string RoomNumber { get; set; } // For display purposes

        // Default constructor
        public Patient() : base()
        {
            Gender = string.Empty;
            BloodType = string.Empty;
            InsuranceNumber = string.Empty;
            PatientFamily = string.Empty;
            Status = string.Empty;
            DateOfBirth = DateTime.Now;
        }

        // Constructor with patient info
        public Patient(string firstName, string lastName, string email, string phone, 
                      DateTime dateOfBirth, string gender, string address = "") 
            : base(firstName, lastName, email, phone, address)
        {
            DateOfBirth = dateOfBirth;
            Gender = gender ?? string.Empty;
            BloodType = string.Empty;
            InsuranceNumber = string.Empty;
            PatientFamily = string.Empty;
            Status = string.Empty;
        }

        // Override validation to include patient-specific fields
        public override bool Validate()
        {
            return base.Validate() &&
                   !string.IsNullOrWhiteSpace(Gender) &&
                   !string.IsNullOrWhiteSpace(BloodType) &&
                   !string.IsNullOrWhiteSpace(Status);
        }

        // Calculate age
        public int GetAge()
        {
            DateTime today = DateTime.Today;
            int age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }

        // Override virtual methods from Person for polymorphism
        /// <summary>
        /// Gets the display name with patient identifier
        /// </summary>
        public override string GetDisplayName()
        {
            return $"{FullName} (Patient)";
        }

        /// <summary>
        /// Gets detailed patient information
        /// </summary>
        public override string GetDisplayInfo()
        {
            return $"{FullName} - Age: {GetAge()} - Status: {Status}";
        }

        /// <summary>
        /// Patients can only be deleted if they are not active
        /// </summary>
        public override bool CanBeDeleted()
        {
            return Status != "Active";
        }

        /// <summary>
        /// Returns the entity type
        /// </summary>
        public override string GetEntityType()
        {
            return "Patient";
        }
    }
}

