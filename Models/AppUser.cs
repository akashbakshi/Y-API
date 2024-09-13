using Microsoft.AspNetCore.Identity;

namespace YApi.Models;

public class AppUser: IdentityUser
{
    public string DisplayName { get; set; }
    
    public virtual List<AppUser> Followers { get; set; }= new List<AppUser>();
    
    public virtual List<AppUser> Following { get; set; }= new List<AppUser>();

    public virtual List<Tweet> LikedTweets { get; set; } = new List<Tweet>();


}