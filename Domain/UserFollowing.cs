using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    // joint table for self referencing - many to many relationship
    // a user can follow many other users and also can be followed by many others.
    public class UserFollowing
    {
        public string? ObserverId { get; set; }  
        public AppUser? Observer { get; set; }  // who
        public string? TargetId {get; set;}
        public AppUser? Target { get; set; }  // follow whom
    }
}