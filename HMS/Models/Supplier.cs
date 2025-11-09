namespace HMS.Models
{
    /// <summary>
    /// Represents a supplier in the hospital management system
    /// Implements IContactable (not a Person, as it's an organization)
    /// </summary>
    public class Supplier : IContactable
    {
        // Supplier-specific properties
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string ContactPerson { get; set; }
        public bool IsDeleted { get; set; }

        // IContactable implementation
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        // Default constructor
        public Supplier()
        {
            SupplierName = string.Empty;
            ContactPerson = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            Address = string.Empty;
        }

        // Constructor with supplier info
        public Supplier(string supplierName, string contactPerson, string email, string phone, string address = "")
        {
            SupplierName = supplierName ?? string.Empty;
            ContactPerson = contactPerson ?? string.Empty;
            Email = email ?? string.Empty;
            Phone = phone ?? string.Empty;
            Address = address ?? string.Empty;
        }

        // Validation method
        public bool Validate()
        {
            return !string.IsNullOrWhiteSpace(SupplierName) &&
                   !string.IsNullOrWhiteSpace(ContactPerson) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Phone);
        }

        // Display methods (similar to Person but for Supplier)
        /// <summary>
        /// Gets the display name for the supplier
        /// </summary>
        public string GetDisplayName()
        {
            return SupplierName;
        }

        /// <summary>
        /// Gets detailed supplier information
        /// </summary>
        public string GetDisplayInfo()
        {
            return $"{SupplierName} - Contact: {ContactPerson} - {Email}";
        }

        /// <summary>
        /// Determines if the supplier can be deleted
        /// </summary>
        public bool CanBeDeleted()
        {
            return true; // Suppliers can always be deleted (soft delete)
        }

        /// <summary>
        /// Returns the entity type
        /// </summary>
        public string GetEntityType()
        {
            return "Supplier";
        }

        // Override ToString for display
        public override string ToString()
        {
            return GetDisplayName();
        }
    }
}

