using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using patient.Models;
using patient.Repositories;
using System.Security.Claims; 

namespace patient.Controllers
{
    [Authorize]
    public class DoctorController : Controller
    {
        private readonly IDoctorRepository _doctorRepo;
        private readonly ApplicationDbContext _context;

        public DoctorController(IDoctorRepository doctorRepo, ApplicationDbContext context)
        {
            _doctorRepo = doctorRepo;
            _context = context;
        }

       
        public async Task<IActionResult> Index()
        {
            var doctors = await _doctorRepo.GetAllAsync();
            return View(doctors);
        }

    
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAdd(Doctor doctorRequest)
        {
            if (ModelState.IsValid)
            {
               
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                doctorRequest.UserId = userId; 

                await _doctorRepo.AddAsync(doctorRequest);
                return RedirectToAction(nameof(Index));
            }
            return View("Add", doctorRequest);
        }

    
        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            return View(doctor);
        }

   
        public async Task<IActionResult> MyPatients(int doctorId)
        {
            var doctor = await _context.Doctors
                .Include(d => d.TimeSlots)
                .ThenInclude(t => t.Appointment)
                .ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

      
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            return View(doctor);
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Doctor doctor)
        {
            if (ModelState.IsValid)
            {
              
                var existingDoctor = await _doctorRepo.GetByIdAsync(doctor.Id);
                if (existingDoctor != null)
                {
                    existingDoctor.Name = doctor.Name;
                    existingDoctor.Specialization = doctor.Specialization;

                    await _doctorRepo.UpdateAsync(existingDoctor);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(doctor);
        }

  
        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _doctorRepo.GetByIdAsync(id);
            if (doctor != null)
            {
                await _doctorRepo.DeleteAsync(doctor);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}