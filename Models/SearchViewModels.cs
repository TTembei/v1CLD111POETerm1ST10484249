using CLD111POETerm1ST10484249.Repository.Models;
using System;
using System.Collections.Generic;

namespace CLD111POETerm1ST10484249.Models
{
    public class VenueSearchViewModel
    {
        public string SearchTerm { get; set; }
        public string Location { get; set; }
        public int? MinCapacity { get; set; }
        public int? MaxCapacity { get; set; }
        public string Status { get; set; }
        public List<Venue> Results { get; set; } = new List<Venue>();
        public List<VenueStatus> VenueStatuses { get; set; } = new List<VenueStatus>();
    }

    public class EventSearchViewModel
    {
        public string SearchTerm { get; set; }
        public string EventType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string VenueName { get; set; }
        public List<Event> Results { get; set; } = new List<Event>();
    }

    public class BookingSearchViewModel
    {
        public string SearchTerm { get; set; }
        public string VenueName { get; set; }
        public string EventName { get; set; }
        public DateTime? BookingDateFrom { get; set; }
        public DateTime? BookingDateTo { get; set; }
        public string Status { get; set; }
        public List<Booking> Results { get; set; } = new List<Booking>();
        public List<BookingStatus> BookingStatuses { get; set; } = new List<BookingStatus>();
    }
} 