using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YApi.Data;
using YApi.DTOs;
using YApi.Models;

namespace YApi.Controllers;


[ApiController]
[Route("/api/v1/[controller]")]
public class UsersController: Controller
{
    private YDbContext _dbContext;
    private UserManager<AppUser> _userManager;
    private SignInManager<AppUser> _signInManager;
    private RoleManager<IdentityRole> _roleManager;

    public UsersController(YDbContext dbContext, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }


    [HttpPost]
    public IActionResult Register([FromBody] RegisterDto newUserDto)
    {
        return Ok();
    }
}