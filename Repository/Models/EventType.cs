using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CLD111POETerm1ST10484249.Repository.Models;

[Table("EventType")]
public partial class EventType
{
    [Key]
    public int TypeId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? TypeName { get; set; }

    [InverseProperty("EventType")]
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
