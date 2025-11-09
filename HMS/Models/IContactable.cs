namespace HMS.Models
{
    /// <summary>
    /// Interface for entities that have contact information (Email, Phone, Address)
    /// </summary>
    public interface IContactable
    {
        string Email { get; set; }
        string Phone { get; set; }
        string Address { get; set; }
    }
}

