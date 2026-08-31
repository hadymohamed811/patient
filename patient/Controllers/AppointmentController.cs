using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using patient.Models;
using patient.Repositories;
using System.Security.Claims;

namespace patient.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly ApplicationDbContext _context;

        public AppointmentController(IDoctorRepository doctorRepo, ApplicationDbContext context)
        {
            _doctorRepo = doctorRepo;
            _context = context;
        }

     
        public async Task<IActionResult> BookAppointment()
        {
            var doctors = await _doctorRepo.GetAllAsync();
            return View(doctors);
        }


        [HttpGet]
        public async Task<IActionResult> Book(int timeSlotId)
        {
            var timeSlot = await _context.TimeSlots
                .Include(t => t.Doctor)
                .FirstOrDefaultAsync(t => t.Id == timeSlotId);

            if (timeSlot == null || timeSlot.IsBooked)
            {
                return NotFound();
            }

            return View(timeSlot);
        }

   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmBooking(int timeSlotId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
            {
                return RedirectToAction("Register", "Patient");
            }

            var timeSlot = await _context.TimeSlots.FindAsync(timeSlotId);

            if (timeSlot == null || timeSlot.IsBooked)
            {
                return NotFound();
            }

            timeSlot.IsBooked = true;

            var appointment = new Appointment
            {
                TimeSlotId = timeSlotId,
                PatientId = patient.Id
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyAppointments");
        }

    
        public async Task<IActionResult> MyAppointments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
            {
                return RedirectToAction("Register", "Patient");
            }

            var appointments = await _context.Appointments
                .Where(a => a.PatientId == patient.Id)
                .Include(a => a.TimeSlot)
                .ThenInclude(t => t.Doctor)
                .Include(a => a.Patient)
                .ToListAsync();

            return View(appointments);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
            {
                return RedirectToAction("Register", "Patient");
            }

       
            var appointment = await _context.Appointments
                .Include(a => a.TimeSlot)
                .FirstOrDefaultAsync(a => a.Id == id && a.PatientId == patient.Id);

            if (appointment == null)
            {
                return NotFound();
            }

            if (appointment.TimeSlot != null)
            {
                appointment.TimeSlot.IsBooked = false;
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyAppointments");
        }

       
        public async Task<IActionResult> DoctorAppointments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var doctorAppointments = await _context.Appointments
                .Include(a => a.TimeSlot)
                .Include(a => a.Patient)
                .Where(a => a.TimeSlot.DoctorId == doctor.Id)
                .ToListAsync();

            return View(doctorAppointments);
        }
    }
}