using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class AppUser : IdentityUser  // derived from AspNetCor Identity class to get all necessary props
    {
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
        // User goes to Activities or host Activities which is saved in Activity table
        // ActivityAttendee is the joint table between Activity table and User table for many to many relationship
        public ICollection<ActivityAttendee>? Activities { get; set; }
        // one to many relationship - one Appuser has many photos
        public ICollection<Photo>? Photos {get; set;}
    }
}