using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CLD111POETerm1ST10484249.Repository.Models;

[Table("Booking")]
public partial class Booking
{
    [Key]
    public int BookingId { get; set; }

    public int? EventId { get; set; }

    [Column("VenueID")]
    public int? VenueId { get; set; }

    [StringLength(10)]
    public string? Description { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? BookingDate { get; set; }

    public int? BookingStatusId { get; set; }

    [ForeignKey("BookingStatusId")]
    [InverseProperty("Bookings")]
    public virtual BookingStatus? BookingStatus { get; set; }

    [ForeignKey("EventId")]
    [InverseProperty("Bookings")]
    public virtual Event? Event { get; set; }

    [ForeignKey("VenueId")]
    [InverseProperty("Bookings")]
    public virtual Venue? Venue { get; set; }
}
