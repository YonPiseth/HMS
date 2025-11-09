using System;

namespace HMS.Models
{
    /// <summary>
    /// Base class representing a person with common properties
    /// Implements IContactable for contact information
    /// </summary>
    public abstract class Person : IContactable
    {
        // Person-specific properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] ProfilePhoto { get; set; }

        // IContactable implementation
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        // Computed property for full name
        public string FullName => $"{FirstName} {LastName}".Trim();

        // Default constructor
        public Person()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            Address = string.Empty;
        }

        // Constructor with basic info
        public Person(string firstName, string lastName, string email, string phone, string address = "")
        {
            FirstName = firstName ?? string.Empty;
            LastName = lastName ?? string.Empty;
            Email = email ?? string.Empty;
            Phone = phone ?? string.Empty;
            Address = address ?? string.Empty;
        }

        // Validation method
        public virtual bool Validate()
        {
            return !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Phone);
        }

        // Virtual methods for polymorphism - can be overridden in derived classes
        /// <summary>
        /// Gets the display name for the person
        /// Can be overridden in derived classes for custom display
        /// </summary>
        public virtual string GetDisplayName()
        {
            return FullName;
        }

        /// <summary>
        /// Gets detailed display information about the person
        /// Can be overridden in derived classes for custom display
        /// </summary>
        public virtual string GetDisplayInfo()
        {
            return $"{FullName} - {Email}";
        }

        /// <summary>
        /// Determines if the person entity can be deleted
        /// Can be overridden in derived classes for business logic
        /// </summary>
        public virtual bool CanBeDeleted()
        {
            return true;
        }

        /// <summary>
        /// Gets the entity type name
        /// Can be overridden in derived classes
        /// </summary>
        public virtual string GetEntityType()
        {
            return "Person";
        }

        // Override ToString for display
        public override string ToString()
        {
            return GetDisplayName();
        }
    }
}

