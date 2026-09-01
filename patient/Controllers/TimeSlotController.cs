using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using patient.Models;

namespace patient.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class TimeSlotController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TimeSlotController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var userId = _userManager.GetUserId(User);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null)
            {
                return RedirectToAction("SelectRole", "Account");
            }

            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(TimeSlot timeSlot)
        {
            var userId = _userManager.GetUserId(User);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null)
            {
                return RedirectToAction("SelectRole", "Account");
            }

         
            timeSlot.DoctorId = doctor.Id;
            timeSlot.IsBooked = false;

           
            if (ModelState.IsValid)
            {
                _context.TimeSlots.Add(timeSlot);
                await _context.SaveChangesAsync();

             
                return RedirectToAction("DoctorAppointments", "Appointment");
            }

            return View(timeSlot);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor == null) return RedirectToAction("SelectRole", "Account");

            var timeSlot = await _context.TimeSlots.FindAsync(id);

        
            if (timeSlot == null || timeSlot.IsBooked || timeSlot.DoctorId != doctor.Id)
            {
                return NotFound();
            }

            return View(timeSlot);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TimeSlot timeSlot)
        {
            var userId = _userManager.GetUserId(User);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor == null) return RedirectToAction("SelectRole", "Account");

            var existingSlot = await _context.TimeSlots.FindAsync(timeSlot.Id);

          
            if (existingSlot == null || existingSlot.IsBooked || existingSlot.DoctorId != doctor.Id)
            {
                return NotFound();
            }

      
            existingSlot.SlotDate = timeSlot.SlotDate;

            _context.TimeSlots.Update(existingSlot);
            await _context.SaveChangesAsync();

            return RedirectToAction("DoctorAppointments", "Appointment");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor == null) return RedirectToAction("SelectRole", "Account");

            var timeSlot = await _context.TimeSlots.FindAsync(id);

            
            if (timeSlot != null && !timeSlot.IsBooked && timeSlot.DoctorId == doctor.Id)
            {
                _context.TimeSlots.Remove(timeSlot);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("DoctorAppointments", "Appointment");
        }
    }
}