using System.Collections.Generic;
using System.Threading.Tasks;
using HMS.Models;
using HMS.Repositories;

namespace HMS.Services
{
    public interface IPatientService : IRepository<Patient>
    {
        List<Patient> GetAllPatients();
        Task<List<Patient>> GetAllPatientsAsync();
        
        Patient GetPatientById(int id);
        Task<Patient> GetPatientByIdAsync(int id);
        
        bool AddPatient(Patient patient);
        Task<bool> AddPatientAsync(Patient patient);
        
        bool UpdatePatient(Patient patient);
        Task<bool> UpdatePatientAsync(Patient patient);
        
        bool DeletePatient(int id);
        Task<bool> DeletePatientAsync(int id);
        
        List<Patient> SearchPatients(string searchTerm);
        Task<List<Patient>> SearchPatientsAsync(string searchTerm);
    }
}
