using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CLD111POETerm1ST10484249.Repository.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using CLD111POETerm1ST10484249.Models;
using CLD111POETerm1ST10484249.Exceptions;
using Microsoft.Extensions.Logging;

namespace CLD111POETerm1ST10484249.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboardData = new DashboardViewModel
                {
                    TotalVenues = await _context.Venues.CountAsync(),
                    TotalEvents = await _context.Events.CountAsync(),
                    TotalBookings = await _context.Bookings.CountAsync(),
                    AvailableVenues = await _context.Venues.CountAsync(v => v.VenueStatus.StatusName == "Available"),
                    UpcomingEvents = await _context.Events
                        .Where(e => e.EventStartDate > DateTime.Now)
                        .OrderBy(e => e.EventStartDate)
                        .Take(5)
                        .ToListAsync(),
                    RecentBookings = await _context.Bookings
                        .Include(b => b.Venue)
                        .Include(b => b.Event)
                        .Include(b => b.BookingStatus)
                        .OrderByDescending(b => b.BookingDate)
                        .Take(5)
                        .ToListAsync(),
                    VenueUtilization = await _context.Venues
                        .Select(v => new VenueUtilizationData
                        {
                            VenueName = v.VenueName,
                            TotalEvents = v.Events.Count,
                            UtilizationRate = (double)v.Events.Count / 30
                        })
                        .ToListAsync()
                };

                return View(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                throw new DatabaseOperationException("Loading dashboard data", ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckDateAvailability(DateTime startDate, DateTime endDate, int? venueId = null)
        {
            try
            {
                if (endDate <= startDate)
                {
                    throw new InvalidDateRangeException();
                }

                var query = _context.Events.AsQueryable();

                if (venueId.HasValue)
                {
                    var venue = await _context.Venues.FindAsync(venueId.Value);
                    if (venue == null)
                    {
                        throw new VenueNotFoundException(venueId.Value);
                    }
                    query = query.Where(e => e.VenueId == venueId.Value);
                }

                var conflictingEvents = await query
                    .Where(e => 
                        (startDate >= e.EventStartDate && startDate < e.EventEndDate) ||
                        (endDate > e.EventStartDate && endDate <= e.EventEndDate) ||
                        (startDate <= e.EventStartDate && endDate >= e.EventEndDate))
                    .ToListAsync();

                return Json(new { 
                    isAvailable = !conflictingEvents.Any(),
                    conflictingEvents = conflictingEvents.Select(e => new { 
                        e.EventName, 
                        e.EventStartDate, 
                        e.EventEndDate 
                    })
                });
            }
            catch (InvalidDateRangeException)
            {
                return Json(new { 
                    isAvailable = false,
                    error = "End date must be after start date."
                });
            }
            catch (VenueNotFoundException ex)
            {
                return Json(new { 
                    isAvailable = false,
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking date availability");
                return Json(new { 
                    isAvailable = false,
                    error = "An error occurred while checking date availability."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchVenues(VenueSearchViewModel model)
        {
            try
            {
                var query = _context.Venues
                    .Include(v => v.VenueStatus)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(model.SearchTerm))
                {
                    query = query.Where(v => v.VenueName.Contains(model.SearchTerm) || 
                                           v.Description.Contains(model.SearchTerm));
                }

                if (!string.IsNullOrWhiteSpace(model.Location))
                {
                    query = query.Where(v => v.Location.Contains(model.Location));
                }

                if (model.MinCapacity.HasValue)
                {
                    query = query.Where(v => v.Capacity >= model.MinCapacity.Value);
                }

                if (model.MaxCapacity.HasValue)
                {
                    query = query.Where(v => v.Capacity <= model.MaxCapacity.Value);
                }

                if (!string.IsNullOrWhiteSpace(model.Status))
                {
                    query = query.Where(v => v.VenueStatus.StatusName == model.Status);
                }

                model.Results = await query.ToListAsync();
                model.VenueStatuses = await _context.VenueStatuses.ToListAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching venues");
                throw new DatabaseOperationException("Searching venues", ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchEvents(EventSearchViewModel model)
        {
            try
            {
                var query = _context.Events
                    .Include(e => e.EventType)
                    .Include(e => e.Venue)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(model.SearchTerm))
                {
                    query = query.Where(e => e.EventName.Contains(model.SearchTerm) || 
                                           e.Description.Contains(model.SearchTerm));
                }

                if (!string.IsNullOrWhiteSpace(model.EventType))
                {
                    query = query.Where(e => e.EventType.TypeName == model.EventType);
                }

                if (model.StartDate.HasValue)
                {
                    query = query.Where(e => e.EventStartDate >= model.StartDate.Value);
                }

                if (model.EndDate.HasValue)
                {
                    query = query.Where(e => e.EventEndDate <= model.EndDate.Value);
                }

                if (!string.IsNullOrWhiteSpace(model.VenueName))
                {
                    query = query.Where(e => e.Venue.VenueName.Contains(model.VenueName));
                }

                model.Results = await query.ToListAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching events");
                throw new DatabaseOperationException("Searching events", ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchBookings(BookingSearchViewModel model)
        {
            try
            {
                var query = _context.Bookings
                    .Include(b => b.Venue)
                    .Include(b => b.Event)
                    .Include(b => b.BookingStatus)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(model.SearchTerm))
                {
                    query = query.Where(b => b.Event.EventName.Contains(model.SearchTerm) || 
                                           b.Venue.VenueName.Contains(model.SearchTerm));
                }

                if (!string.IsNullOrWhiteSpace(model.VenueName))
                {
                    query = query.Where(b => b.Venue.VenueName.Contains(model.VenueName));
                }

                if (!string.IsNullOrWhiteSpace(model.EventName))
                {
                    query = query.Where(b => b.Event.EventName.Contains(model.EventName));
                }

                if (model.BookingDateFrom.HasValue)
                {
                    query = query.Where(b => b.BookingDate >= model.BookingDateFrom.Value);
                }

                if (model.BookingDateTo.HasValue)
                {
                    query = query.Where(b => b.BookingDate <= model.BookingDateTo.Value);
                }

                if (!string.IsNullOrWhiteSpace(model.Status))
                {
                    query = query.Where(b => b.BookingStatus.StatusName == model.Status);
                }

                model.Results = await query.ToListAsync();
                model.BookingStatuses = await _context.BookingStatuses.ToListAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching bookings");
                throw new DatabaseOperationException("Searching bookings", ex);
            }
        }
    }
}
