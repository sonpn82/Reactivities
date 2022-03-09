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
    }
}