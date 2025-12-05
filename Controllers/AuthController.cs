using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using technova_ecommerce.Models.Entities;

namespace technova_ecommerce.Controllers
{
    public class AuthController : Controller
    {
        private DatabaseContext _db;

        public AuthController(DatabaseContext db)
        {
            _db = db;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            if (ModelState.IsValid)
            {
                if (_db.Users.Any(u => u.UserName == user.UserName))
                {
                    var loggedinUser = await _db.Users.FirstOrDefaultAsync(u => u.UserName.Equals(user.UserName));
                    if (loggedinUser != null && BCrypt.Net.BCrypt.Verify(user.HashedPassword, loggedinUser.HashedPassword))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Error = "Incorrect Password.";
                        return View(user);
                    }
                }
                else
                {
                    ViewBag.Error = "User does not exist.";
                    return View(user);
                }
            }

           
            return View(user);
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (_db.Users.Any(u => u.UserName == user.UserName))
                {
                    ViewBag.Error = "Username already exists. Please try another username.";
                    return View(user);
                }

                
                user.HashedPassword = BCrypt.Net.BCrypt.HashPassword(user.HashedPassword);  
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
                return RedirectToAction("Login", "Auth");
            }

            return View(user);
        }
    }

}
