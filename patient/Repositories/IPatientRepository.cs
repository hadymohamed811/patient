using patient.Models;

namespace patient.Repositories
{
    public interface IPatientRepository
    {
        Task AddAsync(Patient patient);
        Task<Patient?> GetByUserIdAsync(string userId);
        Task<List<Patient>> GetPatientsByDoctorIdAsync(int doctorId);
    }
}