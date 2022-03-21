using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    // class to contain fields in AppUser photo collection
    // Id and Url from Cloudinary when image is uploaded
    // IsMain to show if the photo is user main photo (only allow 1 main photo)
    public class Photo
    {
        public string? Id { get; set; }  
        public string? Url { get; set; }
        public bool IsMain { get; set; }
    }
}