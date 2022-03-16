using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService
    {
    private readonly IConfiguration _config;
    public TokenService(IConfiguration config)
    {
      _config = config;
    }

    // create the token in the JWT 3 parts: header, payload and signature
    public string CreateToken(AppUser user)
        {
            // claim in the payload part
            // contain necessary info about user so later we can access it easily
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // a security key to encode and decode the token - TokenKey in appsettings.development.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"]));
            // algorithm to encrypt the key in token
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            // more info on token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // show in payload
                Expires = DateTime.Now.AddDays(7),  // show in payload
                SigningCredentials = creds  // show in header 
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}