using HMS.Repositories;
using HMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HMS.Services
{
    public interface IDoctorService : IRepository<Doctor>
    {
        List<Doctor> GetAllDoctors();
        Task<List<Doctor>> GetAllDoctorsAsync();

        List<Doctor> SearchDoctors(string searchTerm);
        Task<List<Doctor>> SearchDoctorsAsync(string searchTerm);

        Doctor GetDoctorById(int id);
        Task<Doctor> GetDoctorByIdAsync(int id);

        bool AddDoctor(Doctor doctor);
        Task<bool> AddDoctorAsync(Doctor doctor);

        bool UpdateDoctor(Doctor doctor);
        Task<bool> UpdateDoctorAsync(Doctor doctor);

        bool DeleteDoctor(int id);
        Task<bool> DeleteDoctorAsync(int id);
        
        // Example of business logic that doesn't belong in a repository
        bool IsDoctorAvailable(int doctorId);
        Task<bool> IsDoctorAvailableAsync(int doctorId);

        List<Doctor> GetDoctorsBySpecialization(int specializationId);
    }
}
