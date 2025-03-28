using System;
using System.Collections.Generic;
using CLD111POETerm1ST10484249.Repository.Models;

namespace CLD111POETerm1ST10484249.Models
{
    public class DashboardViewModel
    {
        public int TotalVenues { get; set; }
        public int TotalEvents { get; set; }
        public int TotalBookings { get; set; }
        public int AvailableVenues { get; set; }
        public List<Event> UpcomingEvents { get; set; }
        public List<Booking> RecentBookings { get; set; }
        public List<VenueUtilizationData> VenueUtilization { get; set; }
    }

    public class VenueUtilizationData
    {
        public string VenueName { get; set; }
        public int TotalEvents { get; set; }
        public double UtilizationRate { get; set; }
    }
} 