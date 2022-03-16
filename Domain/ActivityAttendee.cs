using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    // joint table between Activity & Attendee
    // many to many relationship
    public class ActivityAttendee
    {
        // contain Id and User object of User table - a standard for many to many relation ship
        public string? AppUserId { get; set; }   
        public AppUser? AppUser { get; set; }
        // contain Id and Activity object of Activity table - a standard for many to many relation ship
        public Guid ActivityId { get; set; }
        public Activity? Activity { get; set; }
        // extra prop of IsHost to show if the attendee is host of the activity
        public bool IsHost { get; set; }
    }
}