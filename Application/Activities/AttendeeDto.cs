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
    }
}