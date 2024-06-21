using Microsoft.AspNetCore.Identity;

namespace YApi.Models;

public class AppUser: IdentityUser
{
    public string DisplayName { get; set; }
    
}