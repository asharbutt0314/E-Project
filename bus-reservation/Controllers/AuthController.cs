using bus_reservation.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bus_reservation.Controllers
{
   

    public class AuthController : Controller
	{
        private readonly BusReservationContext db;

        public AuthController(BusReservationContext _db)
        {
            db = _db;
        }
        public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Login(Employee user)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.msg = "Invalid data.";
				return View();
			}

			var checkUser = db.Employees.FirstOrDefault(a => a.EmployeeEmail == user.EmployeeEmail);

			if (checkUser != null)
			{
				var hasher = new PasswordHasher<string>();
				var verifyPass = hasher.VerifyHashedPassword(user.EmployeeEmail, checkUser.Password, user.Password);

				if (verifyPass == PasswordVerificationResult.Success)
				{
					var identity = new ClaimsIdentity(new[]
					{
				new Claim(ClaimTypes.Name, checkUser.Username),
				new Claim(ClaimTypes.Role, "User") // Replace "User" with dynamic role if you have roles
            }, CookieAuthenticationDefaults.AuthenticationScheme);

					var principal = new ClaimsPrincipal(identity);

					await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

					HttpContext.Session.SetInt32("UserID", checkUser.EmployeeId);
					HttpContext.Session.SetString("UserEmail", checkUser.EmployeeEmail);

					return RedirectToAction("Index", "Home"); // Redirect to the appropriate controller/action
				}
				else
				{
					ViewBag.msg = "Invalid Credentials";
					return View();
				}
			}
			else
			{
				ViewBag.msg = "Invalid User";
				return View();
			}
		}

	}
}
