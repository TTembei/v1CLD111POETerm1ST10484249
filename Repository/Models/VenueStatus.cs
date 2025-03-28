using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CLD111POETerm1ST10484249.Repository.Models;

[Table("VenueStatus")]
public partial class VenueStatus
{
    [Key]
    public int StatusId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? StatusName { get; set; }

    [InverseProperty("VenueStatus")]
    public virtual ICollection<Venue> Venues { get; set; } = new List<Venue>();
}
