using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CLD111POETerm1ST10484249.Repository.Models;

public partial class VeneuImage
{
    [Key]
    public int Id { get; set; }

    public int? VenueId { get; set; }

    [Unicode(false)]
    public string? Decription { get; set; }

    [Column("ImageURL")]
    [Unicode(false)]
    public string? ImageUrl { get; set; }

    [ForeignKey("VenueId")]
    [InverseProperty("VeneuImages")]
    public virtual Venue? Venue { get; set; }

    [NotMapped] 
    public IFormFile? ImageFile { get; set; }
}
