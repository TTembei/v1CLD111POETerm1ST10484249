using CLD111POETerm1ST10484249.Repository.Models;

namespace ST1048422449CloudClassApp.Models
{
    public class HomeViewModel
    {
        //public AspNetUser User { get; set; }
        public IEnumerable<Event> events { get; set; }
        public IEnumerable<Venue> venues { get; set; }
        public IEnumerable<Booking> bookings { get; set; }
        public IEnumerable<VeneuImage> veneuImages { get; set; }
        public IEnumerable<EventType> eventTypes { get; set; }
    }
} 