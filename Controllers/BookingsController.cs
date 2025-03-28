using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CLD111POETerm1ST10484249.Repository.Models;
using CLD111POETerm1ST10484249.Models;
using Newtonsoft.Json;
using System.Text;
using System.Data;
using Humanizer.Localisation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CLD111POETerm1ST10484249.Controllers
{
    /// <summary>
    /// Controller for managing event bookings in the system
    /// </summary>
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserSessionHelper _userSessionHelper;

        public BookingsController(ApplicationDbContext context, UserSessionHelper userSessionHelper)
        {
            _context = context;
            _userSessionHelper = userSessionHelper;
        }

        /// <summary>
        /// Displays a list of all bookings with their associated events and venues
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Bookings
                .Include(b => b.BookingStatus)
                .Include(b => b.Event)
                .Include(b => b.Venue);

            ViewBag.BookingStatuses = await _context.BookingStatuses.ToListAsync();
            return View(await applicationDbContext.ToListAsync());
        }

        /// <summary>
        /// Alternative entry point to display the bookings list
        /// </summary>
        public async Task<IActionResult> Default()
        {
            return await Index();
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Venue)
                .Include(b => b.Event)
                .Include(b => b.BookingStatus)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Load data for dropdowns
            ViewBag.BookingStatuses = await _context.BookingStatuses.ToListAsync();
            ViewBag.Events = await _context.Events.ToListAsync();

            return View(booking);
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,EventId,VenueId,Description,BookingDate,BookingStatusId")] Booking booking,
         DateTime EventStartDate, DateTime EventEndDate, int EventTypeId, string EventName)
        {

            if (ModelState.IsValid)
            {
                // Create and save the Event first
                Event @event = new Event
                {
                    EventStartDate = EventStartDate,
                    EventEndDate = EventEndDate,
                    Description = booking.Description,
                    EventTypeId = EventTypeId,
                    VenueId = booking.VenueId,
                    EventName = EventName
                };

                _context.Events.Add(@event);
                await _context.SaveChangesAsync();

                booking.BookingStatusId = 1;
                booking.EventId = @event.EventId;

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                var user = _userSessionHelper.GetLoggedInUser();

                return user != null && user.Role == "Admin"
                    ? RedirectToAction(nameof(Edit), new { id = booking.BookingId })
                    : RedirectToAction(nameof(Details), new { id = booking.BookingId });
            }

            ViewData["BookingStatusId"] = new SelectList(_context.BookingStatuses, "StatusId", "StatusId", booking.BookingStatusId);
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", booking.VenueId);

            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["BookingStatusId"] = new SelectList(_context.BookingStatuses, "StatusId", "StatusName", booking.BookingStatusId);
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", booking.VenueId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,VenueId,EventId,Description,BookingDate,StatusId")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return Json(new { success = false, message = "Invalid booking ID" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
                    {
                        return Json(new { success = false, message = "Booking not found" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "An error occurred while updating the booking" });
                    }
                }
            }

            return Json(new { success = false, message = "Invalid booking data" });
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.BookingStatus)
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            try
            {
                if (request == null)
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }

                var booking = await _context.Bookings
                    .Include(b => b.BookingStatus)
                    .Include(b => b.Event)
                    .Include(b => b.Venue)
                    .FirstOrDefaultAsync(b => b.BookingId == request.BookingId);

                if (booking == null)
                {
                    return Json(new { success = false, message = "Booking not found" });
                }

                var newStatus = await _context.BookingStatuses.FindAsync(request.StatusId);
                if (newStatus == null)
                {
                    return Json(new { success = false, message = "Invalid status" });
                }

                booking.BookingStatusId = request.StatusId;
                _context.Entry(booking).Property(x => x.BookingStatusId).IsModified = true;
                
                var result = await _context.SaveChangesAsync();
                
                if (result > 0)
                {
                    return Json(new { 
                        success = true,
                        statusName = newStatus.StatusName
                    });
                }
                else
                {
                    return Json(new { 
                        success = false, 
                        message = "No changes were saved" 
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = "An error occurred while updating the status: " + ex.Message 
                });
            }
        }

        public class UpdateStatusRequest
        {
            public int BookingId { get; set; }
            public int StatusId { get; set; }
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
