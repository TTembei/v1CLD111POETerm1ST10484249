using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CLD111POETerm1ST10484249.Repository.Models;

public partial class EventImage
{
    [Key]
    public int Id { get; set; }

    public int? EventId { get; set; }

    [Unicode(false)]
    public string? Decription { get; set; }

    [Column("ImageURL")]
    [Unicode(false)]
    public string? ImageUrl { get; set; }

    [ForeignKey("EventId")]
    [InverseProperty("EventImages")]
    public virtual Event? Event { get; set; }
}
