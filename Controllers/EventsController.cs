using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CLD111POETerm1ST10484249.Models;
using CLD111POETerm1ST10484249.Repository.Models;
using ST1048422449CloudClassApp.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;

namespace CLD111POETerm1ST10484249.Controllers
{
    /// <summary>
    /// Controller for managing events in the system
    /// </summary>
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EventsController> _logger;

        public EventsController(ApplicationDbContext context, IConfiguration configuration, ILogger<EventsController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Displays a list of all events with their associated types and venues
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var eventType = await _context.EventTypes.ToListAsync();
            var events = await _context.Events
                .Include(p => p.EventType)
                .Include(p => p.Venue)
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                eventTypes = eventType,
                events = events
            };

            return View(viewModel);
        }

        /// <summary>
        /// Alternative entry point to display the events list
        /// </summary>
        public async Task<IActionResult> Default()
        {
            return await Index();
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(p => p.EventType)
                .Include(p => p.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "TypeId", "TypeId");
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,VenueId,EventName,EventTypeId,EventStartDate,EventEndDate,Description")] Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "TypeId", "TypeId", @event.EventTypeId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", @event.VenueId);
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "TypeId", "TypeId", @event.EventTypeId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", @event.VenueId);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,VenueId,EventName,EventTypeId,EventStartDate,EventEndDate,Description")] Event @event)
        {
            if (id != @event.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "TypeId", "TypeId", @event.EventTypeId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", @event.VenueId);
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(p => p.EventType)
                .Include(p => p.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        /// <summary>
        /// Deletes a blob from Azure Storage
        /// </summary>
        /// <param name="blobUrl">The URL of the blob to delete</param>
        private async Task DeleteBlobFromAzure(string blobUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(blobUrl))
                    return;

                var sasUrl = _configuration["AzureStorage:SASUrl"];
                var blobServiceClient = new BlobServiceClient(new Uri(sasUrl));
                string containerName = "webfiles";
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                // Extract blob name from URL
                var uri = new Uri(blobUrl);
                var blobName = Path.GetFileName(uri.LocalPath);
                var blobClient = containerClient.GetBlobClient(blobName);

                await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                // Log the error but don't throw - we don't want to prevent the event deletion
                // just because the blob deletion failed
                _logger.LogError($"Error deleting blob from Azure Storage: {ex.Message}");
            }
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                // Delete the event from the database
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }

        // Add this new method to check date availability
        [HttpGet]
        public async Task<IActionResult> CheckDateAvailability(DateTime startDate, DateTime endDate, int? venueId = null)
        {
            var query = _context.Events.AsQueryable();

            if (venueId.HasValue)
            {
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
    }
}
