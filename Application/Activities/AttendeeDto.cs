using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Activities
{
    // A copy of profile class without the photo collection field - replace Profile in ActivityDto
    public class AttendeeDto
    {
        public string? Username { get; set; }
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
        public string? Image { get; set; }   

        // below is for following - follower 
        public bool Following { get; set; }    // do current user follow the selected user or not - this belongs to the selected user
        public int FollowersCount { get; set; }  // how many user follow this user
        public int FollowingCount { get; set; }  // how many user this user follow
    }
}