using Microsoft.AspNetCore.Identity;

namespace YApi.Models;

public class AppUser: IdentityUser
{
    public string DisplayName { get; set; }
    
    public virtual List<AppUser> Followers { get; set; }
    
    public virtual List<AppUser> Following { get; set; }
}