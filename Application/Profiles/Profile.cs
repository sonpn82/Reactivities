using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace Application.Profiles
{
    // class contain fields in profile list of Attendees in ActivityDto
    public class Profile
    {
        public string? Username { get; set; }
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
        public string? Image { get; set; }    
        public bool Following { get; set; }    // do current user follow the selected user or not - this belongs to the selected user
        public int FollowersCount { get; set; }  // how many user follow this user
        public int FollowingCount { get; set; }  // how many user this user follow
        public ICollection<Photo>? Photos { get; set; }  // List of photo of an attendee
    }
}