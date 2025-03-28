using System;
using System.Collections.Generic;
using CLD111POETerm1ST10484249.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace CLD111POETerm1ST10484249.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingStatus> BookingStatuses { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventImage> EventImages { get; set; }

    public virtual DbSet<EventType> EventTypes { get; set; }

    public virtual DbSet<VeneuImage> VeneuImages { get; set; }

    public virtual DbSet<Venue> Venues { get; set; }

    public virtual DbSet<VenueStatus> VenueStatuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Name=DefaultConnection");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.Property(e => e.Description).IsFixedLength();

            entity.HasOne(d => d.BookingStatus).WithMany(p => p.Bookings).HasConstraintName("FK_Booking_BookingStatus");

            entity.HasOne(d => d.Event).WithMany(p => p.Bookings).HasConstraintName("FK_Booking_Event");

            entity.HasOne(d => d.Venue).WithMany(p => p.Bookings).HasConstraintName("FK_Booking_Venue");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasOne(d => d.EventType).WithMany(p => p.Events).HasConstraintName("FK_Event_EventType");

            entity.HasOne(d => d.Venue).WithMany(p => p.Events)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Event_Venue");
        });

        modelBuilder.Entity<EventImage>(entity =>
        {
            entity.HasOne(d => d.Event).WithMany(p => p.EventImages)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_EventImages_Event");
        });

        modelBuilder.Entity<VeneuImage>(entity =>
        {
            entity.HasOne(d => d.Venue).WithMany(p => p.VeneuImages)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_VeneuImages_Venue");
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasOne(d => d.VenueStatus).WithMany(p => p.Venues).HasConstraintName("FK_Venue_VenueStatus");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
