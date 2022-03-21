using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Photos
{
    // class contains 3 main fields to access cloudinary account
    // Use for type setting in PhotoAccessor
    // Cloudinary account info saved in AppSettings.json
    public class CloudinarySettings
    {
        public string? CloudName { get; set; }
        public string? ApiKey { get; set; }
        public string? ApiSecret { get; set; }
    }
}