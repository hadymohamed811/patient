using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using patient.Models;

namespace patient.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<IdentityUser> userManager,
                                 SignInManager<IdentityUser> signInManager,
                                 RoleManager<IdentityRole> roleManager,
                                 ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password)
        {
            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("SelectRole", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult SelectRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SelectRole(string role)
        {
          
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

     
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                }

             
                await _userManager.AddToRoleAsync(user, role);

             
                await _signInManager.RefreshSignInAsync(user);
            }

            if (role == "Doctor")
            {
                return RedirectToAction("Add", "Doctor");
            }
            else if (role == "Patient")
            {
                return RedirectToAction("Register", "Patient");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                
                    if (await _userManager.IsInRoleAsync(user, "Doctor"))
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    if (await _userManager.IsInRoleAsync(user, "Patient"))
                    {
                        return RedirectToAction("BookAppointment", "Appointment");
                    }

            
                    var isDoctor = await _context.Doctors.AnyAsync(d => d.UserId == user.Id);
                    if (isDoctor)
                    {
                        if (!await _roleManager.RoleExistsAsync("Doctor"))
                        {
                            await _roleManager.CreateAsync(new IdentityRole("Doctor"));
                        }

                        if (!await _userManager.IsInRoleAsync(user, "Doctor"))
                        {
                            await _userManager.AddToRoleAsync(user, "Doctor");
                        }

                        return RedirectToAction("Index", "Home");
                    }

              
                    var isPatient = await _context.Patients.AnyAsync(p => p.UserId == user.Id);
                    if (isPatient)
                    {
                        if (!await _roleManager.RoleExistsAsync("Patient"))
                        {
                            await _roleManager.CreateAsync(new IdentityRole("Patient"));
                        }

                        if (!await _userManager.IsInRoleAsync(user, "Patient"))
                        {
                            await _userManager.AddToRoleAsync(user, "Patient");
                        }

                        return RedirectToAction("BookAppointment", "Appointment");
                    }

                    return RedirectToAction("SelectRole", "Account");
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "فشل تسجيل الدخول، تأكد من البريد الإلكتروني أو كلمة المرور.");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}