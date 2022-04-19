using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.DTOs;
using API.Services;
using Domain;
using Infrastructure.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.Controllers
{
   // [AllowAnonymous]  // allow all end points below to be accessed without authentication - remove after app finish
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _config;  // for facebook login
        private readonly EmailSender _emailSender;
        private readonly HttpClient _httpClient;  // for facebook login

    public AccountController(UserManager<AppUser> userManager, 
                                SignInManager<AppUser> signInManager,
                                TokenService tokenService,
                                IConfiguration config,
                                EmailSender emailSender)  // add IConfiguration for facebook login - add emailSender for email ver
            {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _config = config;  // for facebook login
        _emailSender = emailSender;  // for email verification
        _httpClient = new HttpClient // for facebook login
        {
            BaseAddress = new System.Uri("https://graph.facebook.com")
        };
    }

        // api/account/login
        [AllowAnonymous]  // add after app finish
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // find the login user by his email
            // also load the photo collection of user
            var user = await _userManager.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.Email == loginDto.Email);

            // return unauthorized if not found
            if(user == null) return Unauthorized("Invalid email"); // add Invalid email after app finish for email verify
            
            // after app finish: to allow bob to login without email verify
            if (user.UserName == "bob") user.EmailConfirmed = true;

            // after app finish - for email verification
            if (!user.EmailConfirmed) return Unauthorized("Email not confirmed");

            // check the login password and let user sign in
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            // if login success, return an UserDto object
            if (result.Succeeded)
            {
                // after app finsh to save token and cookie
                await SetRefreshToken(user);

                return CreateUserObject(user);
            }

            // else return Unauthorized
            return Unauthorized("Invalid password");
        }

        // api/account/register
        [AllowAnonymous]  // add after app finish
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // check if the email is already used or not
            if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email)) {
                ModelState.AddModelError("email", "Email taken");
                // return an error object, not just a string to avoid getting error in Register form API
                return ValidationProblem();  
            }
            // check if the user name is already in used or not
            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.Username)) {
                ModelState.AddModelError("username", "Username taken");
                // return an error object, not just a string to avoid getting error in Register form API
                return ValidationProblem();
            }
            // if ok, create a new user with register info
            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Username
            };
            // input user data to database
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            
            // add after app finish - for email verify
            if (!result.Succeeded) return BadRequest("Problem registering user");

            var origin = Request.Headers["origin"];
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var verifyUrl = $"{origin}/account/verifyEmail?token={token}&email={user.Email}";
            var message = $"<p>Please click the below link to verify your email address:</p><p><a href='{verifyUrl}'>Click to verify email</a></p>";

            await _emailSender.SendEmailAsync(user.Email, "Please verify email", message);

            return Ok("Registration success - please verify email");
            // remove below code for email verification - after app finish
            // if ok then return the newly created user
            // if (result.Succeeded)
            // {
            //     // after app finsh to save token and cookie
            //     await SetRefreshToken(user);

            //     return CreateUserObject(user);
            // }
            // // else return a bad request
            // return BadRequest("Problem registering user");
        }

        // After app finish - end point to verify email
        [AllowAnonymous]
        [HttpPost("verifyEmail")]
        public async Task<IActionResult> VerifyEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Unauthorized();

            var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded) return BadRequest("Could not verify email address");

            return Ok("Email confirmed - you can now login");
        }

        // after app finish - to resend email verification link
        [AllowAnonymous]
        [HttpGet("resendEmailConfirmationLink")]
        public async Task<IActionResult> ResendEmailConfirmationLink(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return Unauthorized();

            var origin = Request.Headers["origin"];
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var verifyUrl = $"{origin}/account/verifyEmail?token={token}&email={user.Email}";
            var message = $"<p>Please click the below link to verify your email address:</p><p><a href='{verifyUrl}'>Click to verify email</a></p>";

            await _emailSender.SendEmailAsync(user.Email, "Please verify email", message);

            return Ok("Email verification resent");
        }

        // /api/account
        [Authorize]  // user must be authorized to get to this end point
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            // get current user email from token payload - email field
            // also load the user photo collection
            var user = await _userManager.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(x=> x.Email == User.FindFirstValue(ClaimTypes.Email));
            
            // set the cookie and save token - after app finish
            await SetRefreshToken(user);
            
            // return a new user object from this
            return CreateUserObject(user);
        }

        [AllowAnonymous]  // add after app finish
        [HttpPost("fbLogin")]
        public async Task<ActionResult<UserDto>> FacebookLogin(string accessToken)
        {
            var fbVerifyKeys = _config["Facebook:AppId"] + "|" + _config["Facebook:AppSecret"];
            var verifyToken = await _httpClient
                .GetAsync($"debug_token?input_token={accessToken}&access_token={fbVerifyKeys}");

            if (!verifyToken.IsSuccessStatusCode) return Unauthorized();

            var fbUrl = $"me?access_token={accessToken}&fields=name,email,picture.width(100).height(100)";

            var response = await _httpClient.GetAsync(fbUrl);

            if(!verifyToken.IsSuccessStatusCode) return Unauthorized();          

            var fbInfo = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

            var username = (string)fbInfo.id;

            var user = await _userManager.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.UserName == username);

            // if existing user then just return the user
            if (user != null) return CreateUserObject(user);

            // if not then create a new user
            user = new AppUser
            {
                DisplayName = (string)fbInfo.name,  // need to convert to string for dynamic object
                Email = (string)fbInfo.email,
                UserName = (string)fbInfo.id,
                Photos = new List<Photo>{
                    new Photo{
                        Id = "fb_" + (string)fbInfo.id,
                        Url = (string)fbInfo.picture.data.url,
                        IsMain = true
                    }
                }
            };

            // add for email verification
            user.EmailConfirmed = true;

            // save user to database
            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded) return BadRequest("Problem creating user account");

            // add after app finish
            // to set the token to cookie & to database
            await SetRefreshToken(user);
            return CreateUserObject(user);        
        }

        [Authorize]
        [HttpPost("refreshToken")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            // get token from Cookies
            var refreshToken = Request.Cookies["refreshToken"];

            // find the user (with token) by the username in claim
            var user = await _userManager.Users
                .Include(r => r.RefreshTokens)
                .Include(p => p.Photos)           
                .FirstOrDefaultAsync(x => x.UserName == User.FindFirstValue(ClaimTypes.Name));

            if (user == null) return Unauthorized();

            // get the token from database, check if it is still active or not
            var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

            if (oldToken != null && !oldToken.IsActive) return Unauthorized();

            // if ok then continue
            return CreateUserObject(user);
        }


        // save the token to cookie and database
        // after app finish
        private async Task SetRefreshToken(AppUser user)
        {
            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,  // not allow jsscript to access token
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }

        // create UserDto from AppUser
        private UserDto CreateUserObject(AppUser user)
        {
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Image = user?.Photos?.FirstOrDefault(x =>x.IsMain)?.Url, // image is main photo of AppUser
                Token = _tokenService.CreateToken(user),
                Username = user.UserName
            };
        }
    }
}