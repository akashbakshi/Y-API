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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        
        
        var userRole = new IdentityRole(){Name = "User", NormalizedName = "USER"};
        builder.Entity<IdentityRole>().HasData(userRole);
        base.OnModelCreating(builder);
    }

    public DbSet<Tweet> Tweets { get; set; }
}
