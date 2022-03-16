using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Profiles;

namespace Application.Activities
{
    // a copy of Activity class without the Icollection<Attendees> prop to avoid infinite loop error
    // because Attendees also contain activity props which contain attendees ...
    // use the Icollection<profile> instead, which only contain basic user info, not the activity
    public class ActivityDto
    {
        public Guid Id { get; set; }  // global unique identifier - be primary key in Activities table in DataContext.cs       
        public string? Title { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? City { get; set; }
        public string? Venue { get; set; }
        public bool IsCancelled { get; set; }  

        // additional prop compare with the base Activity class
        public string? HostUsername { get; set; }  // to know who is the host of the event
        public ICollection<Profile>? Attendees { get; set; }  // to avoid infinite loop if using ActivityAttendees
    }
}