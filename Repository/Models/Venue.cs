using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CLD111POETerm1ST10484249.Repository.Models;

[Table("Venue")]
public partial class Venue
{
    [Key]
    public int VenueId { get; set; }

    
    [Unicode(false)]
    public string? Description { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? VenueName { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Location { get; set; }

    public int? Capacity { get; set; }

    [Unicode(false)]
    public string? ImageUrl { get; set; }

    [NotMapped] // Prevents EF from mapping this to the database
    public IFormFile? ImageFile { get; set; }

    public int? VenueStatusId { get; set; }

    [InverseProperty("Venue")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [InverseProperty("Venue")]
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    [InverseProperty("Venue")]
    public virtual ICollection<VeneuImage> VeneuImages { get; set; } = new List<VeneuImage>();

    [ForeignKey("VenueStatusId")]
    [InverseProperty("Venues")]
    public virtual VenueStatus? VenueStatus { get; set; }
}
