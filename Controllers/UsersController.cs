using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    private string? FetchUsernameFromRequestHeader(string token)
    {
        token = token.Replace("Bearer ", "");
        var tokenClaims = _jwtService.GetTokenClaims(token);

        return tokenClaims.FirstOrDefault(t => t.Type == "unique_name")?.Value;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetUserProfile()
    {
        string? username = FetchUsernameFromRequestHeader(HttpContext.Request.Headers.Authorization.ToString());

        if (String.IsNullOrEmpty(username))
            return BadRequest("Failed to fetch username from token");

        AppUser? userProfile = _dbContext.Users
            .Include(f=>f.Followers)
            .Include(f=>f.Following)
            .FirstOrDefault(u => u.UserName == username);

        if (userProfile is null)
            return BadRequest($"No profile with the username '{username}' found");

        UserDto profileDto = new UserDto()
        {
            Username = userProfile.UserName, Email = userProfile.Email, DisplayName = userProfile.DisplayName,
            Followers = userProfile.Followers.Select(f => f.UserName).ToList(),
            Following = userProfile.Following.Select(f => f.UserName).ToList()
        };
        
        return Ok(profileDto);
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
            { UserName = newUserDto.Username, Email = newUserDto.email, DisplayName = newUserDto.displayName,Followers = new List<AppUser>(),Following = new List<AppUser>()};

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

    [Authorize]
    [HttpPatch("follow")]
    public async Task<IActionResult> FollowUser([FromBody] FollowDto followDto)
    {
        //1. We're going to fetch the current user from our token and get their AppUser object - OK
        //2. We're going to get the AppUser object for the userToFollow. - OK
        //3. Verify their not already following each other.- OK
        //4. We're going to add the userToFollow in the current users 'following' array -OK
        //5. We're goin to add the current user's AppUser object into the userToFollow's followers array -OK

        string? username = FetchUsernameFromRequestHeader(HttpContext.Request.Headers.Authorization.ToString());
        
        if(String.IsNullOrEmpty(username))
            return StatusCode(500,"Failed to find username in the token");

        AppUser? currentUser = await _userManager.FindByNameAsync(username);

        if (currentUser is null)
            return BadRequest("User does not exist in the system");

        AppUser? appUserToFollow = await _userManager.FindByNameAsync(followDto.userToFollow);
        
        if (appUserToFollow is null)
            return BadRequest($"User '{followDto.userToFollow}' does not exist.");
        
        
        if (currentUser.Following.Any(f => f.UserName == appUserToFollow.UserName) && appUserToFollow.Followers.Any(f => f.UserName == currentUser.UserName))
        {
            return BadRequest($"The users '{currentUser.UserName}' and '{appUserToFollow.UserName}' already follow each other.");
        }


        try
        {
            //Follow each other
            currentUser.Following.Add(appUserToFollow);
            appUserToFollow.Followers.Add(currentUser);

            _dbContext.Users.Update(currentUser);
            _dbContext.Users.Update(appUserToFollow);

            await _dbContext.SaveChangesAsync();


            UserDto currentUserDto = new UserDto()
            {
                DisplayName = currentUser.DisplayName,
                Email = currentUser.Email,
                Username = currentUser.UserName,
                Followers = currentUser.Following.Select(c => c.UserName).ToList()
            };
            
            
            return Ok(currentUserDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Can't follow user {appUserToFollow.UserName}");
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            return StatusCode(500, $"Cannot follow user {appUserToFollow.UserName}");
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