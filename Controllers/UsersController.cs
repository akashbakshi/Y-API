using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YApi.Data;
using YApi.DTOs;
using YApi.Models;
using YApi.Services;

namespace YApi.Controllers;


[ApiController]
[Route("/api/v1/[controller]")]
public class UsersController: Controller
{
    private YDbContext _dbContext;
    private UserManager<AppUser> _userManager;
    private SignInManager<AppUser> _signInManager;
    private RoleManager<IdentityRole> _roleManager;

    private JwtService _jwtService;
    
    public UsersController(YDbContext dbContext, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager, JwtService jwtService)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
    }


    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterDto newUserDto)
    {
        if (_userManager.Users.FirstOrDefault(u => u.UserName == newUserDto.Username) != null)
        {
            return BadRequest($"Username {newUserDto.Username} is already taken");
        }

        if (_userManager.Users.FirstOrDefault(u => u.Email == newUserDto.email) != null)
        {
            return BadRequest(
                $"The Email Address {newUserDto.email} already exists. If you forgot your username, please click 'Forgot Username'");
        }

        AppUser newUser = new AppUser()
            { UserName = newUserDto.Username, Email = newUserDto.email, DisplayName = newUserDto.displayName };

        try
        {
            var createAccountResult = await _userManager.CreateAsync(newUser, newUserDto.password);

            if (!createAccountResult.Succeeded)
            {
                foreach (var error in createAccountResult.Errors)
                {
                    Console.WriteLine(error.Description);
                }
                
                return StatusCode(500, $"Failed to create user account. {createAccountResult.Errors.First().Description}");
            }

            var roleAssocationResult = await _userManager.AddToRoleAsync(newUser, "User");

            if (!roleAssocationResult.Succeeded)
            {
                Console.WriteLine($"Could not associate role 'User' to new user ");
                return BadRequest("Failed to create account.");
            }
        
            return Created($"/api/v1/users/{newUserDto.Username}",newUser);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            
            return StatusCode(500, "Failed to create user account. Please try again");

        }
      
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        AppUser? appUser = await _userManager.FindByNameAsync(loginDto.username);

        if (appUser == null)
        {
            return BadRequest("Invalid credentials!");
        }

        var loginResult = await _signInManager.CheckPasswordSignInAsync(appUser, loginDto.password, false);
        
        if (!loginResult.Succeeded)
        {
            return BadRequest("Invalid credentials!");
        }

        var userRoles = await _userManager.GetRolesAsync(appUser);
        
        //TODO: Generate JWT token

        List<Claim> tokenClaims = new List<Claim>();

        foreach (var role in userRoles)
        {
            tokenClaims.Add(new Claim(ClaimTypes.Role,role));
        }
        
        tokenClaims.Add(new Claim(ClaimTypes.Name,loginDto.username));
        tokenClaims.Add(new Claim(ClaimTypes.Email,appUser.Email));
        tokenClaims.Add(new Claim("DisplayName",appUser.DisplayName));
        
        string token = _jwtService.CreateToken(tokenClaims);


        HttpContext.Response.Headers.Add("access-token",token);
        return Ok();
    }
}