using CLD111POETerm1ST10484249.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ST1048422449CloudClassApp.Models;
using System.Diagnostics;

namespace CLD111POETerm1ST10484249.Controllers
{
    /// <summary>
    /// Controller for handling the main pages of the application
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Displays the home page with venue images and event/venue selection options
        /// </summary>
        public async Task<IActionResult> IndexAsync()
        {
            var veneuImages = await _context.VeneuImages.ToListAsync();

            var viewModel = new HomeViewModel
            {
                veneuImages = veneuImages,
            };

            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Name");
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "Name");
            return View(viewModel);
        }

        /// <summary>
        /// Displays the privacy policy page
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Displays the error page
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
