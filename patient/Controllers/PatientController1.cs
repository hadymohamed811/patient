using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using patient.Models;
using patient.Repositories;
using System.Security.Claims;

namespace patient.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientController : Controller
    {
        private readonly IPatientRepository _patientRepo;
        private readonly ApplicationDbContext _context;

        public PatientController(IPatientRepository patientRepo, ApplicationDbContext context)
        {
            _patientRepo = patientRepo;
            _context = context;
        }

       
        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == currentUserId);

            if (patient == null)
            {
                return RedirectToAction(nameof(Register));
            }

            return View("Details", patient);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRegister(Patient patientRequest)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                patientRequest.UserId = userId;

                await _patientRepo.AddAsync(patientRequest);
                return RedirectToAction("Index", "Home");
            }
            return View("Register", patientRequest);
        }

        public async Task<IActionResult> Details(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

      
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (patient.UserId != currentUserId)
            {
                return Forbid();
            }

            return View(patient);
        }

    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Patient patient)
        {
            if (ModelState.IsValid)
            {
                var existingPatient = await _context.Patients.FindAsync(patient.Id);
                if (existingPatient == null)
                {
                    return NotFound();
                }

                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (existingPatient.UserId != currentUserId)
                {
                    return Forbid();
                }

             
                existingPatient.Name = patient.Name;
                existingPatient.Phone = patient.Phone;
                existingPatient.Email = patient.Email;

                _context.Patients.Update(existingPatient);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(MyProfile));
            }
            return View(patient);
        }
    }
}