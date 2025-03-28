using CLD111POETerm1ST10484249.Repository.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CLD111POETerm1ST10484249.Controllers
{
    /// <summary>
    /// Controller for handling user authentication and session management.
    /// Note: This is a demonstration implementation and should not be used in production.
    /// </summary>
    public class UsersController : Controller
    {
        // Demo user data - In production, this should be stored securely in a database
        private static readonly List<User> users = new List<User>
        {
            new User("admin", "admin123", "Admin"),
            new User("john_tim", "password456", "User"),
            new User("manager", "manager789", "Manager")
        };

        /// <summary>
        /// Displays the login page
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Handles user login authentication
        /// </summary>
        /// <param name="Username">The username entered by the user</param>
        /// <param name="Password">The password entered by the user</param>
        /// <returns>Redirects to home page on success, returns to login page with error on failure</returns>
        [HttpPost]
        public IActionResult Login(string Username, string Password)
        {
            // Validate user credentials
            var user = users.FirstOrDefault(u => u.Username == Username && u.Password == Password);

            if (user != null)
            {
                // Store user session data
                HttpContext.Session.SetString("LoggedInUser", JsonConvert.SerializeObject(user));
                return RedirectToAction("Index", "Home");
            }

            // Invalid credentials
            ViewBag.Error = "Invalid Username or Password";
            return View("Index");
        }

        /// <summary>
        /// Handles user logout by clearing the session
        /// </summary>
        /// <returns>Redirects to login page</returns>
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("LoggedInUser");
            return RedirectToAction("Index");
        }
    }
}
