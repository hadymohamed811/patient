using Microsoft.EntityFrameworkCore;
using patient.Models;

namespace patient.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
        }

        public async Task<Patient?> GetByUserIdAsync(string userId)
        {
            return await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<List<Patient>> GetPatientsByDoctorIdAsync(int doctorId)
        {
            return await _context.Patients
                .Where(p => p.Appointments.Any(a => a.TimeSlot.DoctorId == doctorId))
                .ToListAsync();
        }
    }
}