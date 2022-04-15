using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.Controllers
{
    [AllowAnonymous]  // allow all end points below to be accessed without authentication
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _config;  // for facebook login
        private readonly HttpClient _httpClient;  // for facebook login

    public AccountController(UserManager<AppUser> userManager, 
                                SignInManager<AppUser> signInManager,
                                TokenService tokenService,
                                IConfiguration config)  // add IConfiguration for facebook login
            {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _config = config;  // for facebook login
        _httpClient = new HttpClient // for facebook login
        {
            BaseAddress = new System.Uri("https://graph.facebook.com")
        };
    }
        // api/account/login
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // find the login user by his email
            // also load the photo collection of user
            var user = await _userManager.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.Email == loginDto.Email);

            // return unauthorized if not found
            if(user == null) return Unauthorized();
            
            // check the login password and let user sign in
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            // if login success, return an UserDto object
            if (result.Succeeded)
            {
                return CreateUserObject(user);
            }

            // else return Unauthorized
            return Unauthorized();
        }

        // api/account/register
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
            // if ok then return the newly created user
            if (result.Succeeded)
            {
                return CreateUserObject(user);
            }
            // else return a bad request
            return BadRequest("Problem registering user");
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
            // return a new user object from this
            return CreateUserObject(user);
        }

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

            // save user to database
            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded) return BadRequest("Problem creating user account");

            return CreateUserObject(user);        
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