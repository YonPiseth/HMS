using System.Collections.Generic;
using System.Threading.Tasks;
using HMS.Models;
using HMS.Repositories;
using System.Linq;

namespace HMS.Services
{
    public class PatientService : IPatientService
    {
        private readonly IRepository<Patient> _patientRepository;

        public PatientService(IRepository<Patient> patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public List<Patient> GetAll() => GetAllPatients();
        public Task<List<Patient>> GetAllAsync() => GetAllPatientsAsync();

        public Patient GetById(int id) => GetPatientById(id);
        public Task<Patient> GetByIdAsync(int id) => GetPatientByIdAsync(id);

        public bool Insert(Patient entity) => AddPatient(entity);
        public Task<bool> InsertAsync(Patient entity) => AddPatientAsync(entity);

        public bool Update(Patient entity) => UpdatePatient(entity);
        public Task<bool> UpdateAsync(Patient entity) => UpdatePatientAsync(entity);

        public bool Delete(int id) => DeletePatient(id);
        public Task<bool> DeleteAsync(int id) => DeletePatientAsync(id);

        public List<Patient> Search(string searchTerm) => SearchPatients(searchTerm);
        public Task<List<Patient>> SearchAsync(string searchTerm) => SearchPatientsAsync(searchTerm);

        public List<Patient> GetAllPatients()
        {
            if (_patientRepository is PatientRepository patientRepo)
            {
                return patientRepo.GetAllWithRooms();
            }
            return _patientRepository.GetAll();
        }

        public async Task<List<Patient>> GetAllPatientsAsync()
        {
            if (_patientRepository is PatientRepository patientRepo)
            {
                return await patientRepo.GetAllWithRoomsAsync();
            }
            return await _patientRepository.GetAllAsync();
        }

        public Patient GetPatientById(int id) => _patientRepository.GetById(id);
        public Task<Patient> GetPatientByIdAsync(int id) => _patientRepository.GetByIdAsync(id);

        public bool AddPatient(Patient patient) => _patientRepository.Insert(patient);
        public Task<bool> AddPatientAsync(Patient patient) => _patientRepository.InsertAsync(patient);

        public bool UpdatePatient(Patient patient) => _patientRepository.Update(patient);
        public Task<bool> UpdatePatientAsync(Patient patient) => _patientRepository.UpdateAsync(patient);

        public bool DeletePatient(int id) => _patientRepository.Delete(id);
        public Task<bool> DeletePatientAsync(int id) => _patientRepository.DeleteAsync(id);

        public List<Patient> SearchPatients(string searchTerm) => _patientRepository.Search(searchTerm);
        public Task<List<Patient>> SearchPatientsAsync(string searchTerm) => _patientRepository.SearchAsync(searchTerm);
    }
}
