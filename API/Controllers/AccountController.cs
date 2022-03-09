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

    public AccountController(UserManager<AppUser> userManager, 
                                SignInManager<AppUser> signInManager,
                                TokenService tokenService)
            {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }
        // api/login
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // find the login user by his email
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

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

        // api/register
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // check if the email is already used or not
            if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email)) {
                return BadRequest("Email taken");
            }
            // check if the user name is already in used or not
            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.Username)) {
                return BadRequest("Username taken");
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

        [Authorize]  // user must be authorized to get to this end point
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            // get current user email from token payload - email field
            var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            // return a new user object from this
            return CreateUserObject(user);
        }

        private UserDto CreateUserObject(AppUser user)
        {
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Image = null,
                Token = _tokenService.CreateToken(user),
                Username = user.UserName
            };
        }
    }
}