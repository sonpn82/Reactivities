using System;

namespace Domain
{
    // class contains our data model
    // use by entity framework to generate the datatable in Persistence
    public class Activity
    {
        // properties initialize
        // validation can be done here using data anotation - [required] before a field
        // but validation of data should not happend in domain layer, it should be put in application layer
        public Guid Id { get; set; }  // global unique identifier - be primary key in Activities table in DataContext.cs       
        public string? Title { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? City { get; set; }
        public string? Venue { get; set; }
        public bool IsCancelled { get; set; }  // is the activity cancelled by the holder or not - need to do migration again if add new prop to database
        // Attendees of activity is also in user table
        // a joint table of ActivityAttendee is created to set the many to many relation between Acvitity and User table
        // set initial val is list of attendees to avoid object not refer to instance of object error
        public ICollection<ActivityAttendee>? Attendees { get; set; } = new List<ActivityAttendee>();
    }
}