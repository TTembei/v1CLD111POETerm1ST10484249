using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CLD111POETerm1ST10484249.Repository.Models;

[Table("Event")]
public partial class Event
{
    [Key]
    public int EventId { get; set; }

    public int? VenueId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? EventName { get; set; }

    public int? EventTypeId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EventStartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EventEndDate { get; set; }

    [Unicode(false)]
    public string? Description { get; set; }

    [InverseProperty("Event")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [InverseProperty("Event")]
    public virtual ICollection<EventImage> EventImages { get; set; } = new List<EventImage>();

    [ForeignKey("EventTypeId")]
    [InverseProperty("Events")]
    public virtual EventType? EventType { get; set; }

    [ForeignKey("VenueId")]
    [InverseProperty("Events")]
    public virtual Venue? Venue { get; set; }
}
