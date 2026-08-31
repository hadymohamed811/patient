using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using patient.Models;
using patient.Repositories;
using System.Security.Claims; 

namespace patient.Controllers
{
    [Authorize] 
    public class PatientController : Controller
    {
        private readonly IPatientRepository _patientRepo;

        public PatientController(IPatientRepository patientRepo)
        {
            _patientRepo = patientRepo;
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
    }
}