using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CLD111POETerm1ST10484249.Repository.Models;

[Table("BookingStatus")]
public partial class BookingStatus
{
    [Key]
    public int StatusId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? StatusName { get; set; }

    [InverseProperty("BookingStatus")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
