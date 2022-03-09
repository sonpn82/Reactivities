using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required] // this field is required
        public string DisplayName { get; set; }
        [Required] // this field is required
        [EmailAddress] // this field must be type of email address (abc@xyz)
        public string Email { get; set; }
        [Required]
        [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$", ErrorMessage = "Password must be complex")] // password with 1 number 1 lower case 1 upper case between 4-8 character long
        public string Password { get; set; }
        [Required]
        public string Username { get; set; }
    }
}