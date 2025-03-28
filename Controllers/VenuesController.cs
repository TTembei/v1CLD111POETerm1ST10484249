using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CLD111POETerm1ST10484249.Models;
using CLD111POETerm1ST10484249.Repository.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;
using System.Threading.Tasks;
using Humanizer.Localisation;
using ST1048422449CloudClassApp.Models;

namespace CLD111POETerm1ST10484249.Controllers
{
    /// <summary>
    /// Controller for managing venues in the system
    /// </summary>
    public class VenuesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public VenuesController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Displays a list of all venues with their status
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Venues.Include(v => v.VenueStatus);
            return View(await applicationDbContext.ToListAsync());
        }

        /// <summary>
        /// Alternative entry point to display the venues list
        /// </summary>
        public async Task<IActionResult> Default()
        {
            return await Index();
        }

        [HttpPost]
        public async Task<IActionResult> AddVenueImages(int? id, List<IFormFile> ImageFiles)
        {
            if (id == null || ImageFiles == null || !ImageFiles.Any())
            {
                ModelState.AddModelError("", "Please select at least one image.");
                return View();
            }

            List<VeneuImage> uploadedImages = new List<VeneuImage>();

            foreach (var file in ImageFiles)
            {
                if (file.Length > 0)
                {
                    // Upload to Azure Blob Storage
                    string blobUrl = await UploadImageToAzureBlob(file);

                    // Save the image record
                    uploadedImages.Add(new VeneuImage
                    {
                        VenueId = id.Value,
                        ImageUrl = blobUrl
                    });
                }
            }

            // Save the uploaded images to the database (Assuming you have a DbContext)
            _context.VeneuImages.AddRange(uploadedImages);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Images uploaded successfully.";
            return RedirectToAction("Details", new { id });
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .Include(v => v.VenueStatus)
                .FirstOrDefaultAsync(m => m.VenueId == id);

            var veneuImages = await _context.VeneuImages
           .Where(us => us.VenueId == id)
           .ToListAsync();

            var viewModel = new HomeViewModel
            {
                veneuImages = veneuImages,
            };

            if (venue == null)
            {
                return NotFound();
            }

           ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "TypeId", "TypeName");
            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueId,VenueName,Location,Capacity,ImageFile,VenueStatusId,Description")] Venue venue)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (venue.ImageFile != null && venue.ImageFile.Length > 0)
                    {
                        // Upload to Azure Blob Storage
                        string blobUrl = await UploadImageToAzureBlob(venue.ImageFile);
                        venue.ImageUrl = blobUrl;
                    }

                    _context.Add(venue);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Edit), new { id = venue.VenueId });

                }
            }
          catch (Exception ex)
            {

            }

            
            ViewData["VenueStatusId"] = new SelectList(_context.VenueStatuses, "StatusId", "StatusId", venue.VenueStatusId);
            return View(venue);
        }

        public async Task<string> UploadImageToAzureBlob(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Invalid image file");

            //var connectionString = _configuration["AzureStorage:ConnectionString"];
            //var blobServiceClient = new BlobServiceClient(connectionString);

            var sasUrl = _configuration["AzureStorage:SASUrl"];
            var blobServiceClient = new BlobServiceClient(new Uri(sasUrl));

            string containerName = "webfiles"; 

           
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Ensure container exists, but do not set public access if authentication is required
            // await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
            await containerClient.CreateIfNotExistsAsync();

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var blobClient = containerClient.GetBlobClient(fileName);

            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = imageFile.ContentType });
            }
            // Return the blob URL
            return blobClient.Uri.ToString(); 
        }

        public IActionResult Create()
        {
            
            return View();
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var veneuImages = await _context.VeneuImages
                 .Where(us => us.VenueId == id)
                 .ToListAsync();

            var viewModel = new HomeViewModel
            {
              veneuImages = veneuImages,
            };

   
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }
            ViewData["VenueStatusId"] = new SelectList(_context.VenueStatuses, "StatusId", "StatusName", venue.VenueStatusId);
            return View(venue);
        }

        // POST: Venues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,VenueName,Location,Capacity,ImageUrl,VenueStatusId,ImageFile,Description")] Venue venue)
        {
            if (id != venue.VenueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // If an image file is uploaded, handle the upload to Azure Blob Storage
                    if (venue.ImageFile != null && venue.ImageFile.Length > 0)
                    {
                        string blobUrl = await UploadImageToAzureBlob(venue.ImageFile);
                        venue.ImageUrl = blobUrl;
                    }

                    // Update the venue entity
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    // Log or handle the exception as needed
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the changes: " + ex.Message);
                }
            }

            // If ModelState is not valid, or if an error occurred, reload the venue status list and return the view with the model
            ViewData["VenueStatusId"] = new SelectList(_context.VenueStatuses, "StatusId", "StatusId", venue.VenueStatusId);
            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues.Include(v => v.VenueStatus).FirstOrDefaultAsync(m => m.VenueId == id);

            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
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
                // Log the error but don't throw - we don't want to prevent the venue deletion
                // just because the blob deletion failed
                //_logger.LogError($"Error deleting blob from Azure Storage: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes all images associated with a venue
        /// </summary>
        /// <param name="venueId">The ID of the venue</param>
        private async Task DeleteVenueImages(int venueId)
        {
            var images = await _context.VeneuImages
                .Where(vi => vi.VenueId == venueId)
                .ToListAsync();

            foreach (var image in images)
            {
                await DeleteBlobFromAzure(image.ImageUrl);
                _context.VeneuImages.Remove(image);
            }

            await _context.SaveChangesAsync();
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);

            if (venue == null)
            {
                return NotFound();
            }

            // Only delete when status is Available
            if (venue.VenueStatusId == 1)
            {
                // Delete the venue's main image from Azure Storage
                await DeleteBlobFromAzure(venue.ImageUrl);

                // Delete all additional venue images
                await DeleteVenueImages(id);

                // Delete the venue from the database
                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueId == id);
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

                var venue = await _context.Venues
                    .Include(v => v.VenueStatus)
                    .FirstOrDefaultAsync(v => v.VenueId == request.VenueId);

                if (venue == null)
                {
                    return Json(new { success = false, message = "Venue not found" });
                }

                var newStatus = await _context.VenueStatuses.FindAsync(request.StatusId);
                if (newStatus == null)
                {
                    return Json(new { success = false, message = "Invalid status" });
                }

                venue.VenueStatusId = request.StatusId;
                _context.Entry(venue).Property(x => x.VenueStatusId).IsModified = true;
                
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
            public int VenueId { get; set; }
            public int StatusId { get; set; }
        }
    }
}
