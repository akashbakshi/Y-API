using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YApi.Models;

namespace YApi.Data;

public class YDbContext : IdentityDbContext<AppUser,IdentityRole,string>
{
    public YDbContext(DbContextOptions<YDbContext> options) : base(options)
    {

    }

    public DbSet<Tweet> Tweets { get; set; }
}
