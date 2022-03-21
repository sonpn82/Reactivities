using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Photos
{
    // class to contains 2 field from file uploaded to cloudinary: PublicId and Url
    public class PhotoUploadResult
    {
        public string? PublicId { get; set; }
        public string? Url { get; set; }
    }
}