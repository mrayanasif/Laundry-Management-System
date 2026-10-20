using Microsoft.AspNetCore.Mvc;
using Laundry.Models;
using Microsoft.AspNetCore.Http; 
using System.Linq;

namespace Laundry.Controllers
{
    public class AccountController : Controller
    {
        private readonly LaundryContext _context;

        public AccountController(LaundryContext context)
        {
            _context = context;
        }

        // 1. GET: yeh method hum use kartai hain kai jab ham nai kisi page ko call karna hoa data read karnai kai liyai or page kholnai kai liyai use hota hia 
        public IActionResult Register()
        {
            return View();
        }

        // 2. POST:Yeh hum tb use kartay haim jab ham apna data likhein or isko kahtay kai jaakr yehb data database mein store karlai jab hum
        // submit ka button daaba dein 
        [HttpPost]
        public IActionResult Register(User user)
        {
            ModelState.Remove("Role");
            ModelState.Remove("ConfirmPassword");

            if (!ModelState.IsValid)
            {
                return View(user); 
            }

            // 2. Duplicate Email Check
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
            {
                ViewBag.Error = "This email is already registered";
                return View(user);
            }

            
            int totalUsers = _context.Users.Count();
            if (totalUsers == 0)
            {
                user.Role = "Admin";
            }
            else
            {
                user.Role = "Customer";
            }

            // 4. Save
            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        // Login Credential
        public IActionResult Login()
        {
            return View();
        }

       
        [HttpPost]
        public IActionResult Login(string Email, string Password)
        {
          
            var user = _context.Users.FirstOrDefault(u => u.Email == Email && u.Password == Password);

            if (user != null)
            {
                HttpContext.Session.SetString("UserID", user.UserId.ToString());
                HttpContext.Session.SetString("UserName", user.FullName);
                HttpContext.Session.SetString("UserRole", user.Role);

                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.Error = "Invalid Email or Password";
            return View();
        }

        // Session End
        public IActionResult Logout()
        {
          
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("UserID");
            return RedirectToAction("Index", "Home");
        }
    }
}