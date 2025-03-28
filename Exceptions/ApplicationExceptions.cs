using System;

namespace CLD111POETerm1ST10484249.Exceptions
{
    public class VenueNotFoundException : Exception
    {
        public VenueNotFoundException(int venueId) 
            : base($"Venue with ID {venueId} was not found.")
        {
        }
    }

    public class EventNotFoundException : Exception
    {
        public EventNotFoundException(int eventId) 
            : base($"Event with ID {eventId} was not found.")
        {
        }
    }

    public class BookingNotFoundException : Exception
    {
        public BookingNotFoundException(int bookingId) 
            : base($"Booking with ID {bookingId} was not found.")
        {
        }
    }

    public class VenueNotAvailableException : Exception
    {
        public VenueNotAvailableException(int venueId, DateTime startDate, DateTime endDate) 
            : base($"Venue with ID {venueId} is not available for the selected dates ({startDate:MM/dd/yyyy} - {endDate:MM/dd/yyyy}).")
        {
        }
    }

    public class InvalidDateRangeException : Exception
    {
        public InvalidDateRangeException() 
            : base("End date must be after start date.")
        {
        }
    }

    public class DatabaseOperationException : Exception
    {
        public DatabaseOperationException(string operation, Exception innerException) 
            : base($"Error performing database operation: {operation}", innerException)
        {
        }
    }
} 