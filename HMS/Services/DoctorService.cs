using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMS.Models;
using HMS.Repositories;

namespace HMS.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IRepository<Doctor> _doctorRepository;

        public DoctorService() : this(new DoctorRepository())
        {
        }

        public DoctorService(IRepository<Doctor> doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public List<Doctor> GetAll() => GetAllDoctors();
        public Task<List<Doctor>> GetAllAsync() => GetAllDoctorsAsync();

        public Doctor GetById(int id) => GetDoctorById(id);
        public Task<Doctor> GetByIdAsync(int id) => GetDoctorByIdAsync(id);

        public bool Insert(Doctor entity) => AddDoctor(entity);
        public Task<bool> InsertAsync(Doctor entity) => AddDoctorAsync(entity);

        public bool Update(Doctor entity) => UpdateDoctor(entity);
        public Task<bool> UpdateAsync(Doctor entity) => UpdateDoctorAsync(entity);

        public bool Delete(int id) => DeleteDoctor(id);
        public Task<bool> DeleteAsync(int id) => DeleteDoctorAsync(id);

        public List<Doctor> Search(string searchTerm) => SearchDoctors(searchTerm);
        public Task<List<Doctor>> SearchAsync(string searchTerm) => SearchDoctorsAsync(searchTerm);

        public List<Doctor> GetAllDoctors()
        {
            if (_doctorRepository is DoctorRepository docRepo)
            {
                return docRepo.GetAllWithSpecialization();
            }
            return _doctorRepository.GetAll();
        }

        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            if (_doctorRepository is DoctorRepository docRepo)
            {
                return await docRepo.GetAllWithSpecializationAsync();
            }
            return await _doctorRepository.GetAllAsync();
        }

        public List<Doctor> SearchDoctors(string searchTerm)
        {
            if (_doctorRepository is DoctorRepository docRepo)
            {
                return docRepo.SearchWithSpecialization(searchTerm);
            }
            return _doctorRepository.Search(searchTerm);
        }

        public async Task<List<Doctor>> SearchDoctorsAsync(string searchTerm)
        {
            if (_doctorRepository is DoctorRepository docRepo)
            {
                return await docRepo.SearchWithSpecializationAsync(searchTerm);
            }
            return await _doctorRepository.SearchAsync(searchTerm);
        }

        public Doctor GetDoctorById(int id)
        {
            return _doctorRepository.GetById(id);
        }

        public async Task<Doctor> GetDoctorByIdAsync(int id)
        {
            return await _doctorRepository.GetByIdAsync(id);
        }

        public bool AddDoctor(Doctor doctor)
        {
            // Business rule: Check if doctor already exists or contact info is unique
            return _doctorRepository.Insert(doctor);
        }

        public async Task<bool> AddDoctorAsync(Doctor doctor)
        {
            return await _doctorRepository.InsertAsync(doctor);
        }

        public bool UpdateDoctor(Doctor doctor)
        {
            return _doctorRepository.Update(doctor);
        }

        public async Task<bool> UpdateDoctorAsync(Doctor doctor)
        {
            return await _doctorRepository.UpdateAsync(doctor);
        }

        public bool DeleteDoctor(int id)
        {
            // Business rule: Check if doctor has pending appointments before deleting
            return _doctorRepository.Delete(id);
        }

        public async Task<bool> DeleteDoctorAsync(int id)
        {
            return await _doctorRepository.DeleteAsync(id);
        }

        public bool IsDoctorAvailable(int doctorId)
        {
            var doctor = _doctorRepository.GetById(doctorId);
            return doctor != null && doctor.IsAvailable;
        }

        public async Task<bool> IsDoctorAvailableAsync(int doctorId)
        {
            var doctor = await _doctorRepository.GetByIdAsync(doctorId);
            return doctor != null && doctor.IsAvailable;
        }

        public List<Doctor> GetDoctorsBySpecialization(int specializationId)
        {
            return _doctorRepository.GetAll()
                .Where(d => d.SpecializationID == specializationId)
                .ToList();
        }
    }
}
